using UnityEngine;

namespace NewRatCharacterController {
	public class WallRunState : BaseState {
		private const float AntiFloatForce = 25.0f;
		private static float wallRunMagnitudeThreshold = 0.04f;
		private Vector3 _wallNormal;
		private const string State = nameof(WallRunState);

		public static bool Requirement(NewRatCharacterController newRatCharacter) {

			if (newRatCharacter.Grounded || Vector3.Dot(newRatCharacter._velocity, newRatCharacter.transform.forward) < 0.0f)
				return false;

			Vector3 verticalVelocity = Vector3.ProjectOnPlane(newRatCharacter._velocity, Vector3.up);

			RaycastHit rightHit = RayCast(newRatCharacter, newRatCharacter.transform.right);
			if (rightHit.collider) {
				Vector3 wallProjectionVector = Vector3.ProjectOnPlane(verticalVelocity, rightHit.normal);
				return wallProjectionVector.magnitude > wallRunMagnitudeThreshold;
			}

			RaycastHit leftHit = RayCast(newRatCharacter, -newRatCharacter.transform.right);
			if (leftHit.collider) {
				Vector3 wallProjectionVector = Vector3.ProjectOnPlane(verticalVelocity, leftHit.normal);
				return wallProjectionVector.magnitude > wallRunMagnitudeThreshold;
			}

			return false;
		}

		public override void Enter() {
			StateChange.stateUpdate?.Invoke(State);
			Vector3 transformRight = NewRatCharacter.transform.right;
			RaycastHit rightHit = RayCast(NewRatCharacter, transformRight);
			RaycastHit leftHit = RayCast(NewRatCharacter, -transformRight);
			
			if (rightHit.collider)
				_wallNormal = rightHit.normal;
			else if (leftHit.collider)
				_wallNormal = leftHit.normal;
		}

		public override void Run() {
			// Move character along vector of wall surface normal
			// project characters _velocity on wall normal, multiply with initial wall running speed
			// redirect characters _velocity into run? add upwards velocity?
			// decrease speed with time

			// Player.AirControl();

			AddGravityForce();

			// Player.ApplyAirFriction();

			// if (Player.pressedJump && Player._velocity.y > threshold) 
			// 	stateMachine.TransitionTo<JumpState>();

			//if (NewRatCharacter.PressedJump)
			//	stateMachine.TransitionTo<WallJumpState>();

			if (!RayCast(NewRatCharacter, NewRatCharacter.transform.right).collider && !RayCast(NewRatCharacter, -NewRatCharacter.transform.right).collider)
				stateMachine.TransitionTo<AirState>();

			if (NewRatCharacter.Grounded)
				stateMachine.TransitionTo<MoveState>();
			
			if (NewRatCharacter.Caught)
				stateMachine.TransitionTo<CaughtState>();
		}

		private void AddGravityForce() {

			Vector3 gravityMovement = NewRatCharacter._defaultGravity * Time.deltaTime * Vector3.down;
			if (NewRatCharacter._velocity.y > 0.0f)
				gravityMovement *= .75f;
			else {
				NewRatCharacter._velocity += NewRatCharacter._velocity.y * -_wallNormal * Time.deltaTime;
			}

			//CounteractFloat();
			NewRatCharacter._velocity += gravityMovement;
		}

		private static RaycastHit RayCast(NewRatCharacterController newRatCharacter, Vector3 direction) {
			Ray ray = new Ray(newRatCharacter._point2Transform.position/*Player.transform.position + Player._point2*/, direction);
			Physics.Raycast(ray, out var hit, newRatCharacter._colliderRadius + .5f, newRatCharacter._collisionMask);
			return hit;
		}

		public override void Exit() { }
	}
}