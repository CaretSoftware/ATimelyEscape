using System;
using UnityEngine;

public class VariableManager : MonoBehaviour
{
    private NewRatCharacterController.NewRatCharacterController ratCharacter;
    
    private void Awake() {
        ratCharacter = FindObjectOfType<NewRatCharacterController.NewRatCharacterController>();
    }

    public void SetVelocity(float velocity) {
        ratCharacter.SetVelocity(velocity);
    }

    public void SetJumpForce(float jumpForce) {
        ratCharacter.SetJump(jumpForce);
    }

    public void SetPushCubeVelocity(float velocity) {
        ratCharacter.SetPushVelocity(velocity);
    }

    public void SetAcceleration(float acceleration) {
        ratCharacter.SetAcceleration(acceleration);
    }
    
    public void SetDeceleration(float deceleration) {
        ratCharacter.SetDeceleration(deceleration);
    }
}