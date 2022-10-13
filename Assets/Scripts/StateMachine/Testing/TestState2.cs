using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

[CreateAssetMenu(menuName = "State Machine/TestState2")]
public class TestState2 : State {
    public override void Run() {
        Debug.Log("Test State 2 running!");
        if (Input.GetKey(KeyCode.W)) stateMachine.TransitionBack();
    }

    public override void Exit() { Debug.Log("Exiting test state 2!"); }
}