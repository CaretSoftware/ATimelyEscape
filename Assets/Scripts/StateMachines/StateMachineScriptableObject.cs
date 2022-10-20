using System;
using System.Collections.Generic;

namespace StateMachines {
    public class StateMachineScriptableObject {
        public StateScriptableObject CurrentState { get; private set; }
        private StateScriptableObject queuedState;

        private Stack<StateScriptableObject> automaton;
        private Dictionary<Type, StateScriptableObject> stateDict = new Dictionary<Type, StateScriptableObject>();

        public StateMachineScriptableObject(object actor, StateScriptableObject[] states) {
            foreach (StateScriptableObject state in states) {
                var instance = UnityEngine.Object.Instantiate(state);
                instance.Owner = actor;
                instance.StateMachine = this;
                stateDict.Add(instance.GetType(), instance);
                if (CurrentState != null) continue;
                CurrentState = instance;
                queuedState = CurrentState;
            }

            automaton = new Stack<StateScriptableObject>();
            CurrentState?.Enter();
        }

        public void TransitionTo<T>() where T : StateScriptableObject { queuedState = stateDict[typeof(T)]; }

        public void TransitionBack() {
            if (automaton.Count != 0) queuedState = automaton.Pop();
        }

        public void Run() {
            UpdateState();
            CurrentState.Run();
        }

        private void UpdateState() {
            if (queuedState == CurrentState) return;
            CurrentState?.Exit();
            automaton.Push(CurrentState);
            CurrentState = queuedState;
            CurrentState.Enter();
        }
    }
}