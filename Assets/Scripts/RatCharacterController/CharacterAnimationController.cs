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
		private Transform _camera;
		private Animator _animator;
		private NavMeshAgent _navMeshAgent;
		private Transform _transform;
		private float _velX;
		private float _velZ;
		[SerializeField] 
		private float smoothTime = .1f;
		private float _currentVelX;
		private float _currentVelZ;
		private Vector3 _inputVector;
		private static readonly int Grounded = Animator.StringToHash("Grounded");
		private static readonly int Pushing = Animator.StringToHash("Push");

		private void Awake() {
			_transform = transform;
			_animator = GetComponent<Animator>();
			_camera = FindObjectOfType<Camera>().transform;
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

		public void InputVector(Vector2 inputVector) {

			// bool shiftHeld = Input.GetKey(KeyCode.LeftShift);
			// float horizontalInput = Input.GetAxisRaw("Horizontal");
			// float verticalInput = Input.GetAxisRaw("Vertical");
			
			// Vector3 inputVector = new Vector3(horizontalInput, 0.0f, verticalInput);

			if (inputVector.magnitude > 1)
				inputVector.Normalize();

			// if (shiftHeld)
			// 	inputVector *= .5f;

			_inputVector = new Vector3(inputVector.x, 0.0f, inputVector.y);
		}

		public void Push(bool push) {
			_animator.SetBool(Pushing, push);
		}

		public void JumpToFreeHang() {
			_animator.SetTrigger(Jump);
		}

		public void SetGrounded(bool grounded) {
			_animator.SetBool(Grounded, grounded);
		}
	}
}