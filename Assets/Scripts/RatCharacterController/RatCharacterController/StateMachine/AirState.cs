using UnityEngine;

namespace NewRatCharacterController {
	public class AirState : BaseState {

		private const float AntiFloatForce = 25.0f;

		private const string State = "AirState";
		public override void Enter() {
			
			StateChange.stateUpdate?.Invoke(State);
		}

		public override void Run() {

			NewRatCharacter.AirControl();

			AddGravityForce();

			NewRatCharacter.ApplyAirFriction();

			if (NewRatCharacter.Grounded)
				stateMachine.TransitionTo<MoveState>();

			if (NewRatCharacter.Jumped) // coyote time jump
				stateMachine.TransitionTo<JumpState>();

			if (WallRunState.Requirement(NewRatCharacter))
				stateMachine.TransitionTo<WallRunState>();
		}

		private void AddGravityForce() {

			float gravityMovement =
				-NewRatCharacter._defaultGravity * NewRatCharacter._fallGravityMultiplier * Time.deltaTime;
			
			CounteractFloat();
			
			NewRatCharacter._velocity.y += gravityMovement;

			void CounteractFloat() {
				if (NewRatCharacter._velocity.y > 0)
					gravityMovement -= AntiFloatForce * Time.deltaTime;
			}
		}

		public override void Exit() {
			lastState = this;
		}
	}
}