using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyGlas : MonoBehaviour
{
    [SerializeField] private GameObject glas;
    private void OnTriggerEnter(Collider other)
    {
        
        if(other.gameObject.tag == "Enemy")
        {
            Debug.Log("Hit");
            Destroy(glas);
        }
    }
}
