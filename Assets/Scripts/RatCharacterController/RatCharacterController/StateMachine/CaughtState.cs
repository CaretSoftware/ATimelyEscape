using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
namespace NewRatCharacterController
{
    public class CaughtState : BaseState {
        private const string State = nameof(CaughtState);

        public override void Enter() {
            StateChange.stateUpdate?.Invoke(State);
            NewRatCharacter.NewCharacterInput.CanTimeTravel = false;
            NewRatCharacter._velocity = Vector3.zero;
            NewRatCharacter.AnimationController.SetCaught(true);
        }

        public override void Run() {
            if (!NewRatCharacter.Caught)
                stateMachine.TransitionTo<MoveState>();
        }

        public override void Exit()
        {
            NewRatCharacter.NewCharacterInput.CanTimeTravel = true;
            NewRatCharacter.AnimationController.SetCaught(false);
            NewRatCharacter.transform.parent = null; // if caught and parented to scientist hand
        }
    }
}
