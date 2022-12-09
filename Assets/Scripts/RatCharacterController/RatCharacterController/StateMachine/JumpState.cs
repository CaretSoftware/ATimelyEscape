using UnityEngine;

namespace NewRatCharacterController {
    public class JumpState : BaseState {
        private bool _falling = false;

        private const string State = "JumpState";
        public override void Enter() {
            StateChange.stateUpdate?.Invoke(State);
            _falling = false;
            NewRatCharacter._jumpedOnce = true;
            NewRatCharacter.airTime = 0;
            NewRatCharacter._velocity.y = NewRatCharacter._jumpForce;
        }

        public override void Run() {

            NewRatCharacter.AirControl();

            Vector3 gravityMovement = NewRatCharacter._defaultGravity * Time.deltaTime * Vector3.down;

            NewRatCharacter._velocity += gravityMovement;

            NewRatCharacter.ApplyAirFriction();

            if (!NewRatCharacter.HoldingJump || NewRatCharacter._velocity.y < float.Epsilon)
                _falling = true;

            if (_falling)
                stateMachine.TransitionTo<AirState>();

            if (WallRunState.Requirement(NewRatCharacter))
                stateMachine.TransitionTo<WallRunState>();

            if (NewRatCharacter.Grounded && NewRatCharacter._velocity.y < float.Epsilon)
                stateMachine.TransitionTo<MoveState>();
        }

        public override void Exit() {
            lastState = this;
        }
    }
}