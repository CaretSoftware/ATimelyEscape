using UnityEngine;

namespace NewRatCharacterController {
	public class PauseState : BaseState {
		private const string State = "PauseState";

		public override void Enter() {
			PauseMenuBehaviour.pauseDelegate?.Invoke(true);
			StateChange.stateUpdate?.Invoke(State);
		} 

		public override void Run() {
			if (!NewRatCharacter.paused)
				stateMachine.TransitionTo<MoveState>();
		}

		public override void Exit() {
			PauseMenuBehaviour.pauseDelegate?.Invoke(false);
		}
	}
}
