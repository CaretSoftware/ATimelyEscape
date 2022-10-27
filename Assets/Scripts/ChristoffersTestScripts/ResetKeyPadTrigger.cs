using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetKeyPadTrigger : MonoBehaviour
{

    [SerializeField] private Collider keyPadTrigger;

    private void OnTriggerEnter(Collider other)
    {
        keyPadTrigger.enabled = true; 
    }


}
