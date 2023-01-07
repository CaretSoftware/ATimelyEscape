using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyThis : MonoBehaviour
{
    [SerializeField] private GameObject destroyThis;

    private void OnTriggerEnter(Collider other)
    {
        Destroy(destroyThis);
    }

}
