using System;
using CallbackSystem;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NewRatCharacterController {
	public class NewCharacterInput : MonoBehaviour {
		public delegate void DPadRightPressed();
		public static DPadRightPressed dPadRightPressed;
		
		public delegate void DPadLeftPressed();
		public static DPadLeftPressed dPadLeftPressed;

		private PlayerInputActions _playerInputActions;
		private NewRatCharacterController _newRatCharacterController;
		private NewRatCameraController _camController;

		private bool _paused;

		// Time Travel
		public bool CanTimeTravel { get; set; }
		public bool CanTimeTravelPast { get; set; } = true;
		public bool CanTimeTravelPresent { get; set; } = true;
		public bool CanTimeTravelFuture { get; set; } = true;
		

		private void Start() {
			PauseMenuBehaviour.pauseDelegate += Paused;

			_camController = GetComponent<NewRatCameraController>();
			
			_newRatCharacterController = GetComponent<NewRatCharacterController>();
			
			_playerInputActions = new PlayerInputActions();
			_playerInputActions.CameraControls.Enable();
			_playerInputActions.CharacterMovement.Enable();
			_playerInputActions.Interact.Enable();
			_playerInputActions.Pause.Enable();
			_playerInputActions.Onboarding.Enable();
			_playerInputActions.Onboarding.DLeft.started += DPadLeft;
			_playerInputActions.Onboarding.DRight.started += DPadRight;
			_playerInputActions.CharacterMovement.Jump.started += Jump;
			_playerInputActions.CharacterMovement.Jump.canceled += JumpReleased;
			_playerInputActions.Pause.Pause.performed += Pause;
			_playerInputActions.Interact.Interact.performed += Interact;
			_playerInputActions.Interact.Interact.canceled += StopInteract;
			_playerInputActions.Interact.Past.performed += TravelToPast;
			_playerInputActions.Interact.Present.performed += TravelToPresent;
			_playerInputActions.Interact.Future.performed += TravelToFuture;
		}

		private void Paused(bool paused) => _paused = paused;

		private void DPadRight(InputAction.CallbackContext context) {
			//Debug.Log(nameof(NewCharacterInput.DPadRight));
			dPadRightPressed?.Invoke();
		}

		private void DPadLeft(InputAction.CallbackContext context) {
			//Debug.Log(nameof(NewCharacterInput.DPadLeft));
			dPadLeftPressed?.Invoke();
		}

		private void Update() {
			if (_paused) return;

			MovementInput(_playerInputActions.CharacterMovement.Movement.ReadValue<Vector2>());
			CameraInput();
			DeveloperCheats();
		}

		private void MovementInput(Vector2 input) => MovementInput(input.ToVector3());

		private void MovementInput(Vector3 input) =>
			_newRatCharacterController.InputVector = input;
		
		private void CameraInput() {
			Vector2 cameraStickInput = _playerInputActions.CameraControls.CameraThumbstick.ReadValue<Vector2>();
			Vector2 cameraMouseInput = _playerInputActions.CameraControls.CameraMouseInput.ReadValue<Vector2>();

			_camController.StickInput(cameraStickInput);
			_camController.MouseInput(cameraMouseInput);
		}

		private void Jump(InputAction.CallbackContext context) {
			_newRatCharacterController.PressedJump = true;
			_newRatCharacterController.HoldingJump = true;
		}

		private void JumpReleased(InputAction.CallbackContext context) =>
			_newRatCharacterController.HoldingJump = false;

		private void Pause(InputAction.CallbackContext context) {
			_newRatCharacterController.paused = !_newRatCharacterController.paused;
		}

		private void Interact(InputAction.CallbackContext context) {
			_newRatCharacterController.Interact();
		}
		
		private void StopInteract(InputAction.CallbackContext context) {
			_newRatCharacterController.StopInteract();
		}

		private void TravelToPast(InputAction.CallbackContext context) {
			if (FindObjectOfType<TimeTravelManager>() == null) {
				Debug.LogWarning("No TimeTravelManager found");
				return;
			}
			if (CanTimeTravel && CanTimeTravelPast)
				TimeTravelManager.DesiredTimePeriod(TimeTravelPeriod.Past);
		}

		private void TravelToPresent(InputAction.CallbackContext context) {
			if (FindObjectOfType<TimeTravelManager>() == null) {
				Debug.LogWarning("No TimeTravelManager found");
				return;
			}
			if (CanTimeTravel && CanTimeTravelPresent) // TODO use the booleans!
				TimeTravelManager.DesiredTimePeriod(TimeTravelPeriod.Present);
		}

		private void TravelToFuture(InputAction.CallbackContext context) {
			if (FindObjectOfType<TimeTravelManager>() == null) {
				Debug.LogWarning("No TimeTravelManager found");
				return;
			}
			if (CanTimeTravel && CanTimeTravelFuture)
				TimeTravelManager.DesiredTimePeriod(TimeTravelPeriod.Future);
		}

		private void OnDestroy() => Unsubscribe();
		
		private void Unsubscribe() {
			PauseMenuBehaviour.pauseDelegate -= Paused;
			
			_playerInputActions.Onboarding.DLeft.started -= DPadLeft;
			_playerInputActions.Onboarding.DRight.started -= DPadRight;
			_playerInputActions.CharacterMovement.Jump.started -= Jump;
			_playerInputActions.CharacterMovement.Jump.canceled -= JumpReleased;
			_playerInputActions.Pause.Pause.performed -= Pause;
			_playerInputActions.Interact.Interact.performed -= Interact;
			_playerInputActions.Interact.Interact.canceled -= StopInteract;
			_playerInputActions.Interact.Past.performed -= TravelToPast;
			_playerInputActions.Interact.Present.performed -= TravelToPresent;
			_playerInputActions.Interact.Future.performed -= TravelToFuture;
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