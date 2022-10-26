using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    [SerializeField] private GameObject checkPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = checkPoint.transform.position;
            Debug.Log("SCIENTIST CAUGHT YOU!");
        }
    }
}
