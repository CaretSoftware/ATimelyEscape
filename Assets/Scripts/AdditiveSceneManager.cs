using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

[DisallowMultipleComponent]
public class AdditiveSceneManager : MonoBehaviour {
	[SerializeField, HideInInspector] private SceneSetup[] _sceneSetups;

	public void SaveScenes() {
		_sceneSetups = EditorSceneManager.GetSceneManagerSetup();
		Debug.Log($"{_sceneSetups.Length} additive scene" +
		          $"{(_sceneSetups.Length > 1 ? "s" : string.Empty)} saved");
		EditorUtility.SetDirty(this);
	}

	public void LoadScenes(bool loaded = true) {

		if (_sceneSetups == null || _sceneSetups.Length <= 0) {
			Debug.LogWarning("No additive scene setup saved");
			return;
		}

		int openScenes = _sceneSetups.Length;
		int scenesLoaded = 0;
		int missingFiles = 0;
		for (int scene = 0; scene < openScenes; ++scene) {
			string path = _sceneSetups[scene].path;
			bool missingFile = !File.Exists(path);
			OpenSceneMode openSceneMode;

			if (missingFile)
				++missingFiles; 
			else
				++scenesLoaded;

			if (loaded)
				openSceneMode = _sceneSetups[scene].isLoaded 
										? OpenSceneMode.Additive 
										: OpenSceneMode.AdditiveWithoutLoading;
			else
				openSceneMode = OpenSceneMode.AdditiveWithoutLoading;
			
			if (!missingFile)
				EditorSceneManager.OpenScene(path, openSceneMode);
		}

		string s = string.Empty;
		if (missingFiles > 0) {
			s = missingFiles > 1 ? "s" : string.Empty;
			Debug.LogWarning(
					$"{missingFiles} scene{s} could not be loaded. " +
					$"Scene{s} moved, renamed or deleted since last save. " +
					$"Add missing scene{s} and save again!");
		}
		
		s = scenesLoaded > 1 ? "s" : string.Empty;
		Debug.Log($"{scenesLoaded} additive scene{s} loaded");
	}
}
