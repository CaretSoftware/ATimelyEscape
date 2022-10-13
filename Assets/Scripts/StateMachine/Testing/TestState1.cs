using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "State Machine/TestState1")]
public class TestState1 : State {
    public override void Run() {
        Debug.Log("Test state 1 running");
        if (Input.GetKey(KeyCode.Space)) stateMachine.TransitionTo<TestState2>();
        if (Input.GetKey(KeyCode.S)) stateMachine.TransitionBack();
    }

    public override void Exit() { Debug.Log("Exiting test state 1!"); }
}