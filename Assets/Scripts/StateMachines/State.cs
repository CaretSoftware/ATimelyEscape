using UnityEngine;

namespace StateMachines {
    public abstract class State {
        public StateMachine StateMachine { get; set; }
        public object Owner { get; set; }
        public virtual void Enter() { }
        public virtual void Exit() { }
        public virtual void Run() { }
    }

    public abstract class StateScriptableObject : ScriptableObject {
        public StateMachineScriptableObject StateMachine { get; set; }
        public object Owner { get; set; }
        public virtual void Enter() { }
        public virtual void Exit() { }
        public virtual void Run() { }
    }
}