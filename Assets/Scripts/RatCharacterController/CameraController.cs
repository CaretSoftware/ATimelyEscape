using System;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class CameraController : MonoBehaviour {
	public static CameraController Instance { get; private set; }
	public static Transform Cam { get; private set; }
	public float MouseSensitivity {
		get => mouseSensitivity;
		set => mouseSensitivity = Mathf.Max(0.0f, value);
	}

	[SerializeField] private Transform _camera;
	private float mouseSensitivity = .7f;
	
	private Vector2 _mouseMovement;
	private Vector3 _cameraPos;
	
	private float _clampLookupMax = 50.0f;
	private float _clampLookupMin = 175.0f;
	public float smoothDampMinVal;
	public float smoothDampMaxVal;

	private Vector3 _smoothDampCurrentVelocityLateral;
	private Vector3 _smoothDampCurrentVelocity;
		
	
	[Header("Camera Settings")]
	[SerializeField, Range(0.0f, 2.0f)]
	private float _headHeight = .16f;
	[SerializeField]
	private bool firstPerson;
	[SerializeField] 
	private LayerMask collisionMask;
	[SerializeField] 
	private Vector3 cameraOffset;
	[SerializeField] [Range(0.0f, 1.0f)] 
	private float smoothness = 0.05f;
	private float _cameraCollisionRadius = .029f;

	private Vector2 _thumbstickDelta;

	private PauseMenuBehaviour pauseMenuBehaviour;

	private Transform dragTransform;
	
	private void Awake() {
		// if (Instance == null) 
		Instance ??= this;
		
		if (_camera == null)
			_camera = Camera.main.transform;

		Cam = _camera;
	}
	
	private void Start() {
		pauseMenuBehaviour = FindObjectOfType<PauseMenuBehaviour>();
		CameraFollow cameraFollow = FindObjectOfType<CameraFollow>();
		
		Vector3 initialCameraVector = cameraFollow.Follow.rotation.eulerAngles;
		_mouseMovement = new Vector2(initialCameraVector.y, initialCameraVector.x);
		
		_lerpOffset = cameraOffset;
		_cameraPos = transform.position;
		_camera.position = _cameraPos + cameraOffset;

		dragTransform = new GameObject("DragTransform").transform;
		dragTransform.position = _camera.position;
		// dragTransform = Instantiate(new GameObject("Drag Transform"), _camera.position, _camera.rotation).transform;

		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void LateUpdate() {
		
		if (pauseMenuBehaviour != null && pauseMenuBehaviour.isPaused()) return;
		
		DragCameraBehind();
		// MoveCamera();
	}

	
	public void MouseInput(Vector2 mouseDelta) {
		const float mouseSensitivity = 25f;
		_mouseMovement.x += mouseDelta.x * MouseSensitivity * Time.deltaTime * mouseSensitivity;
		_mouseMovement.y -= mouseDelta.y * MouseSensitivity * Time.deltaTime * mouseSensitivity;
		
		// TODO testing to see if we can use mouseDelta instead
		_mouseDelta.x = mouseDelta.x * MouseSensitivity * Time.deltaTime * mouseSensitivity;
		_mouseDelta.y = mouseDelta.y * MouseSensitivity * Time.deltaTime * mouseSensitivity;
	}

	public void StickInput(Vector2 stickDelta) {
		const float stickSensitivity = 150f;
		
		_thumbstickDelta = stickSensitivity * Time.deltaTime * new Vector2(stickDelta.x, -stickDelta.y);
	}

	private Vector3 _lerpOffset;
	private Vector2 prevInput;

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

	[SerializeField] private Transform _followTransform;
	private Vector3 _cameraDirection;
	private Vector3 _cameraHeight;
	private Vector2 _mouseDelta;
	private void DragCameraBehind() {
		float distance = .3f;
		Vector3 headHeight = Vector3.up * _headHeight;

		DragCameraPosition();
		
		void DragCameraPosition() {
			
			_cameraDirection = (dragTransform.position - _followTransform.position);
			_cameraDirection = _cameraDirection.ProjectOnPlane();
			_cameraDirection.Normalize();
		}

		RotateCameraPosition();
		
		void RotateCameraPosition() {
			const float lookOffset = 90;

			_cameraDirection = Quaternion.Euler(0, _mouseDelta.x, 0.0f) * _cameraDirection;
			// _cameraHeight = Quaternion.Euler(_mouseDelta.y, 0.0f, 0.0f) * _cameraHeight;
			// Debug.Log(_cameraHeight);
			// Quaternion.Euler(_mouseDelta.y, 0.0f, 0.0f).eulerAngles;
			//
			// _cameraHeight.y = Mathf.Clamp(_cameraHeight.y, _clampLookupMax - lookOffset, _clampLookupMin - lookOffset);

			// Reset mouse delta
			_mouseDelta = Vector2.zero;
		}

		dragTransform.position = _followTransform.position + headHeight + _cameraDirection * distance;
		
		
		_camera.position = dragTransform.position;
		_camera.LookAt(_followTransform);


		// Vector3 initialCameraVector = (_camera.position - transform.position).normalized;
		// cameraFollow.Follow.rotation.eulerAngles;

		// _mouseMovement = new Vector2(cameraDirection.y, cameraDirection.x);

	}
}
	