using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StateMachine {
    private State currentState;
    private State queuedState;

    private Stack<State> automaton;
    private Dictionary<Type, State> stateDict = new Dictionary<Type, State>();

    public StateMachine(object actor, State[] states) {
        foreach (State state in states) {
            State instance = UnityEngine.Object.Instantiate(state);
            instance.owner = actor;
            instance.stateMachine = this;
            stateDict.Add(instance.GetType(), instance);
            if (currentState == null) {
                currentState = instance;
                queuedState = currentState;
            }
        }

        automaton = new Stack<State>();
        currentState?.Enter();
    }

    public void TransitionTo<T>() where T : State { queuedState = stateDict[typeof(T)]; }

    public void TransitionBack() {
        if (automaton.Count != 0) queuedState = automaton.Pop();
    }

    public void Run() {
        UpdateState();
        currentState.Run();
    }

    private void UpdateState() {
        if (queuedState != currentState) {
            currentState?.Exit();
            automaton.Push(currentState);
            currentState = queuedState;
            currentState.Enter();
        }
    }
}