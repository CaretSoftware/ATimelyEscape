using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeCollider : MonoBehaviour
{
    private Vector3 spawnPosition;

    void Start()
    {
        spawnPosition = FindObjectOfType<RatCharacterController.CharacterInput>().transform.position;
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInChildren<RatCharacterController.CharacterInput>())
        {
            other.transform.position = spawnPosition;
        }
    }
}
