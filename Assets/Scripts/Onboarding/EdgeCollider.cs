using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeCollider : MonoBehaviour
{
    private Vector3 spawnPosition;
    [SerializeField] Transform playerTransform;

    void Start()
    {
        if (playerTransform)
        {
            spawnPosition = playerTransform.position;
        }
        else
        {
            Transform tryFindTransform = FindObjectOfType<RatCharacterController.CharacterInput>().transform;
            if (tryFindTransform)
            {
                spawnPosition = tryFindTransform.position;
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (spawnPosition != null)
            {
                other.transform.position = spawnPosition;
            }
        }
    }
}
