using System;
using UnityEngine;
using UnityEngine.UI;

public class MouseSensitivityListener : MonoBehaviour {
    private Slider _slider;
    private static NewRatCameraController _cameraController;

    private void Start() {
        Slider _slider = GetComponent<Slider>();
        if (_slider == null) return;
        _slider.onValueChanged.AddListener( SetMouseSensitivity );
    }

    private void OnDestroy() {
        if (_slider != null)
            _slider.onValueChanged.RemoveListener( SetMouseSensitivity );
    }

    private void SetMouseSensitivity(float value) {
        _cameraController ??= FindObjectOfType<NewRatCameraController>();

        if (_cameraController != null)
            _cameraController.MouseSensitivity = value;
    }
}