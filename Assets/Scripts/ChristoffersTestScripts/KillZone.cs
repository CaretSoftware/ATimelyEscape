using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    [SerializeField] private Transform checkPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && checkPoint != null)
        {
            FailStateScript.Instance.PlayDeathVisualization(checkPoint);
            //other.transform.position = checkPoint.transform.position;
        }
        else if(other.CompareTag("Cube"))
        {
            other.gameObject.SetActive(false);
            Destroy(other.gameObject);
        }
    }
}
