using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathVisualization : MonoBehaviour
{
    private Camera camera;
    
    private void Start()
    {
        camera = GetComponentInParent<Camera>();
    }

    public void PlayDeathVisualization()
    {
        
    }
}
