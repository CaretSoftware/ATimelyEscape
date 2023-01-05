using UnityEngine;

namespace NewRatCharacterController {
	public class PauseState : BaseState {
		private const string State = nameof(PauseState);


		public override void Enter() {
			StateChange.stateUpdate?.Invoke(State);
			NewRatCharacter.NewCharacterInput.CanTimeTravel = false;
			PauseMenuBehaviour.pauseDelegate?.Invoke(true);
		} 

		public override void Run() {
			if (!NewRatCharacter.paused)
				stateMachine.TransitionTo<MoveState>();
		}

		public override void Exit() {
			PauseMenuBehaviour.pauseDelegate?.Invoke(false);
			NewRatCharacter.NewCharacterInput.CanTimeTravel = true;
		}
	}
}
