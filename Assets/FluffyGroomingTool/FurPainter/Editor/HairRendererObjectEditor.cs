using UnityEditor;
using UnityEngine;

namespace FluffyGroomingTool {
    [CustomEditor(typeof(HairRenderer))]
    public class HairRendererObjectEditor : Editor {
        private GeneralSettingsUI generalSettingsUI;
        private MotionSettingsUI motionSettingsUI;
        private WindSettingUI windSettingUI;

        private HairRenderer hairRenderer;
        private Styles styles = new Styles();
        private NormalUI normalUI;
        private CollidersUI collidersUI;
        private StrandShapeUI strandShapeUI;


        private void initialize() {
            hairRenderer = serializedObject.targetObject as HairRenderer;
            generalSettingsUI = new GeneralSettingsUI(serializedObject, "Main Settings", "isMainExpanded") {styles = styles};
            motionSettingsUI = new MotionSettingsUI(serializedObject, "Movement", "isMovementExpanded", "settings") {
                styles = styles
            };
            windSettingUI = new WindSettingUI(serializedObject, "Wind", "isWindExpanded", "settings") {styles = styles};
            normalUI = new NormalUI(serializedObject, "Normals", "isNormalExpanded", "settings") {styles = styles};
            strandShapeUI = new StrandShapeUI(serializedObject, "Strands Shape", "isStrandShapeExpanded") {
                styles = styles,
                hairRenderer = hairRenderer
            };
            collidersUI = new CollidersUI(serializedObject, "Colliders", "isColliderExpanded", "settings") {
                styles = styles,
                furRenderer = null,
                hairRenderer = hairRenderer
            };
            EditorUtility.SetSelectedRenderState((serializedObject.targetObject as MonoBehaviour)?.GetComponent<Renderer>(),
                EditorSelectedRenderState.Hidden);
        }

        public override void OnInspectorGUI() {
            if (generalSettingsUI == null) initialize();
            serializedObject.Update();
            generalSettingsUI.drawUI();
            strandShapeUI.drawUI();
            if (generalSettingsUI.hasNoHairContainer()) {
                serializedObject.ApplyModifiedProperties();
                return;
            }

            motionSettingsUI.drawUI();
            if (hairRenderer.CurrentRenderer != null) {
                normalUI.hasRenderer = hairRenderer.CurrentRenderer != null;
                normalUI.drawUI();
            }

            windSettingUI.drawUI();
            collidersUI.drawUI();
            generalSettingsUI.drawMaterialUI();
            serializedObject.ApplyModifiedProperties();
        }
    }

    class StrandShapeUI : HeaderUI {
        public HairRenderer hairRenderer;
        private Color32 color = new Color32(55, 210, 232, 255);

        public StrandShapeUI(SerializedObject serializedObject, string header, string headerProperty) : base(
            serializedObject, header, headerProperty) { }

        public override void drawContent() {
            GUILayout.BeginVertical(styles.PanelStyle);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.CurveField(new GUIContent("Strand Shape Curve", "The shape curve of each strand from root to tip."),
                hairRenderer.hairContainer.shapeCurve,
                color,
                new Rect(0f, 0f, 1, 1f)
            );

            EditorGUILayout.LabelField("Strands Width:");
            var lastRect = GUILayoutUtility.GetLastRect();
            lastRect.y += 23;
            lastRect.height = 18;
            hairRenderer.hairContainer.strandsWidth =
                GUI.HorizontalSlider(lastRect, hairRenderer.hairContainer.strandsWidth, 0.00007f, 0.01f);

            if (EditorGUI.EndChangeCheck()) {
#if UNITY_EDITOR
                EditorUtility.SetDirty(hairRenderer.hairContainer);
                hairRenderer.rebuildShapeBuffer();
#endif
            }

            GUILayout.Space(35);
            var panelSize = MeshCardPropertiesUI.PANEL_SIZE;
            GUILayout.BeginHorizontal(styles.PanelStyle, GUILayout.MaxWidth(panelSize), GUILayout.MinHeight(panelSize));
            EditorGUILayout.Space(panelSize);
            GUILayout.EndHorizontal();
            lastRect = GUILayoutUtility.GetLastRect();
            lastRect.width = MeshCardPropertiesUI.PREVIEW_HEIGHT;
            MeshCardPropertiesUI.drawGrid(hairRenderer.hairContainer.shapeCurve, hairRenderer.hairContainer.pointsPerStrand, lastRect, color);

            GUILayout.EndVertical();
            GUILayout.Space(PainterResetAndSmoothUI.DEFAULT_MARGIN_TOP);
        }
    }
}