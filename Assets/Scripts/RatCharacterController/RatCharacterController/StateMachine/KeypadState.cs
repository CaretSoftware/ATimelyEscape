using UnityEngine;

namespace NewRatCharacterController {
	public class KeypadState : BaseState {
		private const string State = nameof(PauseState);

		public override void Enter() {
			StateChange.stateUpdate?.Invoke(State);
			NewRatCharacter.EnableCharacterMovement(false);
			NewRatCharacter.AnimationController.SetKeypad(true);
		} 

		public override void Run() {
			NewRatCharacter._velocity = Vector3.zero;
			if (!NewRatCharacter.KeypadInteraction)
				stateMachine.TransitionTo<MoveState>();
		}

		public override void Exit() {
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;

			NewRatCharacter.EnableCharacterMovement(true);
			NewRatCharacter.AnimationController.SetKeypad(false);
		}
	}
}