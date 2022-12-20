using FluffyGroomingTool;
using UnityEngine;

namespace NewRatCharacterController {
	public class CubePushState : BaseState {
		private const string State = nameof(CubePushState);

		private CubePush cube;
		private Transform _cube;
		private Vector3 _hitInverseNormal;
		private Vector3 _hitPosition;
		private Vector3 _pushedCubeOffset;
		private Quaternion worldRotation;
		private Rigidbody cubeRB;

		public static bool Requirement(NewRatCharacterController newRatCharacter) {
			// are we pressing interact? are we in front of cube?
			return newRatCharacter.Interacting && newRatCharacter.InFrontOfCube();
		}

		public override void Enter() {
			StateChange.stateUpdate?.Invoke(State);
			NewRatCharacter.AnimationController.Push(true);
			
			AttachPlayerToCube();
			
			//Quaternion rotation = Quaternion.LookRotation(_hitInverseNormal, Vector3.up);
			//NewRatCharacter.transform.position = _hitPosition;

			// parent player to cube?
			// freeze player input?
			// freeze player stuff
		}

		private void AttachPlayerToCube() {
			NewRatCharacter._velocity = Vector3.zero;
			Transform ratTransform = NewRatCharacter.transform;
			//ratTransform.parent = NewRatCharacter.cubeTransform;
			Quaternion rotation = Quaternion.LookRotation(NewRatCharacter.cubeHitInverseNormal);
			Vector3 position = NewRatCharacter.cubeHitPosition;
			position.y = NewRatCharacter.transform.position.y;
			NewRatCharacter.RatMesh.rotation = rotation;
			ratTransform.position = position + rotation * NewRatCharacter.pushOffset;
			_pushedCubeOffset = ratTransform.position - NewRatCharacter.cubeTransform.position;
			cube = NewRatCharacter.cubeTransform.GetComponent<CubePush>();
			cubeRB = NewRatCharacter.cubeTransform.GetComponent<Rigidbody>();
		}

		public override void Run() {
			// ask RatCharacterController for the input vector
			Vector3 input = NewRatCharacter.InputToCameraProjection( NewRatCharacter.InputVector );
			// move cubes rigidbody with that input

			Ray ray = new Ray(
				NewRatCharacter.transform.position + NewRatCharacter.halfHeight,
				NewRatCharacter.RatMesh.forward);
			
			OffsetPlayerPosition();
			RotatePlayerToSurface();

			void OffsetPlayerPosition() {
				if (cube != null) {
					cube.Push(input);
					NewRatCharacter.transform.position = cube.transform.position + _pushedCubeOffset;
				}
			}

			void RotatePlayerToSurface() {
				if (Physics.Raycast(ray, out RaycastHit hitInfo, .1f, NewRatCharacter.cubeLayerMask, QueryTriggerInteraction.Ignore)) {
					Vector3 rotation = (-hitInfo.normal).ProjectOnPlane().normalized;
					if (rotation != Vector3.zero)
						 NewRatCharacter.RatMesh .transform.rotation = Quaternion.LookRotation(rotation, Vector3.up);
				}
			}

			float velocityLateralMin = -.1f;
			if (NewRatCharacter.LetGoOfCube || cubeRB != null && cubeRB.velocity.y < velocityLateralMin) {
				Debug.Log("FALLING");
				NewRatCharacter.LetGoOfCube = false;
				stateMachine.TransitionTo<MoveState>();
			}
			
			if (!NewRatCharacter.Interacting)
				stateMachine.TransitionTo<MoveState>();
			
			if (NewRatCharacter.Caught)
				stateMachine.TransitionTo<CaughtState>();
		}

		public override void Exit() {
			NewRatCharacter.AnimationController.Push(false);
			//NewRatCharacter.transform.parent = null;
		}
	}
}

// TODO
// Unsubscriptions Emils Eventsystem
// Load markers på passande platser
// SmoothDamp on camera transform
// EventSystem timetravel bool don't set on Gretas timeTravel effect
// SkinnedMeshRenderer i Gretas Tool
// script for AI kinematics
// Displacement max distance