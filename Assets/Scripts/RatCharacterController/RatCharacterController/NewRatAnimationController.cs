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
		private static readonly int Pushing = Animator.StringToHash("Pushing");
		private static readonly int Forward = Animator.StringToHash("Forward");
		private static readonly int Falling = Animator.StringToHash("Falling");
		private static readonly int LedgeJump = Animator.StringToHash("JumpToLedge");
		private static readonly int Caught = Animator.StringToHash("Caught");
		private static readonly int Keypad = Animator.StringToHash("Keypad");

		[SerializeField] Animator anim;  

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

			_animator = anim;
		}

		private void Update() {
			// RotateCharacterMesh();

			_blendVector = BlendVector(_inputVector);
			
			_animator.SetFloat(VelocityZ, _vector.y);
			_animator.SetFloat(VelocityX, _vector.x);
			_animator.SetFloat(VelocityForward, _blendVector.y);
			_animator.SetFloat(VelocityLateral, _blendVector.x);
		}

		public void RotateCharacterMesh() {
			float singleStep = rotationVelocity * Time.deltaTime;

			_lookDirection = Vector3.RotateTowards(rat.forward, _vector.ProjectOnPlane(), 
				singleStep, maxMagnitudeDelta);
			
			if (_lookDirection != Vector3.zero)
				rat.rotation = Quaternion.LookRotation(_lookDirection);
		}

		private Vector2 BlendVector(Vector3 input) {
			Vector3 projectedInput = InputToCameraProjection(input);
			
			Vector3 transformInputDir = rat.InverseTransformDirection(projectedInput);

			return transformInputDir.ToVector2();
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

		public void SetInputVector(Vector3 input) => _inputVector = input;

		public void Jump() => _animator.SetTrigger(Jumped);

		public void Fall() => _animator.SetTrigger(Falling);

		public void Push(bool pushing) => _animator.SetBool(Pushing, pushing);

		public void SetGrounded(bool grounded) {
			if (_animator != null) {
				_animator.SetBool(Grounded, grounded);
			} else {
				_animator = GetComponent<Animator>();
			}
		}
		public void SetLedgeJump() => _animator.SetTrigger(LedgeJump);

		public void SetCaught(bool caught) => _animator.SetBool(Caught, caught);

		public void SetKeypad(bool keypad) => _animator.SetBool(Keypad, keypad);
	}
}