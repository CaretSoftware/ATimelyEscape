using System;
using UnityEngine;

public class VariableManager : MonoBehaviour
{
    private NewRatCharacterController.NewRatCharacterController ratCharacter;
    
    private void Awake() {
        ratCharacter = FindObjectOfType<NewRatCharacterController.NewRatCharacterController>();
    }

    public SetVelocity(float velocity) {
        ratCharacter.SetVelocity(velocity);
    }

    public SetJumpForce(float jumpForce) {
        ratCharacter.SetJump(jumpForce);
    }
}