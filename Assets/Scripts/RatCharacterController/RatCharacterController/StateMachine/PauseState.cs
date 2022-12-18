using UnityEngine;

namespace NewRatCharacterController {
	public class PauseState : BaseState {
		private const string State = "PauseState";

		public override void Enter() {
			StateChange.stateUpdate?.Invoke(State);
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		} 

		public override void Run() {
			// if (!NewRatCharacter.Paused)
			// 	stateMachine.TransitionToLastState();
		}

		public override void Exit() {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}
}
