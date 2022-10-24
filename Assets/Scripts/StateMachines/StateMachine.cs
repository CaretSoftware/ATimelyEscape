using System;
using System.Collections.Generic;

namespace StateMachines {
    public class StateMachine {
        public State CurrentState { get; private set; }
        private State queuedState;

        private Stack<State> automaton;
        private Dictionary<Type, State> stateDict = new Dictionary<Type, State>();


        public StateMachine(object actor, State[] states) {
            foreach (State state in states) {
                state.Owner = actor;
                state.StateMachine = this;
                stateDict.Add(state.GetType(), state);
                if (CurrentState != null) continue;
                CurrentState = state;
                queuedState = CurrentState;
            }

            automaton = new Stack<State>();
            CurrentState?.Enter();
        }

        public void TransitionTo<T>() where T : State { queuedState = stateDict[typeof(T)]; }

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