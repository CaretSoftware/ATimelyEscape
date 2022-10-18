using UnityEngine;
using StateMachines;

namespace StateMachines.Testing {
    [CreateAssetMenu(menuName = "State Machine/TestState1")]
    public class TestState1 : StateScriptableObject {
        public override void Run() {
            Debug.Log("Test state 1 running");
            if (Input.GetKey(KeyCode.Space)) StateMachine.TransitionTo<TestState2>();
            if (Input.GetKey(KeyCode.S)) StateMachine.TransitionBack();
        }

        public override void Exit() { Debug.Log("Exiting test state 1!"); }
    }
}