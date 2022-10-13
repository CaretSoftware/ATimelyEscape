using UnityEngine;

public abstract class State : ScriptableObject {
    public StateMachine stateMachine;
    public object owner;
    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Run() { }
}