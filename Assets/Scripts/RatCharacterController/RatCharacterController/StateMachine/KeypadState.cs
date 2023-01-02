using UnityEngine;

namespace NewRatCharacterController {
	public class KeypadState : BaseState {
		private const string State = nameof(PauseState);


		public override void Enter() {
			StateChange.stateUpdate?.Invoke(State);
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.Confined;
		}

		public override void Run() {
			if (!NewRatCharacter.KeypadInteraction)
				stateMachine.TransitionTo<MoveState>();
		}

		public override void Exit() {
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}
	}
}