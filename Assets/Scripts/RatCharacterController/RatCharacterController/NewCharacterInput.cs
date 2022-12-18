using System;
using CallbackSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NewRatCharacterController {
	public class NewCharacterInput : MonoBehaviour {
		private PlayerInputActions _playerInputActions;
		private NewRatCharacterController _newRatCharacterController;

		// Timetravel
		public bool CanTimeTravel { get; set; }
		private bool _canTimeTravelPast = true;
		private bool _canTimeTravelPresent = true;
		private bool _canTimeTravelFuture = true;
		public bool CantTimeTravelPast {
			get => _canTimeTravelPast;
			set => _canTimeTravelPast = value;
		}
		public bool CantTimeTravelPresent {
			get => _canTimeTravelPresent;
			set => _canTimeTravelPresent = value;

		}
		public bool CantTimeTravelFuture {
			get => _canTimeTravelFuture;
			set => _canTimeTravelFuture = value;
		}

		private void Start() {
			_newRatCharacterController = GetComponent<NewRatCharacterController>();
			
			_playerInputActions = new PlayerInputActions();
			_playerInputActions.CameraControls.Enable();
			_playerInputActions.CharacterMovement.Enable();
			_playerInputActions.Interact.Enable();
			_playerInputActions.CharacterMovement.Jump.started += Jump;
			_playerInputActions.CharacterMovement.Jump.canceled += JumpReleased;
			// _playerInputActions.Interact.Interact.performed += Interact;
			// _playerInputActions.Interact.Interact.canceled += StopInteract;
			// _playerInputActions.Interact.Past.performed += TravelToPast;
			// _playerInputActions.Interact.Present.performed += TravelToPresent;
			// _playerInputActions.Interact.Future.performed += TravelToFuture;
		}

		private void Update() {
			MovementInput(_playerInputActions.CharacterMovement.Movement.ReadValue<Vector2>());
			DeveloperCheats();
		}

		private void MovementInput(Vector2 input) => MovementInput(input.ToVector3());

		private void MovementInput(Vector3 input) =>
			_newRatCharacterController.InputVector = input;
		

		private void Jump(InputAction.CallbackContext context) {
			_newRatCharacterController.PressedJump = true;
			_newRatCharacterController.HoldingJump = true;
		}

		private void JumpReleased(InputAction.CallbackContext context) =>
			_newRatCharacterController.HoldingJump = false;

		private void OnDestroy() => Unsubscribe();
		
		private void Unsubscribe() {
			_playerInputActions.CharacterMovement.Jump.started -= Jump;
			_playerInputActions.CharacterMovement.Jump.canceled -= JumpReleased;
			// _playerInputActions.Interact.Interact.performed -= Interact;
			// _playerInputActions.Interact.Interact.canceled -= StopInteract;
			// _playerInputActions.Interact.Past.performed -= TravelToPast;
			// _playerInputActions.Interact.Present.performed -= TravelToPresent;
			// _playerInputActions.Interact.Future.performed -= TravelToFuture;
		}

		private void DeveloperCheats() {
#if UNITY_EDITOR
			// Developer code - Get Time Travel
			if (Input.GetKeyDown(KeyCode.C) && ((Input.GetKey(KeyCode.LeftControl) ||
			                                     Input.GetKey(KeyCode.RightControl) ||
			                                     Input.GetKey(KeyCode.LeftCommand)))) {
				CanTimeTravel = true;
				Debug.Log($"CanTimeTravel {CanTimeTravel}");
			}
#endif
		}
	}
}