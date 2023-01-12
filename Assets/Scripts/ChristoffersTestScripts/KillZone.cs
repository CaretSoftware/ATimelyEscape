using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    [SerializeField] private GameObject checkPoint;
    [SerializeField] private bool isOnlyRatKillzone;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && checkPoint != null)
        {
            other.transform.position = checkPoint.transform.position;
        }
        if (!isOnlyRatKillzone)
        {
            if (other.CompareTag("Cube"))
            {
                other.gameObject.SetActive(false);
                Destroy(other.gameObject);
            }
        }
    }
}
