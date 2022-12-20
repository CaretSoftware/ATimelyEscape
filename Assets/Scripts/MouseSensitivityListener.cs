using UnityEngine;
using UnityEngine.UI;

public class MouseSensitivityListener : MonoBehaviour {
    private Slider _slider;
    private static NewRatCameraController _cameraController;

    private void Start() {
        _slider = GetComponent<Slider>();
        if (_slider == null) return;
        _cameraController ??= FindObjectOfType<NewRatCameraController>();
        if (_cameraController != null)
            _slider.value = _cameraController.MouseSensitivity;
        _slider.onValueChanged.AddListener( SetMouseSensitivity );
    }

    private void SetMouseSensitivity(float value) {
        _cameraController ??= FindObjectOfType<NewRatCameraController>();

        if (_cameraController != null)
            _cameraController.MouseSensitivity = value;
        else
            Debug.Log($"No ${nameof(NewRatCameraController)} listening to slider", this);
    }
}