using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace FluffyGroomingTool {
    [ExecuteAlways]
    public class HairRenderer : MonoBehaviour {
        public HairContainer hairContainer;
        public Camera lodCamera;
        public Material material;
        public FurRendererSettings settings;
        public SphereCollider[] sphereColliders;
        public CapsuleCollider[] capsuleColliders;
        public SDFCollider[] sdfColliders; //TODO

        public bool motionVectors = true;

        private ComputeBuffer hairStrandPointsBuffer, verletNodesBuffer, strandShapeBuffer;
        private ComputeShader hairRendererCompute;
        private int allInOneKernel;
        private Material motionVectorMaterial;
        private MeshBaker meshBaker;

        private SDFColliderCommon sdfColliderCommon;
        private ComputeBuffer colliderBuffer;
        private ColliderStruct[] collidersStruct;
        internal readonly FluffyRenderersController fluffyRenderersController = new FluffyRenderersController();
        public bool isUrp;
        public HeadersExpanded headerExpanded = new HeadersExpanded();

        private void OnEnable() {
            initialize();
            if (Application.isPlaying && !ColliderHelper.collidersAssigned(sphereColliders, capsuleColliders)) {
                ErrorLogger.logNoColliders();
            }

            RenderPipelineManager.beginFrameRendering += beginFrameRendering;
            Camera.onPreRender += cameraPreRender;
            DuplicateCleaner.checkDuplicates();
        }

        internal int hairContainerID;

        private void checkForHairContainer() {
            if (hairContainerID != hairContainer.id) {
                var existingFurCreator = GetComponent<FurCreator>();
                if (existingFurCreator != null) {
                    DestroyImmediate(existingFurCreator);
                }

                clearResources();
                initialize();
            }
        }


        private void loadDefaultMaterial() {
            CurrentRenderer = GetComponent<Renderer>();
            DefaultMaterialLoader.loadDefaultMaterial(
                out _, out isUrp, ref material, out _, ref motionVectorMaterial, "Strands", CurrentRenderer
            );
        }

        private void initialize() {
            if (hairContainer != null) {
                if (settings == null) {
                    settings = new FurRendererSettings {
                        verletSimulationSettings = new VerletSimulationSettings(),
                        sourceMeshNormalToStrandNormalPercent = 0.9f
                    };
                }

                hairContainerID = hairContainer.id;
                if (hairContainer.isSkinned) {
                    meshBaker = new MeshBaker(gameObject, Instantiate(Resources.Load<ComputeShader>(ShaderID.MESH_BAKER_CS_NAME)));
                    sdfColliderCommon = new SDFColliderCommon(GetComponent<Renderer>(), meshBaker, settings.verletSimulationSettings);
                }

                loadDefaultMaterial();
                var pointsCount = hairContainer.hairStrandPoints.Length;
                var verticesCount = pointsCount * 2;
                verletNodesBuffer = new ComputeBuffer(
                    hairContainer.hairStrandPoints.Length,
                    sizeof(float) * 15 + sizeof(int),
                    ComputeBufferType.Default
                );
                var triIndices =
                    FurMeshCreator.generateTriangleIndicesForStrands(hairContainer.pointsPerStrand, pointsCount / hairContainer.pointsPerStrand);

                hairRendererCompute = Instantiate(Resources.Load<ComputeShader>("HairRenderer"));
                allInOneKernel = hairRendererCompute.FindKernel("AllInOneKernel");
                hairRendererCompute.SetInt("strandPointsCount", hairContainer.pointsPerStrand);
                ColliderHelper.setupCollidersBuffer(ref colliderBuffer, ref collidersStruct, sphereColliders, capsuleColliders);

                fluffyRenderersController.createRendererObject(DefaultMaterialLoader.isHdrp(), isUrp, motionVectorMaterial, verticesCount,
                    triIndices);
                var furMeshBufferStride = fluffyRenderersController.getVertexBufferStride();
                hairRendererCompute.SetInt(ShaderID.FUR_MESH_BUFFER_STRIDE, furMeshBufferStride);
                hairRendererCompute.SetBuffer(allInOneKernel, ShaderID.FUR_MESH_BUFFER, fluffyRenderersController.hairMeshBuffer);

                hairStrandPointsBuffer = new ComputeBuffer(hairContainer.hairStrandPoints.Length, Marshal.SizeOf<HairStrandPointStruct>());
                hairStrandPointsBuffer.SetData(hairContainer.hairStrandPoints.ToList().Select(it => it.convertToStruct()).ToArray());
                hairRendererCompute.SetBuffer(allInOneKernel, "hairStrandPoints", hairStrandPointsBuffer);
                hairRendererCompute.EnableKeyword("INITIALIZE_VERLET_NODES");
                strandShapeBuffer = hairContainer.createShapeBuffer(null);
            }
        }

        private void updateAndRenderHair() {
            if (hairContainer != null) { 
                fluffyRenderersController.setupRenderers(false, material, transform.position, CurrentRenderer, isUrp, motionVectors);
                var thisTransform = transform;
                if (hairContainer.isSkinned) {
                    if (meshBaker == null) {
                        initialize();
                        return;
                    }

                    meshBaker.bakeSkinnedMesh(false);
                    if (settings.verletSimulationSettings.isVerletColliderEnabled()) {
                        sdfColliderCommon.createSDF(thisTransform, hairRendererCompute, allInOneKernel);
                    }

                    hairRendererCompute.SetBuffer(allInOneKernel, ShaderID.SOURCE_MESH, meshBaker.bakedMesh);
                }


                var rotation = thisTransform.rotation;
                hairRendererCompute.SetMatrix(ShaderID.LOCAL_TO_WORLD_MATRIX, thisTransform.localToWorldMatrix);
                hairRendererCompute.SetMatrix(ShaderID.OBJECT_ROTATION_MATRIX, Matrix4x4.Rotate(rotation));
                hairRendererCompute.SetBuffer(allInOneKernel, "strandShapeBuffer", strandShapeBuffer);
                hairRendererCompute.SetBuffer(allInOneKernel, "verletNodes", verletNodesBuffer);
                hairRendererCompute.SetFloat("strandsWidth", getStrandsWidth());

                var strandNodesCount = hairContainer.pointsPerStrand; //Get from layer
                int nearestPow = CullAndSortController.nextPowerOf2(strandNodesCount);
                var nodesCount = hairContainer.hairStrandPoints.Length;
                var strandsCount = nodesCount / strandNodesCount;
                int dispatchCount = nearestPow * strandsCount;
                var layerNodeStartIndex = 0;

                //This should be done in a common function in VerletSimulation.cs
                hairRendererCompute.SetInt("nearestPow", nearestPow);
                hairRendererCompute.SetInt("strandPointsCount", strandNodesCount);
                hairRendererCompute.SetVector("worldSpaceCameraPos", lodCamera.getCamera().transform.position);
                hairRendererCompute.SetFloat("sourceMeshNormalToStrandNormalPercent", settings.sourceMeshNormalToStrandNormalPercent);
                hairRendererCompute.SetInt("verletNodesCount", nodesCount);
                hairRendererCompute.SetInt("layerVertexStartIndex", layerNodeStartIndex);
                hairRendererCompute.SetFloat("shapeConstraintRoot", settings.verletSimulationSettings.stiffnessRoot);
                hairRendererCompute.SetFloat("keepShapeStrength", settings.verletSimulationSettings.keepShapeStrength);
                hairRendererCompute.SetFloat("shapeConstraintTip", settings.verletSimulationSettings.stiffnessTip);
                hairRendererCompute.SetInt("numberOfFixedNodesInStrand", settings.verletSimulationSettings.isFirstNodeFixed ? 1 : 0);
                hairRendererCompute.setKeywordEnabled("IS_SKINNED", hairContainer.isSkinned);

                hairRendererCompute.SetVector("_Gravity", settings.verletSimulationSettings.gravity);
                hairRendererCompute.SetFloat("deltaTime", Time.smoothDeltaTime);

                hairRendererCompute.SetFloat("_Decay", 1f - settings.verletSimulationSettings.drag);
                hairRendererCompute.SetFloat("stepSize", 1f / settings.verletSimulationSettings.constraintIterations);
                hairRendererCompute.SetInt("solverIterations", settings.verletSimulationSettings.constraintIterations);
                ColliderHelper.setupColliderProperties(ref colliderBuffer, ref collidersStruct, sphereColliders, capsuleColliders,
                    hairRendererCompute,
                    allInOneKernel);

                VerletSimulation.setupWind(hairRendererCompute, allInOneKernel, settings);
                hairRendererCompute.setKeywordEnabled("VERLET_ENABLED", Application.isPlaying && settings.verletSimulationSettings.enableMovement);

                hairRendererCompute.setKeywordEnabled("COLLIDE_WITH_SOURCE_MESH",
                    settings.verletSimulationSettings.isVerletColliderEnabled() && hairContainer.isSkinned);
                hairRendererCompute.setKeywordEnabled("USE_FORWARD_COLLISION", settings.verletSimulationSettings.useForwardCollision);
                hairRendererCompute.SetFloat(ShaderID.EXTRA_SCALE, 1f + (thisTransform.lossyScale.x - hairContainer.objectScaleAtSkinning));
                hairRendererCompute.Dispatch(allInOneKernel, dispatchCount.toCsGroups(), 1, 1);
                hairRendererCompute.DisableKeyword("INITIALIZE_VERLET_NODES");
                collideWithOtherSDFColliders();
            }
        }

        private float getStrandsWidth() { return FurCreator.getInterpolatedWidth(hairContainer.strandsWidth, 0.0025f); }

        private void collideWithOtherSDFColliders() {
            if (settings.verletSimulationSettings.isSDFCollisionEnabled() && sdfColliders != null) {
                foreach (var sdfCollider in sdfColliders) {
                    if (sdfCollider != null) {
                        sdfCollider.collideWith(verletNodesBuffer, fluffyRenderersController.getRendererBounds(),
                            fluffyRenderersController.hairMeshBuffer,
                            fluffyRenderersController.getVertexBufferStride());
                    }
                }
            }
        }

        private void beginFrameRendering(ScriptableRenderContext rc, Camera[] cameras) { updateAndRenderHair(); }

        private void cameraPreRender(Camera cam) {
            if (lodCamera.getCamera() == cam) {
                updateAndRenderHair();
            }
        }

        private void OnDisable() {
            clearResources();
            RenderPipelineManager.beginFrameRendering -= beginFrameRendering;
            Camera.onPreRender -= cameraPreRender;
        }

        private void clearResources() {
            meshBaker?.dispose();
            fluffyRenderersController.destroy();
            hairStrandPointsBuffer?.Dispose();
            verletNodesBuffer?.Dispose();
            sdfColliderCommon?.dispose();
            strandShapeBuffer?.Dispose();
        }

        public Renderer CurrentRenderer { get; set; }

        public void recreateSdfCollider() {
            if (hairContainer.isSkinned) {
                sdfColliderCommon?.dispose();
                sdfColliderCommon = new SDFColliderCommon(GetComponent<Renderer>(), meshBaker, settings.verletSimulationSettings);
            }
        }

        public void recreate() {
            clearResources();
            initialize();
        }

        void Update() { checkForHairContainer(); }

        public void rebuildShapeBuffer() { hairContainer.createShapeBuffer(strandShapeBuffer); }
    }
}