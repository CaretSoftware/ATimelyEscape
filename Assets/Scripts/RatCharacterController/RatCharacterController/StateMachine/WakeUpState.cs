using NewRatCharacterController;
using UnityEngine;

namespace NewRatCharacterController
{
    public class WakeUpState : BaseState {
        private const string State = nameof(WakeUpState);
        private float t;
        private float _wakeupDuration = 5f;

        public override void Enter()
        {
            StateChange.stateUpdate?.Invoke(State);
        }

        public override void Run()
        {
            NewRatCharacter.EnableCharacterMovement(false);
            NewRatCharacter.HandleVelocity();
            AddGravityForce();
            
            if (NewRatCharacter.Awakened)
                stateMachine.TransitionTo<MoveState>();
        }

        public override void Exit()
        {
            NewRatCharacter.EnableCharacterMovement(true);
        }
        
        private void AddGravityForce() {
            float gravityMovement = -NewRatCharacter._defaultGravity * Time.deltaTime;
            NewRatCharacter._velocity.y += gravityMovement;
        }
    }
}