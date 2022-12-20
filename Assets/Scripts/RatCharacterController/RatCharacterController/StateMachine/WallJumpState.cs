using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NewRatCharacterController {
	public class WallJumpState : BaseState {
		private bool _falling = false;

		private const string State = "WallJumpState";
		public override void Enter() {
			StateChange.stateUpdate?.Invoke(State);
			
			_falling = false;
			Vector3 verticalVelocity = Vector3.ProjectOnPlane(NewRatCharacter._velocity, Vector3.up);

			RaycastHit rightHit = RayCast(NewRatCharacter, NewRatCharacter.transform.right);
			RaycastHit leftHit = RayCast(NewRatCharacter, -NewRatCharacter.transform.right);

			Vector3 redirectedVelocity = NewRatCharacter._velocity;
			Vector3 wallProjectionVector;
			if (rightHit.collider) {
				wallProjectionVector = Vector3.ProjectOnPlane(verticalVelocity, rightHit.normal);
				redirectedVelocity = RedirectVelocity(wallProjectionVector, rightHit.normal);
			}
			else if (leftHit.collider) {
				wallProjectionVector = Vector3.ProjectOnPlane(verticalVelocity, leftHit.normal);
				redirectedVelocity = RedirectVelocity(wallProjectionVector, leftHit.normal);
			}

			redirectedVelocity += redirectedVelocity.normalized * 1.0f;
			redirectedVelocity.y = NewRatCharacter._jumpForce;;
			NewRatCharacter._velocity = redirectedVelocity;
			
			NewRatCharacter.AnimationController.Jump(); // TODO change to Wall Run Kickoff Animation
		}

		private Vector3 RedirectVelocity(Vector3 velocity, Vector3 normal) {

			Vector3 direction = velocity.normalized;
			float magnitude = velocity.magnitude;

			return magnitude * Vector3.Slerp(direction, normal, .5f);
		}

		public override void Run() {
            
			NewRatCharacter.AirControl();

			float gravityMovement = -NewRatCharacter._defaultGravity * 2.0f * Time.deltaTime;
			NewRatCharacter._velocity.y += gravityMovement;
			
			NewRatCharacter.ApplyAirFriction();
			
			if (!NewRatCharacter.HoldingJump || NewRatCharacter._velocity.y < float.Epsilon)
				_falling = true;

			if (_falling)
				stateMachine.TransitionTo<AirState>();

			// if (WallRunState.Requirement(Player))
			// 	stateMachine.TransitionTo<WallRunState>();

			if (NewRatCharacter.Grounded && NewRatCharacter._velocity.y < float.Epsilon)
				stateMachine.TransitionTo<MoveState>();
		}

		private static RaycastHit RayCast(NewRatCharacterController newRatCharacter, Vector3 direction) {
			Ray ray = new Ray( newRatCharacter._point2Transform.position/*Player.transform.position + Player._point2*/, direction);
			Physics.Raycast(ray, out var hit, newRatCharacter._colliderRadius + .5f, newRatCharacter._collisionMask);
			return hit;
		}

		public override void Exit() { }
	}
}