
namespace NewRatCharacterController {
    public abstract class BaseState : State {
        
        public NewRatCharacterController owner;
        private NewRatCharacterController _newRatCharacter;
        protected BaseState lastState;
        protected NewRatCharacterController NewRatCharacter => owner;
    }
}