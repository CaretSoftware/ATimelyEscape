using UnityEngine;

namespace NewRatCharacterController {
    public class JumpState : BaseState {
        private const string State = nameof(JumpState);
        private bool _falling = false;

        public override void Enter() {
            StateChange.stateUpdate?.Invoke(State);
            _falling = false;
            NewRatCharacter._jumpedOnce = true;
            NewRatCharacter.airTime = 0;
            NewRatCharacter._velocity.y = NewRatCharacter._jumpForce;
            NewRatCharacter.AnimationController.Jump();
        }

        public override void Run() {

            NewRatCharacter.AirControl();

            Vector3 gravityMovement = NewRatCharacter._defaultGravity * Time.deltaTime * Vector3.down;

            NewRatCharacter._velocity += gravityMovement;

            NewRatCharacter.ApplyAirFriction();
            
            NewRatCharacter.AnimationController.RotateCharacterMesh();

            if (!NewRatCharacter.HoldingJump || NewRatCharacter._velocity.y < float.Epsilon)
                _falling = true;

            if (_falling)
                stateMachine.TransitionTo<AirState>();

            if (WallRunState.Requirement(NewRatCharacter))
                stateMachine.TransitionTo<WallRunState>();

            if (NewRatCharacter.Grounded && NewRatCharacter._velocity.y < float.Epsilon)
                stateMachine.TransitionTo<MoveState>();
            
            if (NewRatCharacter.Caught)
                stateMachine.TransitionTo<CaughtState>();
        }

        public override void Exit() {
            lastState = this;
        }
    }
}