using UnityEngine;

namespace NewRatCharacterController {
    public class MoveState : BaseState {

        private float _maxSlopeAngle = 40;
        
        private const string State = nameof(MoveState);
        public override void Enter() {
            StateChange.stateUpdate?.Invoke(State);
            NewRatCharacter._jumpedOnce = false;
            NewRatCharacter.AnimationController.SetGrounded(true);
        }

        public override void Run() {
            
            StepUp();
            
            NewRatCharacter.HandleVelocity();

            if (Vector3.Angle(NewRatCharacter.GroundNormal, Vector3.up) < _maxSlopeAngle)
                ApplyStaticFriction();
            else
                AddGravityForce();

            NewRatCharacter.AnimationController.RotateCharacterMesh();

            if (CubePushState.Requirement(NewRatCharacter))
                stateMachine.TransitionTo<CubePushState>();
            
            if (NewRatCharacter.KeypadInteraction)
                stateMachine.TransitionTo<KeypadState>();
            
            if (NewRatCharacter.Jumped)
                stateMachine.TransitionTo<JumpState>();

            if (NewRatCharacter.Jumped && LedgeJumpState.Requirement(NewRatCharacter))
                stateMachine.TransitionTo<LedgeJumpState>();
            
            if (!NewRatCharacter.Grounded)
                stateMachine.TransitionTo<AirState>();
            
            if (NewRatCharacter.Caught)
                stateMachine.TransitionTo<CaughtState>();
        }

        private void StepUp() {

            Vector3 stepHeightVector = Vector3.up * NewRatCharacter.stepHeight;
            Vector3 velocity = Vector3.ProjectOnPlane(NewRatCharacter._velocity, Vector3.up) * Time.deltaTime;
            Vector3 direction = velocity.normalized;
            float maxDistance = velocity.magnitude + NewRatCharacter._skinWidth;
            
            if (Physics.CapsuleCast(
                    NewRatCharacter._point1Transform.position,
                    NewRatCharacter._point2Transform.position, 
                    NewRatCharacter._colliderRadius, 
                    direction, 
                    out RaycastHit lowHit,
                    maxDistance,
                    NewRatCharacter._collisionMask) &&
                NewRatCharacter._velocity.y < float.Epsilon &&
                !Physics.CapsuleCast(
                    NewRatCharacter._point1Transform.position + stepHeightVector,
                    NewRatCharacter._point2Transform.position + stepHeightVector, 
                    NewRatCharacter._colliderRadius, 
                    direction, 
                    maxDistance + NewRatCharacter._colliderRadius,
                    NewRatCharacter._collisionMask)) {
                
                Vector3 maxMagnitude = Vector3.ClampMagnitude(direction * NewRatCharacter._colliderRadius, NewRatCharacter._velocity.magnitude);
                Physics.CapsuleCast(
                    NewRatCharacter._point1Transform.position + stepHeightVector + maxMagnitude,
                    NewRatCharacter._point2Transform.position + stepHeightVector + maxMagnitude,//direction * Char._colliderRadius,
                    NewRatCharacter._colliderRadius,
                    Vector3.down,
                    out RaycastHit hit, 
                    float.MaxValue, 
                    NewRatCharacter._collisionMask);
                
                NewRatCharacter.transform.position += (stepHeightVector - hit.distance * Vector3.up) * 1.0f;
            }
        }

        private void ApplyStaticFriction() {
            
            if (Vector3.ProjectOnPlane(NewRatCharacter._velocity, Vector3.up).magnitude <
                NewRatCharacter.normalForce.magnitude * NewRatCharacter._staticFrictionCoefficient) {
                
                NewRatCharacter._velocity = Vector3.zero;
            }
        }

        private void AddGravityForce() {

            float gravityMovement = -NewRatCharacter._defaultGravity * Time.deltaTime;
            NewRatCharacter._velocity.y += gravityMovement;
        }

        public override void Exit() {
            NewRatCharacter.AnimationController.SetGrounded(false);

        }
    }
}