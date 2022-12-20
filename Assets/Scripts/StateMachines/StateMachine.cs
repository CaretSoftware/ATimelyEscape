using System;
using System.Collections.Generic;

namespace StateMachines {
    public class StateMachine {
        public State CurrentState { get; set; }
        public State QueuedState { get; set; }

        private Stack<State> automaton;
        public Dictionary<Type, State> stateDict = new Dictionary<Type, State>();


        public StateMachine(object actor, State[] states) {
            foreach (State state in states) {
                state.Owner = actor;
                state.StateMachine = this;
                stateDict.Add(state.GetType(), state);
                if (CurrentState != null) continue;
                CurrentState = state;
                QueuedState = CurrentState;
            }

            automaton = new Stack<State>();
            CurrentState?.Enter();
        }

        public void TransitionTo<T>() where T : State { QueuedState = stateDict[typeof(T)]; }
        public void TransitionTo(State state) { QueuedState = state; }

        public void TransitionBack() {
            if (automaton.Count != 0) QueuedState = automaton.Pop();
        }


        public void Run() {
            UpdateState();
            CurrentState.Run();
        }

        private void UpdateState() {
            if (QueuedState == CurrentState) return;
            CurrentState?.Exit();
            automaton.Push(CurrentState);
            CurrentState = QueuedState;
            CurrentState.Enter();
        }
    }
}