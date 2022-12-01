using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AdditiveSceneManager))]
public class AdditiveSceneEditor : Editor {
	public override void OnInspectorGUI() {
		AdditiveSceneManager ase = (AdditiveSceneManager) target;
		
		if (GUILayout.Button("Save"))
			ase.SaveScenes();
		
		GUILayout.BeginHorizontal();
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Load"))
			ase.LoadScenes();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Load Unloaded"))
			ase.LoadScenes(false);
		GUILayout.EndHorizontal();
		GUILayout.EndHorizontal();
	}
}
