using NewRatCharacterController;
using UnityEngine;

namespace NewRatCharacterController
{
    public class WakeUpState : BaseState {
        private const string State = nameof(WakeUpState);
        private float _time;
        private float _wakeupDuration = 10f;

        public override void Enter() {
            StateChange.stateUpdate?.Invoke(State);
        }

        public override void Run() {
            _time += Time.deltaTime;
            NewRatCharacter.EnableCharacterMovement(false);
            NewRatCharacter.HandleVelocity();
            AddGravityForce();
            
            if (NewRatCharacter.Awakened || _time > _wakeupDuration)
                stateMachine.TransitionTo<MoveState>();
        }

        public override void Exit() {
            _time = 0f;
            NewRatCharacter.EnableCharacterMovement(true);
        }
        
        private void AddGravityForce() {
            float gravityMovement = -NewRatCharacter._defaultGravity * Time.deltaTime;
            NewRatCharacter._velocity.y += gravityMovement;
        }
    }
}