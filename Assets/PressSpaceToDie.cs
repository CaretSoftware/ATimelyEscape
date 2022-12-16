using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressSpaceToDie : MonoBehaviour
{
    [SerializeField] private Transform checkpoint;
    void Update()
    {
           if(Input.GetKeyDown(KeyCode.Space))
               FailStateScript.Instance.PlayDeathVisualization(checkpoint);
    }
}
