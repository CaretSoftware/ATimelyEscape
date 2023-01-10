using UnityEngine;

namespace NewRatCharacterController {
	public class LedgeJumpState : BaseState {
		private const string State = nameof(LedgeJumpState);

		public delegate void LedgeJumpDone();
		public static LedgeJumpDone ledgeJumpDone;
		
		private static readonly Vector3 MarginVector = .01f * Vector3.up;
		private static readonly Vector3 LedgeHeight = .2f * Vector3.up;
		private const float MaxDistance = .1f;
		private const float Margin = .01f;
		
		private static Vector3 _hitInverseNormal;
		private static Vector3 _hitPosition;

		private static bool _ledgeJumpDone;

		public LedgeJumpState() {
			ledgeJumpDone += SetLedgeJumpDone;
		}

		~LedgeJumpState() {
			ledgeJumpDone -= SetLedgeJumpDone;
		}

		private void SetLedgeJumpDone() {
			_ledgeJumpDone = true;
		}
		
		public static bool Requirement(NewRatCharacterController newRatCharacter) {
			return newRatCharacter.Grounded && LedgeAhead(newRatCharacter);
		}

		private static bool LedgeAhead(NewRatCharacterController ratCharacter) {
			Transform playerTransform = ratCharacter.RatMesh;
			CapsuleCollider capsuleCollider = ratCharacter.CharCollider;
			Vector3 playerPosition = playerTransform.position;
			Vector3 playerForward = playerTransform.forward;

			Ray ray = new Ray(playerPosition + Vector3.up * .1f, playerForward);
			
			float radius = capsuleCollider.radius - Margin;
			
			Vector3 point0 = ratCharacter._point1Transform.position + LedgeHeight + MarginVector;
			Vector3 point1 = ratCharacter._point2Transform.position + LedgeHeight + MarginVector;

			LayerMask groundedLayerMask = ratCharacter._collisionMask;

			RaycastHit hitInfo;
			if (RayCastForLedge()) {
				
				_hitInverseNormal = -hitInfo.normal.ProjectOnPlane();
				_hitPosition = hitInfo.point;
				Debug.DrawRay(_hitPosition, _hitInverseNormal * .1f, Color.magenta, 2, false);

				ratCharacter.point0 = point0;
				ratCharacter.point1 = point1;
				
				return !Physics.CapsuleCast(
					point1: point0,
					point2: point1,
					radius: radius - Margin,
					direction: _hitInverseNormal,
					maxDistance: MaxDistance,
					groundedLayerMask,
					QueryTriggerInteraction.Ignore);
			}

			return false;
			
			bool RayCastForLedge() {
				bool objectInFront = Physics.Raycast(ray, out RaycastHit hit, MaxDistance, groundedLayerMask,
					QueryTriggerInteraction.Ignore); 
				bool clearAbove =
				        Physics.OverlapCapsule(point0, point1, radius, groundedLayerMask,
					        QueryTriggerInteraction.Ignore).Length < 1;

				hitInfo = hit;
				
				return objectInFront && clearAbove;
			}
		}

		public override void Enter() {
			StateChange.stateUpdate?.Invoke(State);
			_ledgeJumpDone = false;
			NewRatCharacter._velocity = Vector3.zero;
			Quaternion rotation = Quaternion.LookRotation(_hitInverseNormal, Vector3.up);
			_hitPosition.y = NewRatCharacter.transform.position.y;
			NewRatCharacter.RatMesh.transform.rotation = rotation;
			NewRatCharacter.transform.position = _hitPosition;

			NewRatCharacter.AnimationController.SetLedgeJump();
		}

		public override void Run() {
			if (_ledgeJumpDone) {
				stateMachine.TransitionTo<MoveState>();
				NewRatCharacter.LedgeVaultTeleport();
			}
			
			if (NewRatCharacter.Caught) 
				stateMachine.TransitionTo<CaughtState>();
			
			if (NewRatCharacterController.Locked)
				stateMachine.TransitionTo<LockState>();
		}

		public override void Exit() { }
	}
}