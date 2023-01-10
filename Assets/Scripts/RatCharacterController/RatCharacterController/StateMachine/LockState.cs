using UnityEngine;

namespace NewRatCharacterController
{
    public class LockState : BaseState {
        private const string State = nameof(LockState);

        public override void Enter() {
            StateChange.stateUpdate?.Invoke(State);
            NewRatCharacter.NewCharacterInput.CanTimeTravel = false;
            NewRatCharacter._velocity = Vector3.zero;
            NewRatCharacter.NewCharacterInput.EnableCharacterMovement(false);
        }

        public override void Run() {
            if (!NewRatCharacterController.Locked)
                stateMachine.TransitionTo<MoveState>();
        }

        public override void Exit() {
            NewRatCharacter.NewCharacterInput.CanTimeTravel = true;
            NewRatCharacter.AnimationController.SetCaught(false);
            NewRatCharacter.NewCharacterInput.EnableCharacterMovement(true);
        }
    }
}
