using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AdditiveSceneManager))]
public class AdditiveSceneEditor : Editor {
	
	private AdditiveSceneManager _asm;
	private GUIContent _save;
	private GUIContent _visibilityMixed;
	private GUIContent _visibilityHidden;
	private GUIContent[] _loads;
	
	public override void OnInspectorGUI() {
		Initialize();
		SaveButton();
		LoadButtons();
	}

	private void Initialize() {
		_asm ??= (AdditiveSceneManager)target;
		
		_save ??=
				new GUIContent(" Save",
						EditorGUIUtility.IconContent("d_SaveAs").image,
						"Save All open additive scenes");
		_visibilityMixed ??= 
				new GUIContent(" Load", 
						EditorGUIUtility.IconContent("d_scenevis_visible-mixed_hover").image, 
						"Load and Open all unopened scenes");
		_visibilityHidden ??= 
				new GUIContent(" Load Unloaded", 
						EditorGUIUtility.IconContent("d_scenevis_hidden_hover").image, 
						"Load all unopened scenes unloaded");

		_loads ??= new GUIContent[] { _visibilityMixed, _visibilityHidden };
	}

	private void SaveButton() {
		if (GUILayout.Button(_save))
			_asm.SaveScenes();
	}

	private void LoadButtons() {
		GUILayout.BeginHorizontal();

		int selected = -1;
		selected = GUILayout.SelectionGrid(selected, _loads, 2);
		
		if (selected == 0)
			_asm.LoadScenes();
		if (selected == 1)
			_asm.LoadScenes(false);

		GUILayout.EndHorizontal();
	}
}
