using System;
using UnityEngine;

namespace NewRatCharacterController {
	public class AnimationController : MonoBehaviour {
		private static readonly int VelocityZ = Animator.StringToHash("VelocityZ");
		private static readonly int VelocityX = Animator.StringToHash("VelocityX");
		private static readonly int Jump = Animator.StringToHash("Jump");
		private static readonly int Leap = Animator.StringToHash("Leap");
		private static readonly int Grounded = Animator.StringToHash("Grounded");
		private static readonly int Pushing = Animator.StringToHash("Push");
		private static readonly int Forward = Animator.StringToHash("Forward");
		
		private Animator _animator;
		private Vector3 _inputVector;
		private float _velX;
		private float _velZ;
		private float _currentVelX;
		private float _currentVelZ;
		[SerializeField] private float smoothTimeX = .3f;
		[SerializeField] private float smoothTimeZ = .1f;
		
		private void Awake() {
			_animator = GetComponent<Animator>();
		}

		private void Update() {
			_velX = Mathf.SmoothDamp(_velX, _inputVector.x, ref _currentVelX, smoothTimeX);
			_velZ = Mathf.SmoothDamp(_velZ, _inputVector.z, ref _currentVelZ, smoothTimeZ);
			_animator.SetFloat(VelocityZ, _velZ);
			_animator.SetFloat(VelocityX, _velX);
		}

		public void InputVector(Vector2 inputVector) => InputVector(inputVector.ToVector3());
		public void InputVector(Vector3 inputVector) {

			if (inputVector.magnitude > 1)
				inputVector.Normalize();

			_inputVector = inputVector;
		}
	}
}