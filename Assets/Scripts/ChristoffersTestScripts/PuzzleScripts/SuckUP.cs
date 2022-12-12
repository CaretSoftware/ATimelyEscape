using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuckUP : MonoBehaviour
{
    [SerializeField] private Transform suckPosition;
    [SerializeField] private float suckSpeed;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            other.gameObject.transform.position = suckPosition.position;
        }
    }
}
