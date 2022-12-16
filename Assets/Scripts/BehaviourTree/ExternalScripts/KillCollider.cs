using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class KillCollider : MonoBehaviour
{
    [SerializeField] private Transform checkpoint;
    private BoxCollider collider;

    private void Start()
    {
        collider = GetComponent<BoxCollider>();
        collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider collider)
    {
        print("collision");
        if (collider.transform.tag.Equals("Player"))
        {
            print("collision with player detected");
            FailStateScript.Instance.PlayDeathVisualization(checkpoint);
        }
    }
}
