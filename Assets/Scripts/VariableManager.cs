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

    public void SetPushVelocity(float velocity) {
        ratCharacter.pushSpeed = velocity;
    }
    
}