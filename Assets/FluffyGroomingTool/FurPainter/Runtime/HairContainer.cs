using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace FluffyGroomingTool {
    public class HairContainer : ScriptableObject {
        [SerializeField, HideInInspector] public HairStrandPoint[] hairStrandPoints;
        public int pointsPerStrand;
        [SerializeField, HideInInspector] public int id = -1324198676 + Guid.NewGuid().GetHashCode();
        [SerializeField] public float objectScaleAtSkinning;

        [SerializeField] public AnimationCurve shapeCurve = new AnimationCurve(
            new Keyframe(0, 1f, -0.007165052f, -0.007165052f, 0, 0.6583334f),
            new Keyframe(1f, 0f, -16.49596f, -16.49596f, 0.05833334f, 0)
        );

        [Range(0.00001f, 0.01f)] public float strandsWidth = 0.0005f;
        [SerializeField] public bool isSkinned = true;

        public ComputeBuffer createShapeBuffer(ComputeBuffer strandShapeBuffer) {
            float[] shapeWidthMultipliers = new float[pointsPerStrand];
            for (float i = 0; i < pointsPerStrand; i++) {
                shapeWidthMultipliers[(int) i] = shapeCurve.Evaluate(i / (pointsPerStrand - 1));
            }

            strandShapeBuffer ??= new ComputeBuffer(pointsPerStrand, sizeof(float));
            strandShapeBuffer.SetData(shapeWidthMultipliers);
            return strandShapeBuffer;
        }

        public void regenerateID() {
            id = -1324198676 + Guid.NewGuid().GetHashCode();
        }

        public static HairContainer createFromAlembicAndSkin(Vector3[] curvePoints, int strandPointsCount, GameObject skinToGameObject) {
            var meshBaker = createMeshBaker(skinToGameObject);
            var meshBakerBakedMesh = meshBaker.bakedMesh;
            var compute = Resources.Load<ComputeShader>("AlembicSkinning");
            var hairStrandPointsBuffer = new ComputeBuffer(curvePoints.Length, Marshal.SizeOf<HairStrandPointStruct>());
            compute.SetBuffer(0, "hairStrandPoints", hairStrandPointsBuffer);
            var skinToTransform = skinToGameObject.transform;
            compute.SetMatrix("inverseScaleMatrix", Matrix4x4.Scale(skinToTransform.localToWorldMatrix.inverse.lossyScale));

            var pointsBuffer = new ComputeBuffer(curvePoints.Length, sizeof(float) * 3);
            pointsBuffer.SetData(curvePoints);
            compute.SetBuffer(0, "curvePoints", pointsBuffer);

            compute.SetInt("strandPointsCount", strandPointsCount);

            compute.SetBuffer(0, ShaderID.SOURCE_MESH, meshBakerBakedMesh);
            compute.SetBuffer(0, "meshIndexBuffer", meshBaker.getIndexBuffer());
            compute.SetMatrix(ShaderID.LOCAL_TO_WORLD_MATRIX, skinToTransform.localToWorldMatrix);
            compute.SetMatrix(ShaderID.OBJECT_ROTATION_MATRIX, Matrix4x4.Rotate(skinToTransform.rotation));

            compute.SetInt("maxX", curvePoints.Length);
            compute.SetInt("maxY", meshBaker.sourceMesh.triangles.Length);

            var uvBuffer = new ComputeBuffer(meshBaker.sourceMesh.uv.Length, sizeof(float) * 2);
            uvBuffer.SetData(meshBaker.sourceMesh.uv);
            compute.SetBuffer(0, "uvBuffer", uvBuffer);

            compute.Dispatch(
                0,
                numGroups(curvePoints.Length / strandPointsCount, 16),
                numGroups(meshBaker.sourceMesh.triangles.Length / 3, 16),
                1
            );
            var hairStrandPoints = new HairStrandPointStruct[curvePoints.Length];
            hairStrandPointsBuffer.GetData(hairStrandPoints);

            var hairContainer = CreateInstance<HairContainer>();
            hairContainer.hairStrandPoints = hairStrandPoints.ToList().Select(it => it.convertToSObject()).ToArray();
            hairContainer.pointsPerStrand = strandPointsCount;

            meshBaker.dispose();
            hairStrandPointsBuffer.Dispose();
            pointsBuffer.Dispose();
            uvBuffer.Dispose();
            hairContainer.objectScaleAtSkinning = skinToTransform.lossyScale.x; //For now we only support uniform scaling.
            return hairContainer;
        }

        private static int numGroups(int totalThreads, int groupSize) {
            return (totalThreads + (groupSize - 1)) / groupSize;
        }

        private static MeshBaker createMeshBaker(GameObject skinToGameObject) {
            var meshBaker = new MeshBaker(skinToGameObject, Instantiate(Resources.Load<ComputeShader>(ShaderID.MESH_BAKER_CS_NAME)));
            if (!meshBaker.bakeSkinnedMesh(false) && meshBaker.isSkinnedMesh()) {
                //Sometimes the baking fails for skinned meshes, so we use the CPU backup variant instead
                var mesh = new Mesh();
                meshBaker.skinnedMeshRenderer.BakeMesh(mesh, true);
                meshBaker.dispose();
                return new MeshBaker(
                    mesh,
                    Instantiate(Resources.Load<ComputeShader>(ShaderID.MESH_BAKER_CS_NAME)),
                    MeshBaker.MeshBufferStride
                );
            }

            return meshBaker;
        }

        public static HairContainer createFromAlembicWithoutSkinning(Vector3[] positions, int strandPointsCount, List<Vector2> uvs) {
            var hairContainer = CreateInstance<HairContainer>();
            hairContainer.hairStrandPoints = positions.ToList().Select((pos, index) => new HairStrandPoint {
                barycentricCoordinate = pos,
                triangleIndices = Vector3.zero,
                rotationDiffFromNormal = Vector3.zero,
                distanceToRoot = 0,
                uv = index < uvs.Count - 1 ? uvs[index] : Vector2.zero
            }).ToArray();
            hairContainer.pointsPerStrand = strandPointsCount;
            hairContainer.isSkinned = false;
            return hairContainer;
        }
    }

    public struct HairStrandPointStruct {
        public Vector3 barycentricCoordinate;
        public Vector3 triangleIndices;
        public Vector3 rotationDiffFromNormal;
        public float distanceToRoot;
        public Vector2 uv;

        public HairStrandPoint convertToSObject() {
            return new HairStrandPoint {
                barycentricCoordinate = barycentricCoordinate,
                triangleIndices = triangleIndices,
                rotationDiffFromNormal = rotationDiffFromNormal,
                distanceToRoot = distanceToRoot,
                uv = uv
            };
        }
    }

    [Serializable]
    public struct HairStrandPoint {
        public Vector3 barycentricCoordinate;
        public Vector3 triangleIndices;
        public Vector3 rotationDiffFromNormal;
        public float distanceToRoot;
        public Vector2 uv;

        public HairStrandPointStruct convertToStruct() {
            return new HairStrandPointStruct() {
                barycentricCoordinate = barycentricCoordinate,
                triangleIndices = triangleIndices,
                rotationDiffFromNormal = rotationDiffFromNormal,
                distanceToRoot = distanceToRoot,
                uv = uv
            };
        }
    }
}