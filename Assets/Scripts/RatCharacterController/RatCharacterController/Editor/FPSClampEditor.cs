using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(NewRatCameraController))]
public class FPSClampEditor : Editor {
	private NewRatCameraController _cameraController;

	private void OnEnable() {
		_cameraController = (NewRatCameraController) target;
		_minVal = 180 - _cameraController.clampLookupMin;
		_maxVal = 180 - _cameraController.clampLookupMax;
	}

	private float _minVal   = 0;
	private int _minLimit = 1;
	private float _maxVal   =  170;
	private int _maxLimit =  179;

	private float _smoothDampMaxVal = .25f;
	private float _smoothDampMinVal = 0.05f;

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		
		serializedObject.Update();
		
		LookUpClampMinMaxSlider();
		SmoothDampSmoothTimeMinMaxSlider();
		
		if (GUI.changed && !Application.IsPlaying(_cameraController))
		{
			EditorUtility.SetDirty(_cameraController);
			EditorSceneManager.MarkSceneDirty(_cameraController.gameObject.scene);
		}
	}

	private void LookUpClampMinMaxSlider() {
		EditorGUILayout.LabelField("Clamp Look Range:", String.Format("{0} - {1}",Mathf.Floor(_minVal), Mathf.Ceil(_maxVal)));
		EditorGUILayout.MinMaxSlider(ref _minVal, ref _maxVal, _minLimit, _maxLimit);

		_cameraController.clampLookupMax = 180 - Mathf.CeilToInt(_maxVal);
		_cameraController.clampLookupMin = 180 - (int)_minVal;
	}

	private void SmoothDampSmoothTimeMinMaxSlider() {
		EditorGUILayout.LabelField("SmoothDamp Min - Max: ", String.Format("{0:N2} - {1:N2}", _smoothDampMinVal, _smoothDampMaxVal));
		EditorGUILayout.MinMaxSlider(ref _smoothDampMinVal, ref _smoothDampMaxVal, float.Epsilon, 1.0f);
		
		_cameraController.smoothDampMinVal = _smoothDampMinVal;
		_cameraController.smoothDampMaxVal = _smoothDampMaxVal;
	}
}