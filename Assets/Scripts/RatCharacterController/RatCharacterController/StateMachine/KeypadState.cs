using UnityEngine;

namespace NewRatCharacterController {
	public class KeypadState : BaseState {
		private const string State = nameof(PauseState);

		public override void Enter() {
			StateChange.stateUpdate?.Invoke(State);
			NewRatCharacter.AnimationController.SetKeypad(true);
		} 

		public override void Run() {
			NewRatCharacter._velocity = Vector3.zero;
			// aim camera at keypad
			if (!NewRatCharacter.KeypadInteraction)
				stateMachine.TransitionTo<MoveState>();
		}

		public override void Exit() {
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
			NewRatCharacter.AnimationController.SetKeypad(false);
		}
	}
}