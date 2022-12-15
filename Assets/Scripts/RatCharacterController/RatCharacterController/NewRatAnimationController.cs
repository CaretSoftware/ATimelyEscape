using System;
using UnityEngine;

namespace NewRatCharacterController {
	public class NewRatAnimationController : MonoBehaviour {
		
		private static readonly int VelocityZ = Animator.StringToHash("VelocityZ");
		private static readonly int VelocityX = Animator.StringToHash("VelocityX");
		private static readonly int VelocityForward = Animator.StringToHash("VelocityForward");
		private static readonly int VelocityLateral = Animator.StringToHash("VelocityLateral");
		private static readonly int Jumped = Animator.StringToHash("Jump");
		private static readonly int Leap = Animator.StringToHash("Leap");
		private static readonly int Grounded = Animator.StringToHash("Grounded");
		private static readonly int Pushing = Animator.StringToHash("Push");
		private static readonly int Forward = Animator.StringToHash("Forward");
		private static readonly int Falling = Animator.StringToHash("Falling");

		private Vector2 _blendVector;
		
		private Animator _animator;
		
		private float _mantleAnimationLength = .833f + .460f;

		private Vector2 smoothTime;

		private Vector3 _vector;

		private Vector3 _inputVector;

		[SerializeField] private Transform rat;

		// Smoothing
		private Vector3 _currentVelocity;
		private float _smoothTime = .15f;
		private Vector3 _lookDirection;

		private Quaternion _rotation;

		[SerializeField] private float minEpsilon = .1f;
		[SerializeField] private float rotationVelocity = 1.0f;
		[SerializeField] private float maxMagnitudeDelta = 0;

		[SerializeField] private Transform _cameraTransform;

		// private Vector3 
		
		private void Awake() {
			_animator = GetComponent<Animator>();
			// int numHookAnimations = 5;
			// for (int i = 0; i < numHookAnimations; ++i) {
			// 	_animator[i].Play("mySuperCoolHookAnimation", 0, 1.0f / numHookAnimations * i);
			// }
		}

		private void Update() {
			_animator.SetFloat(VelocityZ, _vector.y);
			_animator.SetFloat(VelocityX, _vector.x);

			// if (_vector.sqrMagnitude > minEpsilon)
			// 	_lookDirection = Vector3.SmoothDamp(_lookDirection, _vector.normalized.ProjectOnPlane(), ref _currentVelocity, _smoothTime);

			float singleStep = rotationVelocity * Time.deltaTime;

			_lookDirection = Vector3.RotateTowards(rat.forward, _vector.ProjectOnPlane(), singleStep, maxMagnitudeDelta);
			
			if (_lookDirection != Vector3.zero)
				rat.rotation = Quaternion.LookRotation(_lookDirection);

			_blendVector = BlendVector(_inputVector);
			
			_animator.SetFloat(VelocityForward, _blendVector.y);
			_animator.SetFloat(VelocityLateral, _blendVector.x);
			// rat.LookAt(rat.position + _lookDirection, Vector3.up);
		}

		private Vector2 BlendVector(Vector3 input) {

			// float dotFwd = Vector3.Dot();


			Vector3 projectedInput = InputToCameraProjection(input);
			
			Vector3 transformInputDir = rat.InverseTransformDirection(projectedInput);

			return transformInputDir.ToVector2(); // TODO 
			//
			// return transformInputDir.ToVector2();
		}
		
		private Vector3 InputToCameraProjection(Vector3 input) {
			if (_cameraTransform == null)
				return input;

			Vector3 cameraRotation = _cameraTransform.transform.rotation.eulerAngles;

			input = Quaternion.Euler(cameraRotation) * input;

			float magnitude = input.magnitude;

			return Vector3.ProjectOnPlane(input, Vector3.up).normalized * magnitude;
		}

		//public void Vector(Vector2 vector) => Vector(vector.ToVector3());
		public void Vector(Vector3 vector) {
			if (vector.magnitude > 1)
				vector.Normalize();
		
			_vector = vector;
		}

		public void SetInputVector(Vector3 input) {
			_inputVector = input;
		}

		public void Jump() {
			_animator.SetTrigger(Jumped);
		}

		public void Fall() {
			_animator.SetTrigger(Falling);
		}

		public void SetGrounded(bool grounded) {
			_animator.SetBool(Grounded, grounded);
		}
	}
}