using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoalTrigger : MonoBehaviour
{
    [SerializeField] private AudioClip goalClip;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInChildren<RatCharacterController.CharacterInput>())
        {
            FindObjectOfType<DialogueManager>().GoalReached(goalClip);
        }
    }
}
