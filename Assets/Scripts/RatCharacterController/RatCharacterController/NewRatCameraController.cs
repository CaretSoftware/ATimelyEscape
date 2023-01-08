using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class NewRatCameraController : MonoBehaviour {
	private static NewRatCameraController _instance;
	private NewRatCharacterController.NewRatCharacterController _ratCharacterController;
	
	private RaycastHit _hit;
	
	private Vector3 _smoothDampCurrentVelocity;
	private Vector3 _smoothDampCurrentVelocityLateral;
	private Vector3 _offset;
	private Vector3 _abovePlayer;
	private Vector3 _offsetTarget;
	private Vector3 _offsetDirection;
	private Vector3 _lerpOffset;
	private Vector3 _cameraPos;
	
	private Vector2 _mouseMovement;
	private Vector2 _thumbstickDelta;
	
	private float _rotationX;
	private float _rotationY;
	private float _smoothDollyTime;

	[SerializeField] 
	private float lookAtInterpolationTimeDefault = 1.5f;
	private float _lookAtInterpolationTime;
	private float _lookAtInterpolation;
	private Transform _lookAtTransform;
	
	private bool _paused;
	
	// [HideInInspector]
	public float ClampLookupMax = 50f;
	[HideInInspector]
	public float ClampLookupMin = 175f;
	[HideInInspector]
	public float smoothDampMinVal;
	[HideInInspector]
	public float smoothDampMaxVal;
	
	[SerializeField] 
	private LayerMask _collisionMask;
	
	[SerializeField]
	private Transform _camera;
	
	[SerializeField] [Range(0.0f, 2.0f)]
	private float _cameraCollisionRadius;

	[SerializeField] [Range(0f, 2.0f)] 
	private float minCollisionZoomDistance;

	[SerializeField] [Range(0.0f, 2.0f)]
	private float _headHeight = 1.6f;

	[SerializeField] 
	private Vector3 _camera3rdPersonOffset;

	[SerializeField] [Range(0.0f, 1.0f)] 
	private float _smoothCameraPosTime = 0.05f;

	public float MouseSensitivity { get; set; } = .2f;

	public bool Accessibility { get; set; } = false;

	private void Awake() {
		if (_instance != null && _instance != this) 
			Destroy(this.gameObject);
		_instance ??= this;
		_cameraPos = transform.position;
	}

	
	private void Start() {
		_ratCharacterController = FindObjectOfType<NewRatCharacterController.NewRatCharacterController>();
		_camera.parent = null; // TODO fix?

		SetMouseMovementVectorFromRotation(transform.rotation.eulerAngles);
		
		PauseMenuBehaviour.pauseDelegate += Pause;
	}

	private void OnDestroy() {
		PauseMenuBehaviour.pauseDelegate -= Pause;
	}

	private void Pause(bool paused) => _paused = paused;

	
	private void LateUpdate()
	{
		// keypadInterpolation = !KeypadTransform ? 0f : keypadInterpolation;
		if (_lookAtTransform) {
			MoveCameraToViewPoint();
			return;
		}

		if (_paused || _ratCharacterController.KeypadInteraction) return;

		ClampCameraTilt();
		if (!Accessibility)
			MoveCamera();
		else
			MoveAccessibleCamera();
	}

	public void MouseInput(Vector2 mouseDelta) {
		const float mouseSensitivity = 25f;
		if (_paused) return;
		
		_mouseMovement.x += mouseDelta.x * MouseSensitivity * Time.deltaTime * mouseSensitivity;
		_mouseMovement.y -= mouseDelta.y * MouseSensitivity * Time.deltaTime * mouseSensitivity;
	}

	public void StickInput(Vector2 stickDelta) {
		const float stickSensitivity = 150f;
		if (_paused) return;
		
		_thumbstickDelta = stickSensitivity * Time.deltaTime * new Vector2(stickDelta.x, -stickDelta.y);
		_mouseMovement += _thumbstickDelta;
	}
	
	private void ClampCameraTilt() {
		const float lookOffset = 90;
		float min = ClampLookupMax - lookOffset;
		float max = ClampLookupMin - lookOffset;
		
		_mouseMovement.y = Mathf.Clamp(_mouseMovement.y, min, max);
	}
	
	private void MoveCamera() {
		
		_camera.rotation = Quaternion.Euler(_mouseMovement.y, _mouseMovement.x, 0.0f);

		_cameraPos = Vector3.SmoothDamp(_cameraPos, transform.position, ref _smoothDampCurrentVelocityLateral, _smoothCameraPosTime);
		
		_abovePlayer = _cameraPos + Vector3.up * _headHeight;
		_offsetTarget = _abovePlayer + _camera.rotation * _camera3rdPersonOffset;
		_offsetDirection = _offsetTarget - _abovePlayer;
		
		Physics.SphereCast(_abovePlayer, 
			_cameraCollisionRadius, 
			_offsetDirection.normalized, 
			out _hit, 
			_offsetDirection.magnitude, 
			_collisionMask,
			QueryTriggerInteraction.Ignore);
		
		if (_hit.collider)
			_offset = _camera3rdPersonOffset.normalized * Mathf.Max(minCollisionZoomDistance, _hit.distance);
		else
			_offset = _camera3rdPersonOffset;

		_smoothDollyTime = _hit.collider ? smoothDampMinVal : smoothDampMaxVal;
		_lerpOffset = Vector3.SmoothDamp(_lerpOffset, _offset, ref _smoothDampCurrentVelocity, _smoothDollyTime);

		_camera.position = _abovePlayer + _camera.rotation * _lerpOffset;
	}

	// private Vector3 debug;
	// private void OnDrawGizmos()
	// {
	// 	if (!Application.isPlaying) return;
	//
	// 	Color c = Gizmos.color;
	// 	Gizmos.color = Color.cyan;
	// 	Gizmos.DrawSphere(debug, .01f);
	// 	Gizmos.color = c;
	// }

	[SerializeField] private Vector3 keyPadRotation;
	[SerializeField] private Vector3 keypadOffset;
	[SerializeField] private float keypadCenterOffset;

	private void MoveCameraToViewPoint() {
		if (_lookAtInterpolation > 1f) {
			SetMouseMovementVectorFromRotation(_camera.rotation.eulerAngles);
			return;
		} 
		
		Vector3 lookAtPosition = _lookAtTransform.position + Vector3.up * keypadCenterOffset;
		
		Vector3 targetPos =  lookAtPosition + _lookAtTransform.TransformDirection(keypadOffset);
		Vector3 keypadLookDirection = (lookAtPosition - targetPos).normalized;

		Vector3 lerpPos = Vector3.Lerp(_interpolationStartPosition, targetPos, Ease.EaseInOutCubic(_lookAtInterpolation));
		
		Vector3 cameraForwardLerp = Vector3.Lerp(_interpolationStartForward, keypadLookDirection, Ease.EaseInOutCubic(_lookAtInterpolation));
		Quaternion targetRot = Quaternion.LookRotation(cameraForwardLerp, Vector3.up);
		
		_camera.SetPositionAndRotation(lerpPos, targetRot);
		
		SetMouseMovementVectorFromRotation(_camera.rotation.eulerAngles);
		
		_lookAtInterpolation += Time.deltaTime * 
		                        (1f / (lookAtInterpolationTimeDefault <= 0f 
			                        ? Mathf.Epsilon 
			                        : lookAtInterpolationTimeDefault));
	}
	
	private void MoveAccessibleCamera()
	{
		float distance = _camera3rdPersonOffset.magnitude;
		Vector3 abovePlayer = transform.position + Vector3.up * _headHeight;
		Vector3 direction = (_camera.position - abovePlayer).normalized;
		_offsetTarget = abovePlayer + direction * distance;

		_camera.position = _offsetTarget;
		_camera.LookAt(abovePlayer, Vector3.up);
	}

	private void SetMouseMovementVectorFromRotation(Vector3 rotationEulerAngles) {
		_mouseMovement = new Vector2(rotationEulerAngles.y, rotationEulerAngles.x);
	}

	private Vector3 _interpolationStartPosition;
	private Vector3 _interpolationStartForward;

	public static void SetLookTarget(Transform lookAtTransform) => 
		SetLookTarget(lookAtTransform, _instance.keypadOffset);

	public static void SetLookTarget(Transform lookAtTransform, Vector3 lookOffsetDirection) => 
		SetLookTarget(lookAtTransform, _instance.keypadOffset, _instance.lookAtInterpolationTimeDefault);
	
	public static void SetLookTarget(Transform lookAtTransform, Vector3 lookOffsetDirection, float transitionDuration) {
		_instance._lookAtTransform = lookAtTransform;
		_instance._interpolationStartPosition = _instance._camera.position;
		_instance._interpolationStartForward = _instance._camera.forward;
		_instance._lookAtInterpolation = 0f;
	}

	public static void SetLookTarget(Transform lookAtTransform, Vector3 lookOffsetDirection, float transitionDuration,
		float returnAfter) {
		SetLookTarget(lookAtTransform, lookOffsetDirection, transitionDuration);
		_instance.Invoke(nameof(UnlockLookTargetInstance), transitionDuration + returnAfter);
	}

	public static void UnlockLookTarget() => _instance._lookAtTransform = null;
	public void UnlockLookTargetInstance() => _instance._lookAtTransform = null;
}
