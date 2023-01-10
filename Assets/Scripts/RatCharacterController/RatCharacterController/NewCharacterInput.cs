using System;
using CallbackSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NewRatCharacterController {
	public class NewCharacterInput : MonoBehaviour {
		
		[SerializeField] private TimeTravelButtonUIManager timeTravelButtonUIManager;

		public delegate void DPadRightPressed();
		public static DPadRightPressed dPadRightPressed;
		
		public delegate void DPadLeftPressed();
		public static DPadLeftPressed dPadLeftPressed;

		private PlayerInputActions _playerInputActions;
		private NewRatCharacterController _newRatCharacterController;
		private NewRatCameraController _camController;

		private bool _paused;

		// Time Travel
		private bool canTimeTravel = false;
		private bool canTimeTravelPast = true;
		private bool canTimeTravelPresent = true;
		private bool canTimeTravelFuture = true;
		public bool CanTimeTravel {
			get => canTimeTravel;
			
			set {
				canTimeTravel = value; 
				
				if (canTimeTravelPast)
					TimeTravelButtonUIManager.buttonActiveDelegate?.Invoke(TimeTravelPeriod.Past, canTimeTravelPast );//&& canTimeTravel);
				if (canTimeTravelPresent)
					TimeTravelButtonUIManager.buttonActiveDelegate?.Invoke(TimeTravelPeriod.Present, canTimeTravelPresent );// && canTimeTravel);
				if (canTimeTravelFuture)
					TimeTravelButtonUIManager.buttonActiveDelegate?.Invoke(TimeTravelPeriod.Future, canTimeTravelFuture );// && canTimeTravel);
			}
		}
		public bool CanTimeTravelPast {
			get => canTimeTravelPast;
			
			set
			{
				canTimeTravel = true;
				canTimeTravelPast = value;
				TimeTravelButtonUIManager.buttonActiveDelegate?.Invoke(TimeTravelPeriod.Past, value);
			}
		}
		public bool CanTimeTravelPresent {
			get => canTimeTravelPresent;
			
			set
			{
				canTimeTravel = true;
				canTimeTravelPresent = value;
				TimeTravelButtonUIManager.buttonActiveDelegate?.Invoke(TimeTravelPeriod.Present, value);
			}
		}
		public bool CanTimeTravelFuture {
			get => canTimeTravelFuture;
			
			set {
				canTimeTravel = true;
				canTimeTravelFuture = value;
				TimeTravelButtonUIManager.buttonActiveDelegate?.Invoke(TimeTravelPeriod.Future, value);
			}
		}
		

		private void Start() {
			PauseMenuBehaviour.pauseDelegate += Paused;

			_camController = GetComponent<NewRatCameraController>();
			
			_newRatCharacterController = GetComponent<NewRatCharacterController>();
			_newRatCharacterController.NewCharacterInput = this;
			
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
			dPadRightPressed?.Invoke();
		}

		private void DPadLeft(InputAction.CallbackContext context) {
			dPadLeftPressed?.Invoke();
		}

		private void Update() {
			if (_paused) return;

			/*
			if (UnityEngine.Input.GetKeyDown(KeyCode.U))
				CanTimeTravel = true;
			else if (UnityEngine.Input.GetKeyDown(KeyCode.H))
				CanTimeTravel = false;
			
			if (UnityEngine.Input.GetKeyDown(KeyCode.I))
				CanTimeTravelPast = true;
			else if (UnityEngine.Input.GetKeyDown(KeyCode.J))
				CanTimeTravelPast = false;
			
			if (UnityEngine.Input.GetKeyDown(KeyCode.O))
				CanTimeTravelPresent = true;
			else if (UnityEngine.Input.GetKeyDown(KeyCode.K))
				CanTimeTravelPresent = false;
			
			if (UnityEngine.Input.GetKeyDown(KeyCode.P))
				CanTimeTravelFuture = true;
			else if (UnityEngine.Input.GetKeyDown(KeyCode.L))
				CanTimeTravelFuture = false;
			*/

			
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
#if UNITY_EDITOR
			if (FindObjectOfType<TimeTravelManager>() == null) {
				Debug.LogWarning("No TimeTravelManager found");
				return;
			}
#endif
			TimeTravelButtonUIManager.buttonPressedDelegate?.Invoke(TimeTravelPeriod.Past, TimeTravelManager.currentPeriod, CanTimeTravel && canTimeTravelPast);
			
			if (CanTimeTravel && CanTimeTravelPast && !_paused)
				TimeTravelManager.DesiredTimePeriod(TimeTravelPeriod.Past);
		}

		private void TravelToPresent(InputAction.CallbackContext context) {
#if UNITY_EDITOR
			if (FindObjectOfType<TimeTravelManager>() == null) {
				Debug.LogWarning("No TimeTravelManager found");
				return;
			}
#endif			
			
			TimeTravelButtonUIManager.buttonPressedDelegate?.Invoke(TimeTravelPeriod.Present, TimeTravelManager.currentPeriod, CanTimeTravel && canTimeTravelPresent);

			if (CanTimeTravel && CanTimeTravelPresent && !_paused)
				TimeTravelManager.DesiredTimePeriod(TimeTravelPeriod.Present);
		}

		private void TravelToFuture(InputAction.CallbackContext context) {
#if UNITY_EDITOR			
			if (FindObjectOfType<TimeTravelManager>() == null) {
				Debug.LogWarning("No TimeTravelManager found");
				return;
			}
#endif			
			TimeTravelButtonUIManager.buttonPressedDelegate?.Invoke(TimeTravelPeriod.Future, TimeTravelManager.currentPeriod, CanTimeTravel && canTimeTravelFuture);

			if (CanTimeTravel && CanTimeTravelFuture && _paused)
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

		public void EnableCharacterMovement(bool enable) {
			if (enable)
				_playerInputActions.CharacterMovement.Enable();
			else
				_playerInputActions.CharacterMovement.Disable();
		}
		
		private void DeveloperCheats() {
#if UNITY_EDITOR
			// Developer Cheat - Get Time Travel
			if (Input.GetKeyDown(KeyCode.C) && ((Input.GetKey(KeyCode.LeftControl) ||
			                                     Input.GetKey(KeyCode.RightControl) ||
			                                     Input.GetKey(KeyCode.LeftCommand)))) {
				CanTimeTravelPast = true;
				CanTimeTravelPresent = true;
				CanTimeTravelFuture = true;
				CanTimeTravel = true;
				Debug.Log($"CanTimeTravel {CanTimeTravel}");
			}
#endif
		}
	}
}
