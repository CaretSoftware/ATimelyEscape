using UnityEngine;

namespace NewRatCharacterController {
	public class PauseState : BaseState {
		private const string State = "PauseState";
		
		public override void Enter() => StateChange.stateUpdate?.Invoke(State);

		public override void Run() {
			// if (!NewRatCharacter.Paused)
			// 	stateMachine.TransitionToLastState();
		}

		public override void Exit() { }
	}
}
