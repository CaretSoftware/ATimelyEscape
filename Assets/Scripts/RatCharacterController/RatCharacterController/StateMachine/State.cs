
namespace NewRatCharacterController {
    public abstract class State {

        public StateMachine stateMachine;

        public abstract void Enter();

        public abstract void Run();

        public abstract void Exit();
    }
}