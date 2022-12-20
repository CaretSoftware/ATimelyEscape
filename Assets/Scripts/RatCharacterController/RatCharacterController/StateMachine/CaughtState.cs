using UnityEngine;
namespace NewRatCharacterController
{
    public class CaughtState : BaseState {
        private const string State = nameof(CaughtState);

        public override void Enter() {
            StateChange.stateUpdate?.Invoke(State);
            NewRatCharacter._velocity = Vector3.zero;
        }

        public override void Run() {
            if (!NewRatCharacter.Caught)
                stateMachine.TransitionTo<MoveState>();
        }

        public override void Exit() {
            
        }
    }
}