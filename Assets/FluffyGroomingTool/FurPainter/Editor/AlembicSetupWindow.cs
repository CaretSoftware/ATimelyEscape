using System;
using System.Collections.Generic;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Formats.Alembic.Importer;

namespace FluffyGroomingTool {
    public class AlembicSetupWindow : EditorWindow {
        [MenuItem("Tools/Fluffy Grooming Tool/Alembic Groom Setup", false, 2)]
        public static AlembicSetupWindow launchFurPainter() {
            var window = GetWindowWithRect<AlembicSetupWindow>(new Rect(0, 0, 600, 665));
            window.titleContent = new GUIContent("Alembic Setup");
            window.Show();
            return window;
        }

        private SerializedProperty sourceGamObject;
        private SerializedProperty alembicGroom;
        private SerializedProperty dontSkin;
        private GameObject helper;
        private SerializedObject seriaLizedObject;
        private Editor sourceEditor;
        private GameObject combinedObject;

        private GUIStyle headingStyle;
        private GUIStyle textStyle;
        private GUIStyle panelStyle;
        private GUIStyle panelOutlineStyle;
        private GUIStyle vignetteStyle;
        private GUIStyle buttonStyle;
        private GUIStyle buttonStyleSelected;
        private readonly EditorDeltaTime editorDeltaTime = new EditorDeltaTime();
        private List<Renderer> renderersInSourceMesh;
        private Vector2 scroll;
        private static readonly float BUILD_BUTTON_SCALE = 0.34f;
        private static readonly float BUILD_BUTTON_WIDTH = 618 * BUILD_BUTTON_SCALE;
        private static readonly float BUILD_BUTTON_HEIGHT = 150f * BUILD_BUTTON_SCALE;

        private ImportantButton startSkinningButton;
        private ToastMessage selectionLockedMessage = new ToastMessage();
        private int selectedRendererIndex;
        private Mesh errorMesh;
        private bool isMeshInWrongFormat;

        private void skinAndCreateObject() {
            var curveScripts = combinedObject.GetComponentsInChildren<AlembicCurves>();
            if (curveScripts.Length == 0) ErrorLogger.logNoCurvesFound();
            var points = new List<Vector3>();
            var uvs = new List<Vector2>();
            foreach (var cs in curveScripts) {
                points.AddRange(cs.Positions);
                uvs.AddRange(cs.UVs);
            }

            var pointsPerStrand = curveScripts[0].CurveOffsets[1];
            if (!dontSkin.boolValue) {
                var skinToObject = renderersInSourceMesh[selectedRendererIndex].gameObject;
                var hairContainer = HairContainer.createFromAlembicAndSkin(points.ToArray(), pointsPerStrand, skinToObject);
                var newObject = (GameObject) Instantiate(sourceGamObject.objectReferenceValue);
                newObject.name = sourceGamObject.objectReferenceValue.name;
                var names = new List<string>();
                parentNames(skinToObject.transform, ref names);
                names.RemoveAt(0);
                var parentNamesS = String.Join("/", names.ToArray());
                skinToObject = parentNamesS.Length == 0 ? newObject : newObject.transform.Find(parentNamesS).gameObject;

                var hairRenderer = skinToObject.AddComponent<HairRenderer>();
                finalizeAndSave(hairContainer, skinToObject, hairRenderer, newObject);
            }
            else {
                var newObject = new GameObject {name = alembicGroom.name + "HairRenderer"};
                var hairContainer = HairContainer.createFromAlembicWithoutSkinning(points.ToArray(), pointsPerStrand, uvs);
                var hairRenderer = newObject.AddComponent<HairRenderer>();
                finalizeAndSave(hairContainer, newObject, hairRenderer, newObject);
            }
        }

        private bool closeWindow;

        private void finalizeAndSave(HairContainer hairContainer, GameObject skinToObject, HairRenderer hairRenderer, GameObject newObject) {
            hairContainer = saveHairContainer(hairContainer, skinToObject);
            if (hairContainer != null) {
                hairRenderer.hairContainer = hairContainer;
                hairRenderer.recreate();
                Selection.activeObject = newObject;
                EditorGUIUtility.PingObject(Selection.activeObject);
                closeWindow = true;
            }
            else {
                startSkinningButton.disableCircle = false;
                DestroyImmediate(newObject);
            }
        }

        void parentNames(Transform transform, ref List<string> names) {
            names.Insert(0, transform.name);
            if (transform.parent != null) {
                parentNames(transform.parent, ref names);
            }
        }

        private GUIStyle createTextHeadingStyle() {
            var guiStyle = new GUIStyle(EditorStyles.label) {
                fontStyle = FontStyle.Bold,
                wordWrap = true
            };
            return guiStyle;
        }

        private void OnDisable() {
            destroyResources();
        }

        private void destroyResources() {
            if (combinedObject != null) DestroyImmediate(combinedObject);
            if (!(!ReferenceEquals(sourceEditor, null) && sourceEditor == null)) DestroyImmediate(sourceEditor);
            if (helper != null) DestroyImmediate(helper);
            seriaLizedObject = null;
            combinedObject = null;
            sourceEditor = null;
            helper = null;

            sourceGamObject = null;
            alembicGroom = null;
            dontSkin = null;
        }

        private void OnEnable() {
            initialize();
        }

        private void initialize() {
            helper = new GameObject();
            seriaLizedObject = new SerializedObject(helper.AddComponent<AlembicSetupHelper>());
            helper.hideFlags = HideFlags.HideInHierarchy;
            sourceGamObject = seriaLizedObject.FindProperty("sourceGameObject");
            alembicGroom = seriaLizedObject.FindProperty("alembicFile");
            dontSkin = seriaLizedObject.FindProperty("dontSkin");
            startSkinningButton = new ImportantButton() {
                positionRect = new Rect(),
                resource = "build_hairs",
                gradientResource = "rate_button_gradient",
                disableCircleAfterClick = true,
                clickAction = skinAndCreateObject
            };
            renderersInSourceMesh = new List<Renderer>();
        }


        private void OnGUI() {
#if UNITY_EDITOR
            if (BuildPipeline.isBuildingPlayer) return;
            if (seriaLizedObject?.targetObject == null) {
                recreate();
            }
#endif
            if (closeWindow) {
                Close();
            }
            else {
                GUI.color = Color.black;
                GUI.DrawTexture(new Rect(0, 0, position.width, position.height), EditorGUIUtility.whiteTexture);
                GUI.color = Color.white;
                selectionLockedMessage.fixedColorIndex = 4;
                selectionLockedMessage.drawMessage(position.width);
                scroll = GUILayout.BeginScrollView(scroll, false, false);
                EditorGUILayout.BeginVertical();
                GUILayout.Space(15);
                createStyles();
                EditorGUILayout.BeginVertical(panelStyle);
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.LabelField("Please assign the Alembic .abc file that contains the hairs(splines) and " +
                                           "assign the source GameObject that the hairs should be skinned to." +
                                           " The source GameObject should be the same that was used when grooming " +
                                           "the alembic file in the third party app and should contain a MeshRenderer/SkinnedMeshRenderer " +
                                           "that will be used for skinning.", headingStyle);
                GUILayout.Space(17);
                EditorGUILayout.PropertyField(alembicGroom);
                if (!dontSkin.boolValue) {
                    EditorGUILayout.PropertyField(sourceGamObject);
                }

                EditorGUILayout.PropertyField(dontSkin,
                        new GUIContent("Don't skin to a Mesh",
                            "Use this option when you don't need the hair to be skinned to a source mesh. For instance human hair often doesn't need to be skinned to a mesh."))
                    ;
                if (GUI.changed) {
                    isInErrorState = errorCheckFields();
                    createPreview();
                }

                EditorGUILayout.EndVertical();
                drawStartSkinningButton();
                drawBetaImage();
                drawMeshWrongStateButton();
                seriaLizedObject?.ApplyModifiedProperties();
                EditorGUILayout.EndVertical();
                GUILayout.EndScrollView();
                Repaint();
            }
        }

        private void recreate() {
            destroyResources();
            initialize();
        }

        private void drawMeshWrongStateButton() {
            if (isMeshInWrongFormat) {
                if (AddFurCreatorUI.draw32IndexFormatWarning(errorMesh, panelStyle, headingStyle, buttonStyle)) {
                    createSourceAndAlembicPreviewObject();
                }
            }
        }

        private bool isInErrorState;

        private bool errorCheckFields() {
            if (sourceGamObject.objectReferenceValue != null) {
                var renderers = (sourceGamObject.objectReferenceValue as GameObject)?.GetComponentsInChildren<Renderer>();
                if (renderers == null || renderers.Length == 0) {
                    showToastMessage("The Source GameObject does not contain any renderers.");
                    return true;
                }

                var sourceObjectAlembicCurves = (sourceGamObject.objectReferenceValue as GameObject)?.GetComponentsInChildren<AlembicStreamPlayer>();
                if (sourceObjectAlembicCurves != null && sourceObjectAlembicCurves.Length > 0) {
                    showToastMessage("The Alembic file should be assigned in the field above :)");
                    return true;
                }
            }

            var alembicCurves = (alembicGroom.objectReferenceValue as AlembicStreamPlayer)?.gameObject.GetComponentsInChildren<AlembicCurves>();
            if (alembicGroom.objectReferenceValue != null && alembicCurves == null || alembicCurves?.Length == 0) {
                showToastMessage("The assigned Alembic file does not contain an AlembicCurves.");
                return true;
            }

            return false;
        }

        private void showToastMessage(string me) {
            selectionLockedMessage.show = true;
            selectionLockedMessage.messageText = me;
            selectionLockedMessage.autoHide();
        }

        private void drawBetaImage() {
            if (shouldNotDrawPreview()) {
                betaTexture ??= Resources.Load<Texture2D>("beta_img");
                GUI.DrawTexture(new Rect(0, position.height - 355, position.width, 192), betaTexture);
            }
        }

        private Texture2D betaTexture;

        private void drawStartSkinningButton() {
            if (sourceEditor != null && !isInErrorState && !isMeshInWrongFormat) {
                editorDeltaTime.Update();
                startSkinningButton.update(editorDeltaTime.deltaTime);
                if (renderersInSourceMesh.Count > 1) {
                    drawWithMultipleRenderers();
                }
                else {
                    drawWithOnlyOneRendererFound();
                }
            }
        }

        private void drawWithMultipleRenderers() {
            if (shouldNotDrawPreview()) return;
            EditorGUILayout.BeginVertical(panelStyle, GUILayout.Width(200), GUILayout.Height(300));
            EditorGUILayout.LabelField("Please select which object\nthe hairs should be skinned to.", headingStyle,
                GUILayout.Width(200), GUILayout.Height(30));
            EditorGUILayout.Space(15);
            for (var index = 0; index < renderersInSourceMesh.Count; index++) {
                var renderer = renderersInSourceMesh[index];
                if (GUILayout.Button(renderer.gameObject.name, selectedRendererIndex == index ? buttonStyleSelected : buttonStyle)) {
                    selectedRendererIndex = index;
                }

                EditorGUILayout.Space(3);
            }

            EditorGUILayout.EndVertical();
            var previewSize = new Rect(245, 186, 590 - 241, 296);
            sourceEditor.DrawPreview(previewSize);
            if (shouldDrawSkinningButton()) {
                startSkinningButton.positionRect = createBuildButtonRect(122);
                startSkinningButton.draw();
                previewSize.y += 277;
                drawCheckScaleAndLayoutText(previewSize, true);
                textStyle.alignment = TextAnchor.MiddleLeft;
            }

            if (Event.current.type == EventType.Repaint) {
                var vignetteAndOutlineRect = new Rect(242, 185, 590 - 235, 301);
                vignetteStyle.Draw(vignetteAndOutlineRect, false, true, false, false);
                panelOutlineStyle.Draw(vignetteAndOutlineRect, false, true, false, false);
            }
        }

        private bool shouldNotDrawPreview() {
            return sourceEditor == null || alembicGroom.objectReferenceValue == null && dontSkin.boolValue;
        }

        private void drawWithOnlyOneRendererFound() {
            if (shouldNotDrawPreview()) return;
            var previewSize = new Rect(7, 186, 588, 297);
            sourceEditor.DrawPreview(previewSize);
            if (shouldDrawSkinningButton()) {
                startSkinningButton.positionRect = createBuildButtonRect(0);
                startSkinningButton.draw();
                previewSize.y += 277;
                drawCheckScaleAndLayoutText(previewSize, false);
            }

            if (Event.current.type == EventType.Repaint) {
                var vignetteAndOutlineRect = new Rect(4, 185, 594, 301);
                vignetteStyle.Draw(vignetteAndOutlineRect, false, true, false, false);
                panelOutlineStyle.Draw(vignetteAndOutlineRect, false, true, false, false);
            }
        }

        private void drawCheckScaleAndLayoutText(Rect previewSize, bool isNarrovLayout) {
            textStyle ??= EditorStyles.label;
            textStyle.alignment = TextAnchor.MiddleCenter;
            textStyle.normal.textColor = Color.white;
            if (isNarrovLayout) {
                GUI.Label(previewSize,
                    "Please check that the objects are aligned\nproperly and make sure the scaling looks\ncorrect in the preview. You can adjust the\nScale Factor in the Model Import Settings.",
                    textStyle);
            }
            else {
                GUI.Label(previewSize,
                    "Please check that the objects are aligned properly\nand make sure the scaling looks correct in the preview.\nYou can adjust the Scale Factor in the Model Import Settings.",
                    textStyle);
            }

            textStyle.alignment = TextAnchor.MiddleLeft;
        }

        private bool shouldDrawSkinningButton() {
            return sourceGamObject.objectReferenceValue != null && alembicGroom.objectReferenceValue != null ||
                   alembicGroom.objectReferenceValue != null && dontSkin.boolValue;
        }

        private Rect createBuildButtonRect(float xOffset) {
            return new Rect(
                position.width / 2f - BUILD_BUTTON_WIDTH / 2f + xOffset,
                515f,
                BUILD_BUTTON_WIDTH,
                BUILD_BUTTON_HEIGHT
            );
        }

        private void createStyles() {
            headingStyle ??= createTextHeadingStyle();
            panelStyle ??= BrushPropertiesUI.createDefaultPanelStyle();
            panelOutlineStyle ??= BrushPropertiesUI.createDefaultPanelStyle("bg_box_pink");
            vignetteStyle ??= BrushPropertiesUI.createDefaultPanelStyle("vignette_tex");
            buttonStyle ??= PainterLayersUI.createButtonStyle("bg_box", "bg_box");
            buttonStyle.padding = new RectOffset(28, 16, 8, 10);
            buttonStyleSelected ??= PainterLayersUI.createButtonStyle("bg_box_blue", "bg_box_blue");
            buttonStyleSelected.padding = new RectOffset(28, 16, 8, 10);
        }

        private void createPreview() {
            if (combinedObject != null) DestroyImmediate(combinedObject);
            if (sourceEditor != null) DestroyImmediate(sourceEditor);
            if (alembicGroom.objectReferenceValue != null ||
                sourceGamObject.objectReferenceValue != null && !isInErrorState && !isMeshInWrongFormat) {
                combinedObject = createSourceAndAlembicPreviewObject();
                sourceEditor = Editor.CreateEditor(combinedObject);
            }
        }

        private GameObject createSourceAndAlembicPreviewObject() {
            GameObject co = new GameObject();
            co.hideFlags = HideFlags.HideAndDontSave;
            if (sourceGamObject.objectReferenceValue != null && !dontSkin.boolValue) {
                var sourceGameObject = Instantiate((GameObject) sourceGamObject.objectReferenceValue, co.transform, true);
                sourceGameObject.hideFlags = HideFlags.HideAndDontSave;
                selectedRendererIndex = 0;
                renderersInSourceMesh.Clear();
                var meshRenderers = sourceGamObject.objectReferenceValue.GetComponentsInChildren<MeshRenderer>();
                renderersInSourceMesh.AddRange(meshRenderers);
                var skinnedMeshRenderers = sourceGamObject.objectReferenceValue.GetComponentsInChildren<SkinnedMeshRenderer>();
                renderersInSourceMesh.AddRange(skinnedMeshRenderers);
                var mesh = meshRenderers.Length > 0 ? meshRenderers[0].GetComponent<MeshFilter>().sharedMesh : skinnedMeshRenderers[0].sharedMesh;
                isMeshInWrongFormat = mesh == null || AddFurCreatorUI.isMedUnreadableOrIndexFormat16(mesh);
                errorMesh = isMeshInWrongFormat ? mesh : null;
            }

            if (alembicGroom.objectReferenceValue != null) {
                var alembicPreview = Instantiate(((AlembicStreamPlayer) alembicGroom.objectReferenceValue).gameObject, co.transform, true);
                alembicPreview.hideFlags = HideFlags.HideAndDontSave;
                ensureCurveRenderer(alembicPreview);
            }

            //If your camera is ever places on this position and you accidentally see the preview object in your scene. 
            //Please send me a screenshot to daniel@danielzeller.no and let me know you've unlocked an easter egg. 
            co.transform.position = Vector3.left * 10000000000000000000;

            return co;
        }

        private void ensureCurveRenderer(GameObject alembicPreview) {
            var curves = alembicPreview.GetComponentsInChildren<AlembicCurves>();
            foreach (var curve in curves) {
                var renderer = curve.GetComponent<AlembicCurvesRenderer>();
                if (renderer == null) {
                    renderer = curve.AddComponent<AlembicCurvesRenderer>();
                }

                var meshRenderer = renderer.GetComponent<MeshRenderer>();
                if (meshRenderer != null && renderersInSourceMesh.Count > 0) {
                    Material material = null;
                    Material mvm = null;
                    DefaultMaterialLoader.loadDefaultMaterial(out bool _, out bool _, ref material, out Material _, ref mvm,
                        "Strands", renderersInSourceMesh[selectedRendererIndex]);
                    meshRenderer.material = material;
                }
            }
        }

        private static HairContainer saveHairContainer(HairContainer hairRenderer, string path) {
#if UNITY_EDITOR
            var containerCopy = Instantiate(hairRenderer);
            AssetDatabase.CreateAsset(containerCopy, path);
            Selection.activeObject = containerCopy;
            return containerCopy;
#endif
        }

        public static HairContainer saveHairContainer(HairContainer hairContainer, GameObject skinTo) {
#if UNITY_EDITOR

            var path = EditorUtility.SaveFilePanel("Save The HairContainer", "Assets/", skinTo.name + "HairContainer", "asset");
            if (!string.IsNullOrEmpty(path)) {
                hairContainer.regenerateID();
                path = FileUtil.GetProjectRelativePath(path);

                var existingFurContainer = AssetDatabase.LoadAssetAtPath<HairContainer>(path);
                if (existingFurContainer != null && existingFurContainer != hairContainer) {
                    AssetDatabase.DeleteAsset(path);
                    hairContainer = saveHairContainer(hairContainer, path);
                }
                else if (existingFurContainer == null) {
                    hairContainer = saveHairContainer(hairContainer, path);
                }


                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.FocusProjectWindow();
                return hairContainer;
            }

            return null;
#endif
        }
    }
}