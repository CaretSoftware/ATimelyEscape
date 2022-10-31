using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinnishObject : MonoBehaviour
{
    [SerializeField] private GameObject finnish;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            finnish.SetActive(true);
        }
    }
}
