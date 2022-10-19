using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour {
	private Transform _camera;
	private Vector2 _mouseMovement;
	private Vector3 _cameraPos;
	
	private float _clampLookupMax = 50.0f;
	private float _clampLookupMin = 175.0f;
	public float smoothDampMinVal;
	public float smoothDampMaxVal;

	private Vector3 _smoothDampCurrentVelocityLateral;
	private Vector3 _smoothDampCurrentVelocity;
		
	[Header("Mouse Sensitivity")]
	[SerializeField] [Range(0.001f, 10.0f)]
	private float mouseSensitivityX = 1.0f;
	[SerializeField, Range(0.001f, 10.0f)]
	private float mouseSensitivityY = 1.0f;
	
	[Header("Camera Settings")]
	[SerializeField, Range(0.0f, 2.0f)]
	private float _cameraCollisionRadius = .4f;
	[SerializeField, Range(0.0f, 2.0f)]
	private float _headHeight = 1.6f;
	[SerializeField]
	private bool firstPerson;
	[SerializeField] 
	private LayerMask collisionMask;
	[SerializeField] 
	private Vector3 cameraOffset;
	[SerializeField] [Range(0.0f, 1.0f)] 
	private float smoothness = 0.05f;

	private Vector2 _thumbstickDelta;
	
	private void Start() {
		_camera = GetComponentInChildren<Camera>().transform;
		
		// init smoothDamp variables and set camera position
		_lerpOffset = cameraOffset;
		_cameraPos = transform.position;
		_camera.position = _cameraPos + cameraOffset;
		
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void Update() {
		MoveCamera();
	}

	public void MouseInput(InputAction.CallbackContext context) {
		Vector2 mouseDelta = context.ReadValue<Vector2>();

		_mouseMovement.x += mouseDelta.x * mouseSensitivityX;
		_mouseMovement.y -= mouseDelta.y * mouseSensitivityY;
	}

	public void StickInput(InputAction.CallbackContext context) {
		const float stickSensitivity = 1.0f;
		Vector2 stickDelta = context.ReadValue<Vector2>();
		
		_thumbstickDelta = new Vector2(stickDelta.x, -stickDelta.y) * stickSensitivity;
		
		// _mouseMovement.x += stickDelta.x * stickSensitivity;
		// _mouseMovement.y -= stickDelta.y * stickSensitivity;
	}

	private Vector3 _lerpOffset;
	private void MoveCamera() {
		const float lookOffset = 90;

		_mouseMovement += _thumbstickDelta;
		_mouseMovement.y = Mathf.Clamp(_mouseMovement.y, _clampLookupMax - lookOffset, _clampLookupMin - lookOffset);
		
		_camera.rotation = Quaternion.Euler(_mouseMovement.y, _mouseMovement.x, 0.0f);
	
		if (firstPerson) {
			_camera.localPosition = transform.position + _headHeight * Vector3.up;
			return;
		}
	
		_cameraPos = Vector3.SmoothDamp(_cameraPos, transform.position, 
				ref _smoothDampCurrentVelocityLateral, smoothness);
		Vector3 abovePlayer = _cameraPos + Vector3.up * _headHeight;
		Vector3 offsetTarget = abovePlayer + _camera.rotation * cameraOffset;
		Vector3 offsetDirection = offsetTarget - abovePlayer;
		
		Physics.SphereCast(abovePlayer, 
			_cameraCollisionRadius, 
			offsetDirection.normalized, 
			out RaycastHit hit, 
			offsetDirection.magnitude, 
			collisionMask);
	
		Vector3 offset;
		if (hit.collider)
			offset = cameraOffset.normalized * hit.distance;
		else
			offset = cameraOffset;

		float smoothDollyTime = hit.collider ? smoothDampMinVal : smoothDampMaxVal;
		_lerpOffset = Vector3.SmoothDamp(_lerpOffset, offset, ref _smoothDampCurrentVelocity, smoothDollyTime);
		
		_camera.position = abovePlayer + _camera.rotation * _lerpOffset;
	}
}
	