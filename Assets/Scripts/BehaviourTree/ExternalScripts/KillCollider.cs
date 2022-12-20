using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Net.NetworkInformation;
using UnityEngine;
public class KillCollider : MonoBehaviour
{
    [SerializeField] private Transform checkpoint;
    private CapsuleCollider capsuleCollider;

    private void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        capsuleCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider collider)
    {
        print("Collision detected");
        if (collider.transform.tag.Equals("Player"))
            FailStateScript.Instance.PlayDeathVisualization(checkpoint);
    }
}
