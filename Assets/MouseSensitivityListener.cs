using UnityEngine;
using UnityEngine.UI;

public class MouseSensitivityListener : MonoBehaviour {
    private Slider _slider;
    private static CameraController _cameraController;

    private void Start() {
        _slider = GetComponent<Slider>();
        if (_slider == null) return;
        _slider.onValueChanged.AddListener( SetMouseSensitivity );
    }

    private void SetMouseSensitivity(float value) {
        _cameraController ??= FindObjectOfType<CameraController>();

        if (_cameraController != null)
            _cameraController.MouseSensitivity = value;
    }
}