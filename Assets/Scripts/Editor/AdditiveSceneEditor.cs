using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AdditiveSceneManager))]
public class AdditiveSceneEditor : Editor {
	public override void OnInspectorGUI() {
		AdditiveSceneManager ase = (AdditiveSceneManager) target;
		
		GUIContent save =
				new GUIContent(" Save",
						EditorGUIUtility.IconContent("d_SaveAs").image,
				"Save All open additive scenes");
		GUIContent visibilityMixed = 
				new GUIContent(" Load", 
						EditorGUIUtility.IconContent("d_scenevis_visible-mixed_hover@2x").image, 
						"Load and Open all unopened scenes");
		GUIContent hidden = 
				new GUIContent(" Load Unloaded", 
						EditorGUIUtility.IconContent("d_scenevis_hidden_hover@2x").image, 
						"Load all unopened scenes unloaded");

		GUIContent[] loads = new GUIContent[] { visibilityMixed, hidden };
		
		if (GUILayout.Button(save))
			ase.SaveScenes();

		GUILayout.BeginHorizontal();

		int selected = -1;
		selected = GUILayout.SelectionGrid(selected, loads, 2);
			
		// if (GUILayout.Button(visibilityMixed))
		// 	ase.LoadScenes();
		//
		// if (GUILayout.Button(hidden))
		// 	ase.LoadScenes(false);
		if (selected == 0)
			ase.LoadScenes();
		if (selected == 1)
			ase.LoadScenes(false);

		GUILayout.EndHorizontal();
	}
}
