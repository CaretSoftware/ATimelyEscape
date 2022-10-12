using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour {
	private Transform _camera;
	private Vector2 _mouseMovement;
	private Vector3 _cameraPos;
	
	private float _clampLookupMax = 20.0f;
	private float _clampLookupMin = 175.0f;
	public float smoothDampMinVal;
	public float smoothDampMaxVal;

	private Vector3 _smoothDampCurrentVelocityLateral;
	private Vector3 _smoothDampCurrentVelocity;
		
	[Header("Mouse Sensitivity")]
	[SerializeField] [Range(0.1f, 10.0f)]
	private float mouseSensitivityX = 1.0f;
	[SerializeField, Range(0.1f, 10.0f)]
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

		Cursor.visible = false;
		
		// Releases the cursor
		// Cursor.lockState = CursorLockMode.None;
		// Locks the cursor
		Cursor.lockState = CursorLockMode.Locked;
		// Confines the cursor
		// Cursor.lockState = CursorLockMode.Confined;
		
	}

	private void Update() {
		MoveCamera();
	}
	
	public void MouseInput(InputAction.CallbackContext context) {
		const float lookOffset = 90;
		Vector2 mouseDelta = context.ReadValue<Vector2>();

		_mouseMovement.x += mouseDelta.x * mouseSensitivityX;
		_mouseMovement.y -= mouseDelta.y * mouseSensitivityY; 
		
		_mouseMovement.y = Mathf.Clamp(_mouseMovement.y, _clampLookupMax - lookOffset, _clampLookupMin - lookOffset);
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

		_mouseMovement += _thumbstickDelta;
		
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
	
		Vector3 offset = Vector3.zero;
		if (hit.collider)
			offset = cameraOffset.normalized * hit.distance;
		else
			offset = cameraOffset;
		
		// _debugHit = hit.collider;
	
		float smoothDollyTime = hit.collider ? smoothDampMinVal : smoothDampMaxVal;
		_lerpOffset = Vector3.SmoothDamp(_lerpOffset, offset, ref _smoothDampCurrentVelocity, smoothDollyTime);
		_camera.position = //cameraTarget + 
			abovePlayer + _camera.rotation * _lerpOffset;
	}
}
	