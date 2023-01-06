using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLineColor : MonoBehaviour
{
    [SerializeField] private Material onMaterial;
    [SerializeField] private Material offMaterial;
    private LineRenderer lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void ChangeColor()
    {
        lineRenderer.material = onMaterial;
    }
    public void ChangeOffColor()
    {
        if (offMaterial != null)
        {
            lineRenderer.material = offMaterial;
        }
    }
}
