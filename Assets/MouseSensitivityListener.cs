using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseSensitivityListener : MonoBehaviour {
    [SerializeField] private Slider slider;

    private void Start() {
        // slider.onValueChanged.AddListener(delegate { SetMouseSensitivity(); } );
        slider = GetComponent<Slider>();
        if (slider != null)
            slider.onValueChanged.AddListener( SetMouseSensitivity );
    }

    private static CameraController _cameraController;
    
    private void SetMouseSensitivity(float value) {
        _cameraController ??= FindObjectOfType<CameraController>();

        if (_cameraController != null)
            _cameraController.MouseSensitivity = value;

        Debug.Log($"value {value}");
    }
}