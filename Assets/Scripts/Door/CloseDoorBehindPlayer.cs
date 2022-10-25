using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseDoorBehindPlayer : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private BoxCollider doorProtection;
    [SerializeField] private Door door;
    private void OnTriggerEnter(Collider other)
    {
        if (layerMask == (layerMask | 1 << other.gameObject.layer))
        {
            doorProtection.enabled = true;
            door.TurnedOn(false);
        }
    }
}
