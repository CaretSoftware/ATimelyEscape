using UnityEngine;

namespace NewRatCharacterController
{
    public class LockState : BaseState {
        private const string State = nameof(LockState);
        private bool _couldTimeTravel;
        
        public override void Enter() {
            StateChange.stateUpdate?.Invoke(State);
            _couldTimeTravel = NewRatCharacter.NewCharacterInput.CanTimeTravel;
            NewRatCharacter.NewCharacterInput.CanTimeTravel = false;
            NewRatCharacter._velocity = Vector3.zero;
            NewRatCharacter.NewCharacterInput.EnableCharacterMovement(false);
        }

        public override void Run() {
            if (!NewRatCharacterController.Locked)
                stateMachine.TransitionTo<MoveState>();
        }

        public override void Exit() {
            NewRatCharacter.NewCharacterInput.CanTimeTravel = _couldTimeTravel;
            NewRatCharacter.AnimationController.SetCaught(false);
            NewRatCharacter.NewCharacterInput.EnableCharacterMovement(true);
        }
    }
}
