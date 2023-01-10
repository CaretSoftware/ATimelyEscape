using System;
using CallbackSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NewRatCharacterController {
	public class NewCharacterInput : MonoBehaviour {
		
		[SerializeField] private TimeTravelButtonUIManager timeTravelButtonUIManager;

		public delegate void AdvanceDialogueDelegate();
		public static AdvanceDialogueDelegate advanceDialogueDelegate;
		
		public delegate void ReturnToGameDelegate();
		public static ReturnToGameDelegate returnToGameDelegate;

		private PlayerInputActions _playerInputActions;
		private NewRatCharacterController _newRatCharacterController;
		private NewRatCameraController _camController;

		private bool _paused;

		// Time Travel
		private bool canTimeTravel = false;
		private bool canTimeTravelPast = false;
		private bool canTimeTravelPresent = false;
		private bool canTimeTravelFuture = false;
		public bool CanTimeTravel {
			get => canTimeTravel;
			
			set {
				canTimeTravel = value;
			
				if (canTimeTravel) {
					if (canTimeTravelPast)
						TimeTravelButtonUIManager.buttonActiveDelegate?.Invoke(TimeTravelPeriod.Past, canTimeTravelPast);
					if (canTimeTravelPresent)
						TimeTravelButtonUIManager.buttonActiveDelegate?.Invoke(TimeTravelPeriod.Present, canTimeTravelPresent);
					if (canTimeTravelFuture)
						TimeTravelButtonUIManager.buttonActiveDelegate?.Invoke(TimeTravelPeriod.Future, canTimeTravelFuture);
				}
			}
		}
		public bool CanTimeTravelPast {
			get => canTimeTravelPast;
			
			set {
				canTimeTravelPast = value;
				if (canTimeTravel)
					TimeTravelButtonUIManager.buttonActiveDelegate?.Invoke(TimeTravelPeriod.Past, value);
			}
		}
		public bool CanTimeTravelPresent {
			get => canTimeTravelPresent;
			
			set {
				canTimeTravelPresent = value;
				if (canTimeTravel)
					TimeTravelButtonUIManager.buttonActiveDelegate?.Invoke(TimeTravelPeriod.Present, value);
			}
		}
		public bool CanTimeTravelFuture {
			get => canTimeTravelFuture;
			
			set {
				canTimeTravelFuture = value;
				if (canTimeTravel)
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
			_playerInputActions.LevelSelect.Enable();
            _playerInputActions.LevelSelect.EnableMenu.performed += EnableLevelSelectMenu;
            _playerInputActions.Onboarding.ReturnToGame.started += ReturnToGame;
            _playerInputActions.Onboarding.AdvanceDialogue.started += AdvanceDialogue;
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

        private void EnableLevelSelectMenu(InputAction.CallbackContext context){
            LevelSelect.Instance.EnableMenu();
        }

		private void AdvanceDialogue(InputAction.CallbackContext context) {
			advanceDialogueDelegate?.Invoke();
		}

		private void ReturnToGame(InputAction.CallbackContext context) {
			returnToGameDelegate?.Invoke();
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

            _playerInputActions.Onboarding.ReturnToGame.started -= ReturnToGame;
            _playerInputActions.Onboarding.AdvanceDialogue.started -= AdvanceDialogue;
            _playerInputActions.CharacterMovement.Jump.started -= Jump;
            _playerInputActions.CharacterMovement.Jump.canceled -= JumpReleased;
            _playerInputActions.Pause.Pause.performed -= Pause;
            _playerInputActions.Interact.Interact.performed -= Interact;
            _playerInputActions.Interact.Interact.canceled -= StopInteract;
            _playerInputActions.Interact.Past.performed -= TravelToPast;
            _playerInputActions.Interact.Present.performed -= TravelToPresent;
            _playerInputActions.Interact.Future.performed -= TravelToFuture;
            _playerInputActions.LevelSelect.EnableMenu.performed -= EnableLevelSelectMenu;
		}

		public void EnableCharacterMovement(bool enable) {
			if (enable)
				_playerInputActions.CharacterMovement.Enable();
			else
				_playerInputActions.CharacterMovement.Disable();
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
