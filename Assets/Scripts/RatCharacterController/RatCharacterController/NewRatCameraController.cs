using System;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class NewRatCameraController : MonoBehaviour {
	
	private const string MouseX = "Mouse X";
	private const string MouseY = "Mouse Y";
	private const float LookOffset = 90;
	
	private RaycastHit _hit;
	private Vector3 _fpsCameraPos;
	private Vector3 _smoothDampCurrentVelocity;
	private Vector3 _smoothDampCurrentVelocityLateral;
	private Vector3 _offset;
	private Vector3 _abovePlayer;
	private Vector3 _offsetTarget;
	private Vector3 _offsetDirection;
	private Vector3 _lerpOffset;
	private Vector3 _cameraPos;
	private Vector2 _mouseMovement;
	private float _rotationX;
	private float _rotationY;
	private float _smoothDollyTime;
	private Camera _cam;
	
	[HideInInspector]
	public float clampLookupMax;
	[HideInInspector]
	public float clampLookupMin;
	[HideInInspector]
	public float smoothDampMinVal;
	[HideInInspector]
	public float smoothDampMaxVal;
	
	[SerializeField]
	private bool _firstPerson;

	[SerializeField] 
	private LayerMask _collisionMask;
	
	[SerializeField]
	private Transform _camera;
	
	[SerializeField] [Range(1.0f, 10.0f)]
	private float mouseSensitivityX = 1.0f;
	
	[SerializeField] [Range(1.0f, 10.0f)]
	private float mouseSensitivityY = 1.0f;
		
	[SerializeField] [Range(0.0f, 2.0f)]
	private float _cameraCollisionRadius;

	[SerializeField] [Range(0.0f, 2.0f)]
	private float _headHeight = 1.6f;

	[SerializeField] 
	private Vector3 _camera3rdPersonOffset;

	[SerializeField] [Range(0.0f, 1.0f)] 
	private float _smoothCameraPosTime = 0.05f;
	
	[SerializeField] private Transform _cameraGimble;

	private void Awake() {
		_fpsCameraPos = _camera.localPosition;
		_cameraPos = transform.position;
		_cam = Camera.main;
	}

	private void Update() {
		Input();
	}

	private void LateUpdate() {
		MoveCamera();
	}

	private void Input() {
		_mouseMovement.x += UnityEngine.Input.GetAxisRaw(MouseX) * mouseSensitivityX;
		_mouseMovement.y -= UnityEngine.Input.GetAxisRaw(MouseY) * mouseSensitivityY;
		_mouseMovement.y = Mathf.Clamp(_mouseMovement.y, clampLookupMax - LookOffset, clampLookupMin - LookOffset);
	}

	private bool _debugHit;
	private void MoveCamera() {
		_camera.rotation = Quaternion.Euler(_mouseMovement.y, _mouseMovement.x, 0.0f);

		_cam.cullingMask = _firstPerson ? ~(1 << 1) : -1;
		
		if (_firstPerson) {
			_camera.localPosition = _cameraGimble.localPosition;
			return;
		}

		_cameraPos = Vector3.SmoothDamp(_cameraPos, transform.position, ref _smoothDampCurrentVelocityLateral, _smoothCameraPosTime);
		
		_abovePlayer = _cameraPos + Vector3.up * _headHeight;
		_offsetTarget = _abovePlayer + _camera.rotation * _camera3rdPersonOffset;
		_offsetDirection = _offsetTarget - _abovePlayer;

		Physics.SphereCast(_abovePlayer, 
			_cameraCollisionRadius, 
			_offsetDirection.normalized, 
			out _hit, 
			_offsetDirection.magnitude, 
			_collisionMask);
		
		if (_hit.collider)
			_offset = _camera3rdPersonOffset.normalized * _hit.distance;
		else
			_offset = _camera3rdPersonOffset;

		_debugHit = _hit.collider;
		
		_smoothDollyTime = _hit.collider ? smoothDampMinVal : smoothDampMaxVal;
		_lerpOffset = Vector3.SmoothDamp(_lerpOffset, _offset, ref _smoothDampCurrentVelocity, _smoothDollyTime);

		_camera.position = _abovePlayer + _camera.rotation * _lerpOffset;
	}

	// private void OnDrawGizmos() {
	// 	
	// 	Gizmos.color = _debugHit ? Color.white : Color.black;
	// 	
	// 	Gizmos.DrawWireSphere(_camera.position, _cameraCollisionRadius);
	//
	// 	Gizmos.color = Color.white;
	//
	// 	Gizmos.matrix = Matrix4x4.TRS( 
	// 		_camera.position,
	// 		_camera.rotation, 
	// 		new Vector3(1.0f, 1.0f, 1.0f) );
	//
	// 	Gizmos.DrawFrustum(
	// 		Vector3.zero,
	// 		Camera.main.fieldOfView, 
	// 		12.0f, 
	// 		.3f, 
	// 		Camera.main.aspect);
	// }
}
