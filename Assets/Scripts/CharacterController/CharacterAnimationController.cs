using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CharacterController { 
	public class CharacterAnimationController : MonoBehaviour {

		private static readonly int VelocityZ = Animator.StringToHash("VelocityZ");
		private static readonly int VelocityX = Animator.StringToHash("VelocityX");
		private Animator _animator;
		private NavMeshAgent _navMeshAgent;
		private Transform _transform;
		private float _velX;
		private float _velZ;
		[SerializeField] 
		private float smoothTime = .1f;
		private float _currentVelX;
		private float _currentVelZ;
		
		private void Awake() {
			_transform = transform;
			_animator = GetComponent<Animator>();
			// _navMeshAgent = GetComponent<NavMeshAgent>();
			// _navMeshAgent.updatePosition = false;
			// _navMeshAgent.updateRotation = false;
		}
		
		private void Update() {
			Vector3 inputVector = InputVector();

			_velX = Mathf.SmoothDamp(_velX, inputVector.x, ref _currentVelX, smoothTime);
			_velZ = Mathf.SmoothDamp(_velZ, inputVector.z, ref _currentVelZ, smoothTime);
			_animator.SetFloat(VelocityZ, _velZ);
			_animator.SetFloat(VelocityX, _velX);

			// _navMeshAgent.nextPosition = transform.position;
		}

		private Vector3 InputVector() {
			bool shiftHeld = Input.GetKey(KeyCode.LeftShift);
			float horizontalInput = Input.GetAxisRaw("Horizontal");
			float verticalInput = Input.GetAxisRaw("Vertical");
			
			Vector3 inputVector = new Vector3(horizontalInput, 0.0f, verticalInput);

			if (inputVector.magnitude > 1)
				inputVector.Normalize();

			if (shiftHeld)
				inputVector *= .5f;

			return inputVector;
		}
	}
}