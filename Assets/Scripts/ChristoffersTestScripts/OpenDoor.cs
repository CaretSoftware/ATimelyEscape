using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    [SerializeField] private Animator doorAnimator;

    private void OnTriggerEnter(Collider other)
    {
        if (!doorAnimator) Debug.Log("Assign DoorAnimator on this object", this);
        
        if (other.gameObject.tag == "Player")
        {
            doorAnimator.SetBool("Open", true);
        }
    }
}
