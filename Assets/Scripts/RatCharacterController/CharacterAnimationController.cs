using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RatCharacterController {
	[SelectionBase]
	public class CharacterAnimationController : MonoBehaviour {

		private static readonly int VelocityZ = Animator.StringToHash("VelocityZ");
		private static readonly int VelocityX = Animator.StringToHash("VelocityX");
		private static readonly int Jump = Animator.StringToHash("Jump");
		private static readonly int Leap = Animator.StringToHash("Leap");
		private Transform _camera;
		private Animator _animator;
		private NavMeshAgent _navMeshAgent;
		private Transform _transform;
		private float _velX;
		private float _velZ;
		[SerializeField] private float smoothTime = .1f;
		private float _currentVelX;
		private float _currentVelZ;
		private Vector3 _inputVector;
		private static readonly int Grounded = Animator.StringToHash("Grounded");
		private static readonly int Pushing = Animator.StringToHash("Push");
		private Rigidbody rb;
		private float _mantleAnimationLength = .833f + .460f;

		private void Awake() {
			_transform = transform;
			_animator = GetComponent<Animator>();
			_camera = FindObjectOfType<Camera>().transform;
			rb = GetComponent<Rigidbody>();
			// _navMeshAgent = GetComponent<NavMeshAgent>();
			// _navMeshAgent.updatePosition = false;
			// _navMeshAgent.updateRotation = false;
		}

		private void Update() {

			_velX = Mathf.SmoothDamp(_velX, _inputVector.x, ref _currentVelX, smoothTime);
			_velZ = Mathf.SmoothDamp(_velZ, _inputVector.z, ref _currentVelZ, smoothTime);
			_animator.SetFloat(VelocityZ, _velZ);
			_animator.SetFloat(VelocityX, _velX);

			// _navMeshAgent.nextPosition = transform.position;
		}

		public void InputVector(Vector2 inputVector) => InputVector(inputVector.ToVector3());
		public void InputVector(Vector3 inputVector) {
			
			// bool shiftHeld = Input.GetKey(KeyCode.LeftShift);

			if (inputVector.magnitude > 1)
				inputVector.Normalize();

			// if (shiftHeld)
			// 	inputVector *= .5f;
			
			_inputVector = inputVector;
		}

		public void Push(bool push) {
			_animator.SetBool(Pushing, push);
		}

		public void JumpToFreeHang() {
			DeactivatePhysics(true);
			StartCoroutine(PhysicsDelay());
			_animator.SetTrigger(Jump);
		}

		public void LeapJump() {
			_animator.SetTrigger(Leap);
		}

		public void SetGrounded(bool grounded) {
			_animator.SetBool(Grounded, grounded);
		}

		private IEnumerator PhysicsDelay() {
			yield return new WaitForSeconds(_mantleAnimationLength);
			DeactivatePhysics(false);
		}

		private void DeactivatePhysics(bool deactivate) {
			if (!deactivate) {
				rb.Sleep();
				rb.velocity = Vector3.zero;
			}
			rb.isKinematic = deactivate;
		}
	}
}