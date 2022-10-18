using UnityEngine;

namespace StateMachines.Testing {
    [CreateAssetMenu(menuName = "State Machine/TestState2")]
    public class TestState2 : StateScriptableObject {
        public override void Run() {
            Debug.Log("Test State 2 running!");
            if (Input.GetKey(KeyCode.W)) StateMachine.TransitionBack();
        }

        public override void Exit() { Debug.Log("Exiting test state 2!"); }
    }
}