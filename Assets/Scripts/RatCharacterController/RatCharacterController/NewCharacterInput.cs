using System;
using System.Collections;
using CallbackSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NewRatCharacterController {
    public class NewCharacterInput : MonoBehaviour
    {
        public delegate void ZoomDelegate(bool zoom);

        public static ZoomDelegate zoomDelegate;

        public delegate void ExitZoomDelegate(bool zoom);

        public static ExitZoomDelegate exitZoomDelegate;

        public delegate void SelectPressed();

        public static SelectPressed selectPressed;


        [SerializeField] private TimeTravelButtonUIManager timeTravelButtonUIManager;

        public delegate void AdvanceDialogueDelegate();

        public static AdvanceDialogueDelegate advanceDialogueDelegate;

        public delegate void ReturnToGameDelegate();

        public static ReturnToGameDelegate returnToGameDelegate;

        private PlayerInputActions _playerInputActions;
        private NewRatCharacterController _newRatCharacterController;
        private NewRatCameraController _camController;

        private bool _paused;

        public static bool Accessibility { get; set; } = true;
        private const int Stop = 0;
        private const int Run = 1;
        private const int StopAgain = 2;
        private const int TurnCamera = 3;
        
        private int AccessibleState = Stop;

        
        // Time Travel
        private bool canTimeTravel = false;
        private bool canTimeTravelPast = false;
        private bool canTimeTravelPresent = false;
        private bool canTimeTravelFuture = false;

        public bool CanTimeTravel
        {
            get { return canTimeTravel; }

            set
            {
                canTimeTravel = value;

                if (canTimeTravelPast && canTimeTravel)
                    TimeTravelButtonUIManager.buttonActiveDelegate?.Invoke(TimeTravelPeriod.Past,
                        canTimeTravelPast); //&& canTimeTravel);
                if (canTimeTravelPresent && canTimeTravel)
                    TimeTravelButtonUIManager.buttonActiveDelegate?.Invoke(TimeTravelPeriod.Present,
                        canTimeTravelPresent); // && canTimeTravel);
                if (canTimeTravelFuture && canTimeTravel)
                    TimeTravelButtonUIManager.buttonActiveDelegate?.Invoke(TimeTravelPeriod.Future,
                        canTimeTravelFuture); // && canTimeTravel);
            }
        }

        public bool CanTimeTravelPast
        {
            get => canTimeTravelPast;

            set
            {
                //canTimeTravel = true;
                canTimeTravelPast = value;
                TimeTravelButtonUIManager.buttonActiveDelegate?.Invoke(TimeTravelPeriod.Past, value);
            }
        }

        public bool CanTimeTravelPresent
        {
            get => canTimeTravelPresent;

            set
            {
                //canTimeTravel = true;
                canTimeTravelPresent = value;
                TimeTravelButtonUIManager.buttonActiveDelegate?.Invoke(TimeTravelPeriod.Present, value);
            }
        }

        public bool CanTimeTravelFuture
        {
            get => canTimeTravelFuture;

            set
            {
                //canTimeTravel = true;
                canTimeTravelFuture = value;
                TimeTravelButtonUIManager.buttonActiveDelegate?.Invoke(TimeTravelPeriod.Future, value);
            }
        }

        private void Start()
        {
            PauseMenuBehaviour.pauseDelegate += Paused;

            _camController = GetComponent<NewRatCameraController>();

            _newRatCharacterController = GetComponent<NewRatCharacterController>();
            _newRatCharacterController.NewCharacterInput = this;

            _playerInputActions = new PlayerInputActions();

            Subscribe();
        }

        private void Subscribe() {
            _playerInputActions.CharacterMovement.Disable();

            ToggleAccessibilityButtons();
            _playerInputActions.CameraControls.Enable();
            _playerInputActions.Pause.Enable();
            _playerInputActions.Onboarding.Enable();
            _playerInputActions.LevelSelect.Enable();
            _playerInputActions.Zoom.Enable();

            _playerInputActions.LevelSelect.EnableMenu.performed += EnableLevelSelectMenu;
            _playerInputActions.LevelSelect.LoadRoom.performed += LoadSelectedRoom;
            
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
            
            _playerInputActions.Zoom.ExitZoom.performed += ExitZoom;
            _playerInputActions.Zoom.ExitZoom.canceled += ExitUnZoom;
            
            _playerInputActions.Zoom.FirstPerson.performed += FirstPersonZoom;
            _playerInputActions.Zoom.FirstPerson.canceled += FirstPersonUnZoom;
            
            _playerInputActions.AccessibilityControls.AButton.performed += AButton;
            _playerInputActions.AccessibilityControls.BButton.performed += BButton;
            _playerInputActions.AccessibilityControls.XButton.performed += XButton;
            _playerInputActions.AccessibilityControls.YButton.performed += YButton;

        }

        private void ToggleAccessibilityButtons() {
            if (true) // TODO debug prod
            {
                _playerInputActions.AccessibilityControls.Enable();
                _playerInputActions.CharacterMovement.Disable();
                _playerInputActions.Interact.Disable();
            }
            else
            {
                Debug.Log("you shouldn't be here");
                _playerInputActions.AccessibilityControls.Disable();
                _playerInputActions.CharacterMovement.Enable();
                _playerInputActions.Interact.Enable();
            }

            Debug.Log($"char mov enabled: {_playerInputActions.CharacterMovement.enabled}");
        }

        private void BButton(InputAction.CallbackContext context) {
            AccessibleState = ++AccessibleState % 4;
            Debug.Log($"{nameof(AButton)} state: {AccessibleState}");

            switch (AccessibleState)
            {
                case (Stop):
                    Debug.Log(nameof(Stop));
                    _camController.TurnCamera(false);
                    _newRatCharacterController.RunForward(false);
                    break;
                case (Run):
                    Debug.Log(nameof(Run));
                    _camController.TurnCamera(false);
                    _newRatCharacterController.RunForward(true);
                    break;
                case (StopAgain):
                    Debug.Log(nameof(StopAgain));
                    _camController.TurnCamera(false);
                    _newRatCharacterController.RunForward(false);
                    break;
                case (TurnCamera):
                    Debug.Log(nameof(TurnCamera));
                    _camController.TurnCamera(true);
                    break;
            }
        }

        private const int Past = 0;
        private const int Present = 1;
        private const int Future = 2;
        private bool xButton;
        private void XButton(InputAction.CallbackContext context) {
            xButton = !xButton;
            if (canTimeTravelPast || canTimeTravelPresent || canTimeTravelFuture) {
                Debug.Log($"{nameof(BButton)} {xButton}");
                StartCoroutine(CycleTimeTravelButtons());
            }
        }

        private IEnumerator CycleTimeTravelButtons() {
            int timeTravelTime = -1;

            while (xButton) {
                timeTravelTime = ++timeTravelTime % 3;
                
                CycleButton(timeTravelTime);
                yield return new WaitForSeconds(1f);
            }

            if (timeTravelTime == Past)
                TravelToPast();
            if (timeTravelTime == Present)
                TravelToPresent();
            if (timeTravelTime == Future)
                TravelToFuture();
            
            TimeTravelUIButton.pulseButtonEvent?.Invoke(TimeTravelPeriod.Past, false);
            TimeTravelUIButton.pulseButtonEvent?.Invoke(TimeTravelPeriod.Present, false);
            TimeTravelUIButton.pulseButtonEvent?.Invoke(TimeTravelPeriod.Future, false);

            xButton = false;
        }

        private void CycleButton(int timeTravelTime) {
            switch (timeTravelTime) {
                case (Past):
                    Debug.Log(nameof(Past));
                    TimeTravelUIButton.pulseButtonEvent?.Invoke(TimeTravelPeriod.Future, false);
                    TimeTravelUIButton.pulseButtonEvent?.Invoke(TimeTravelPeriod.Past, true);
                    break;
                case (Present):
                    Debug.Log(nameof(Present));
                    TimeTravelUIButton.pulseButtonEvent?.Invoke(TimeTravelPeriod.Past, false);
                    TimeTravelUIButton.pulseButtonEvent?.Invoke(TimeTravelPeriod.Present, true);
                    break;
                case (Future):
                    Debug.Log(nameof(Future));
                    TimeTravelUIButton.pulseButtonEvent?.Invoke(TimeTravelPeriod.Present, false);
                    TimeTravelUIButton.pulseButtonEvent?.Invoke(TimeTravelPeriod.Future, true);
                    break;
            }
        }
        
        private void AButton(InputAction.CallbackContext context) {
            Debug.Log($"{nameof(AButton)}");
            Jump();
        }

        private bool _interactAccessible;
        private void YButton(InputAction.CallbackContext context) {
            Debug.Log($"{nameof(YButton)}");
            _interactAccessible = !_interactAccessible;
            
            if (_interactAccessible)
                Interact();
            else
                StopInteract();
        }

        private void ExitZoom(InputAction.CallbackContext context) {
            exitZoomDelegate?.Invoke(true);
        }

        private void ExitUnZoom(InputAction.CallbackContext context) {
            exitZoomDelegate?.Invoke(false);
        }

        private void FirstPersonZoom(InputAction.CallbackContext context) {
            zoomDelegate?.Invoke(true);
        }

        private void FirstPersonUnZoom(InputAction.CallbackContext context) {
            zoomDelegate?.Invoke(false);
        }

        private void Paused(bool paused) => _paused = paused;

        private void EnableLevelSelectMenu(InputAction.CallbackContext context) {
            LevelSelect.Instance.EnableMenu();
        }

        private void LoadSelectedRoom(InputAction.CallbackContext context) {
            LevelSelect.Instance.TriggerRoomLoad();
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

        private void Jump(InputAction.CallbackContext context) => Jump();
        private void Jump() {
            Debug.Log("Pressed jump");
            _newRatCharacterController.PressedJump = true;
            _newRatCharacterController.HoldingJump = true;
        }

        private void JumpReleased(InputAction.CallbackContext context) =>
            _newRatCharacterController.HoldingJump = false;

        private void Pause(InputAction.CallbackContext context) {
            _newRatCharacterController.paused = !_newRatCharacterController.paused;
        }

        private void Interact(InputAction.CallbackContext context) => Interact();
        private void Interact() {
            _newRatCharacterController.Interact();
        }

        private void StopInteract(InputAction.CallbackContext context) => StopInteract();
        private void StopInteract() {
            _newRatCharacterController.StopInteract();
        }

        private void TravelToPast(InputAction.CallbackContext context) => TravelToPast();
        private void TravelToPast() {
#if UNITY_EDITOR
            if (FindObjectOfType<TimeTravelManager>() == null) {
                Debug.LogWarning("No TimeTravelManager found");
                return;
            }
#endif
            TimeTravelButtonUIManager.buttonPressedDelegate?.Invoke(TimeTravelPeriod.Past,
                TimeTravelManager.currentPeriod, CanTimeTravel && canTimeTravelPast);

            if (CanTimeTravel && CanTimeTravelPast && !_paused)
                TimeTravelManager.DesiredTimePeriod(TimeTravelPeriod.Past);
        }

        private void TravelToPresent(InputAction.CallbackContext context) => TravelToPresent();
        private void TravelToPresent() {
#if UNITY_EDITOR
            if (FindObjectOfType<TimeTravelManager>() == null) {
                Debug.LogWarning("No TimeTravelManager found");
                return;
            }
#endif

            TimeTravelButtonUIManager.buttonPressedDelegate?.Invoke(TimeTravelPeriod.Present,
                TimeTravelManager.currentPeriod, CanTimeTravel && canTimeTravelPresent);

            if (CanTimeTravel && CanTimeTravelPresent && !_paused)
                TimeTravelManager.DesiredTimePeriod(TimeTravelPeriod.Present);
        }

        
        private void TravelToFuture(InputAction.CallbackContext context) => TravelToFuture();
        private void TravelToFuture() {
#if UNITY_EDITOR
            if (FindObjectOfType<TimeTravelManager>() == null) {
                Debug.LogWarning("No TimeTravelManager found");
                return;
            }
#endif
            TimeTravelButtonUIManager.buttonPressedDelegate?.Invoke(TimeTravelPeriod.Future,
                TimeTravelManager.currentPeriod, CanTimeTravel && canTimeTravelFuture);

            if (CanTimeTravel && CanTimeTravelFuture && !_paused)
                TimeTravelManager.DesiredTimePeriod(TimeTravelPeriod.Future);
        }
        
        public void EnableCharacterMovement(bool enable) {
            Debug.LogWarning("fix this :) SettingsMeny is setting Accessibility to false");
            if (true) return;   // TODO debug
            
            if (enable) {
                Debug.LogError("h");
                _playerInputActions.CharacterMovement.Enable();
            }
            else
                _playerInputActions.CharacterMovement.Disable();
        }

        private void DeveloperCheats() {
            //#if UNITY_EDITOR
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
            //#endif
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
            _playerInputActions.LevelSelect.LoadRoom.performed -= LoadSelectedRoom;
            _playerInputActions.Zoom.ExitZoom.performed -= ExitZoom;
            _playerInputActions.Zoom.ExitZoom.canceled -= ExitUnZoom;
            _playerInputActions.Zoom.FirstPerson.performed -= FirstPersonZoom;
            _playerInputActions.Zoom.FirstPerson.canceled -= FirstPersonUnZoom;
        }
    }
}
