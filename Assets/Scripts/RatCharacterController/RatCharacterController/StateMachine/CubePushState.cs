using FluffyGroomingTool;
using UnityEngine;

namespace NewRatCharacterController {
	public class CubePushState : BaseState {
		public delegate void PushCubeEvent(Transform cube);
		public static PushCubeEvent pushCubeEvent;
		
		public delegate void CubeLetGo();
		public static CubeLetGo cubeLetGo;

		public delegate void PushCubeUIOn(bool on);
		public static PushCubeUIOn pushCubeUIOn;

		private const string State = nameof(CubePushState);

		private CubePush cube;
		private Transform _cube;
		private Vector3 _hitInverseNormal;
		private Vector3 _hitPosition;
		private Vector3 _pushedCubeOffset;
		private Quaternion worldRotation;
		private Rigidbody cubeRB;

		private bool _letGo;
		
		public CubePushState() {
			cubeLetGo += LetGoOfCUbe;
		}
		
		~CubePushState() {
			cubeLetGo -= LetGoOfCUbe;
		}

		private void LetGoOfCUbe() {
			_letGo = true;
		}

		public static bool Requirement(NewRatCharacterController newRatCharacter) {
			// are we pressing interact? are we in front of cube?
			return newRatCharacter.Interacting && newRatCharacter.InFrontOfCube();
		}

		public override void Enter() {
			StateChange.stateUpdate?.Invoke(State);
			CubePushState.pushCubeUIOn?.Invoke(false);
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
			Transform cubeTransform = NewRatCharacter.cubeTransform;
			_pushedCubeOffset = ratTransform.position - cubeTransform.position;
			cube = cubeTransform.GetComponent<CubePush>();
			cubeRB = cubeTransform.GetComponent<Rigidbody>();
			
			pushCubeEvent?.Invoke(NewRatCharacter.cubeTransform);
		}

		public override void Run() {
			// ask RatCharacterController for the input vector
			Vector3 input = NewRatCharacter.InputToCameraProjection( NewRatCharacter.InputVector );
			// move cubes rigidbody with that input

			float velocity = NewRatCharacter.pushSpeed;
			
			Ray ray = new Ray(
				NewRatCharacter.transform.position + NewRatCharacter.halfHeight,
				NewRatCharacter.RatMesh.forward);

			OffsetPlayerPosition();
			RotatePlayerToSurface();

			void OffsetPlayerPosition() {
				if (cube != null) {
					cube.Push(input * velocity);
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

			float velocityLateralMin = -.2f;
			if (_letGo || NewRatCharacter.LetGoOfCube || (cubeRB != null && cubeRB.velocity.y < velocityLateralMin)) {
				NewRatCharacter.LetGoOfCube = false;
				stateMachine.TransitionTo<MoveState>();
			}
			
			if (!NewRatCharacter.Interacting)
				stateMachine.TransitionTo<MoveState>();
			
			if (NewRatCharacter.Caught)
				stateMachine.TransitionTo<CaughtState>();
			
			if (NewRatCharacterController.Locked)
				stateMachine.TransitionTo<LockState>();
		}

		public override void Exit() {
			CubePushState.pushCubeUIOn?.Invoke(true);
			NewRatCharacter.AnimationController.Push(false);
			_letGo = false;
			//NewRatCharacter.transform.parent = null;
		}
	}
}
