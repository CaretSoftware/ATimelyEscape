using System;
using UnityEngine;

namespace NewRatCharacterController {
	public class NewRatAnimationController : MonoBehaviour {
		
		private static readonly int VelocityZ = Animator.StringToHash("VelocityZ");
		private static readonly int VelocityX = Animator.StringToHash("VelocityX");
		private static readonly int Jump = Animator.StringToHash("Jump");
		private static readonly int Leap = Animator.StringToHash("Leap");
		private static readonly int Grounded = Animator.StringToHash("Grounded");
		private static readonly int Pushing = Animator.StringToHash("Push");
		private static readonly int Forward = Animator.StringToHash("Forward");
		
		private Animator _animator;
		
		private float _mantleAnimationLength = .833f + .460f;

		private float _velX;
		private float _velZ;

		private Vector3 _inputVector;
		
		private void Awake() {
			_animator = GetComponent<Animator>();
		}

		private void Update() {
			_animator.SetFloat(VelocityZ, _velZ);
			_animator.SetFloat(VelocityX, _velX);
		}
		
		public void InputVector(Vector2 inputVector) => InputVector(inputVector.ToVector3());
		public void InputVector(Vector3 inputVector) {
			if (inputVector.magnitude > 1)
				inputVector.Normalize();

			_inputVector = inputVector;
			_velX = _inputVector.x;
			_velZ = _inputVector.z;
		}

		public void SetJumped() {
			_animator.SetTrigger(Jump);
		}
	}
}