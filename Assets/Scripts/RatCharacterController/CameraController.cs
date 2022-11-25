using System;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour {
	public static CameraController Instance { get; private set; }
	public static Transform Cam { get; private set; }
	public float MouseSensitivity {
		get => mouseSensitivity;
		set => mouseSensitivity = Mathf.Max(0.0f, value);
	}

	[SerializeField] 
	private Transform _camera;
	private Transform _rat;
	
	private const float ClampLookupMax = 50.0f;
	private const float ClampLookupMin = 175.0f;
	private float mouseSensitivity = .7f;
	private Vector2 _mouseMovement;
	private Vector3 _cameraPos;
	

	private Vector3 _velocityLateral;
	private Vector3 _smoothDampCurrentVelocity;

	
	[Header("Camera Settings")]
	[SerializeField] 
	private LayerMask collisionMask;
	[SerializeField] 
	private Vector3 cameraOffset;
	[SerializeField] [Range(0.0f, 1.0f)] 
	private float smoothness = 0.05f;
	[SerializeField] private float smoothDampMinVal;
	[SerializeField] private float smoothDampMaxVal;
	
	private float _cameraCollisionRadius = .029f;
	private readonly Vector3 _headHeight = new Vector3(0, .16f, 0);

	private Vector2 _thumbstickDelta;

	private PauseMenuBehaviour pauseMenuBehaviour;

	private Vector3 dragTransform;
	
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
		
		_offset = cameraOffset;
		_cameraPos = transform.position;
		_camera.position = _cameraPos + cameraOffset;

		dragTransform = _camera.position;

		_rat = FindObjectOfType<RatCharacterController.CharacterInput>().transform;
		_ratRigidBody = _rat.GetComponent<Rigidbody>();

		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void LateUpdate() {
		
		if (pauseMenuBehaviour != null && pauseMenuBehaviour.isPaused()) return;

		ClampCameraTilt();
		DragCameraBehind();
	}

	
	public void MouseInput(Vector2 mouseDelta) {
		const float mouseSensitivity = 25f;
		_mouseMovement.x += mouseDelta.x * MouseSensitivity * Time.deltaTime * mouseSensitivity;
		_mouseMovement.y -= mouseDelta.y * MouseSensitivity * Time.deltaTime * mouseSensitivity;
	}

	public void StickInput(Vector2 stickDelta) {
		const float stickSensitivity = 150f;
		
		_thumbstickDelta = stickSensitivity * Time.deltaTime * new Vector2(stickDelta.x, -stickDelta.y);
		_mouseMovement += _thumbstickDelta;
	}

	private void ClampCameraTilt() {
		const float lookOffset = 90;
		const float min = ClampLookupMax - lookOffset;
		const float max = ClampLookupMin - lookOffset;
		
		_mouseMovement.y = Mathf.Clamp(_mouseMovement.y, min, max);
	}

	private Rigidbody _ratRigidBody;
	private float autoRotationSpeed = 300f;
	private Vector3 _ratLeft;
	private Vector3 _ratDampVelocity;
	private Vector3 _offset;
	private float _lastMouseMovementX;
	private void DragCameraBehind() {
		_ratLeft = 
				Vector3.SmoothDamp(
					_ratLeft, 
					-_rat.right, 
					ref _ratDampVelocity,
					.5f, 
					float.MaxValue);
		
		if (NoMouseMovement()) {
			float dot = Vector3.Dot(_camera.forward, _ratLeft);
			Vector3 velocity = _ratRigidBody.velocity;
			velocity.y = 0;
			_mouseMovement.x += dot * velocity.magnitude * autoRotationSpeed * Time.deltaTime;
		}

		_camera.rotation = Quaternion.Euler(_mouseMovement.y, _mouseMovement.x, 0.0f);

		Vector3 abovePlayer = transform.position + _headHeight;
		
		_offset = CalculateOffsetDistance();
		
		_camera.position = abovePlayer + _camera.rotation * _offset;

		_lastMouseMovementX = _mouseMovement.x;
		bool NoMouseMovement() {
			return Mathf.Abs(_lastMouseMovementX - _mouseMovement.x) <= float.Epsilon;
		}
		
		Vector3 CalculateOffsetDistance() {
			Vector3 offsetTarget = abovePlayer + _camera.rotation * cameraOffset;
			Vector3 offsetDirection = offsetTarget - abovePlayer;
			
			Physics.SphereCast(abovePlayer, 
				_cameraCollisionRadius, 
				offsetDirection.normalized, 
				out RaycastHit hit, 
				offsetDirection.magnitude, 
				collisionMask);
		
			Vector3 offset = hit.collider ? cameraOffset.normalized * hit.distance : cameraOffset;
	
			float smoothDollyTime = hit.collider ? smoothDampMinVal : smoothDampMaxVal;
			return Vector3.SmoothDamp(_offset, offset, ref _smoothDampCurrentVelocity, smoothDollyTime);
		}
	}

	private void MoveCamera() {
		_camera.rotation = Quaternion.Euler(_mouseMovement.y, _mouseMovement.x, 0.0f);

		_cameraPos = Vector3.SmoothDamp(_cameraPos, transform.position, 
				ref _velocityLateral, smoothness);
		Vector3 abovePlayer = _cameraPos + _headHeight;
		Vector3 offsetTarget = abovePlayer + _camera.rotation * cameraOffset;
		Vector3 offsetDirection = offsetTarget - abovePlayer;
		
		Physics.SphereCast(abovePlayer, 
			_cameraCollisionRadius, 
			offsetDirection.normalized, 
			out RaycastHit hit, 
			offsetDirection.magnitude, 
			collisionMask);
	
		Vector3 offset = hit.collider ? cameraOffset.normalized * hit.distance : cameraOffset;

		float smoothDollyTime = hit.collider ? smoothDampMinVal : smoothDampMaxVal;
		_offset = Vector3.SmoothDamp(_offset, offset, ref _smoothDampCurrentVelocity, smoothDollyTime);
		
		_camera.position = abovePlayer + _camera.rotation * _offset;
	}
}
