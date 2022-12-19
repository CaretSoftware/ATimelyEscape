using UnityEngine;

namespace NewRatCharacterController {
	public class CubePushState : BaseState {
		private const string State = nameof(CubePushState);

		public static bool Requirement(NewRatCharacterController newRatCharacter) {
			// are we pressing interact? are we in front of cube?
			if (newRatCharacter.Interacting && newRatCharacter.InFrontOfCube())
				return true;
			
			return false;
		}

		public override void Enter() {
			StateChange.stateUpdate?.Invoke(State);
			// parent player to cube?
			
		}

		public override void Run() {
			throw new System.NotImplementedException();
		}

		public override void Exit() {
			throw new System.NotImplementedException();
		}
	}
}