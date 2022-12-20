using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeCollider : MonoBehaviour
{
    private Vector3 spawnPosition;
    [SerializeField] private Transform playerPos;

    void Start()
    {
        if (playerPos)
        {
            spawnPosition = playerPos.position;
        }
        else
        {
            spawnPosition = GameObject.FindWithTag("Player").transform.position;
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
