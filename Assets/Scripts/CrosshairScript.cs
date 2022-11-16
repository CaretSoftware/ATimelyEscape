using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairScript : MonoBehaviour
{
    RGBSlider rgbSlider;
    [SerializeField] private Image crosshair;
    [SerializeField] private LayerMask layerMask;
    private void Start()
    {
        crosshair.color = new Color(1, 1, 1, 0.75f);
    }

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 50f, layerMask))
        {
           // if (hit.transform.gameObject.CompareTag("Cube"))
            {
                crosshair.color = new Color(1, 0, 0, 0.75f);
            }

        }
        else
        {
            crosshair.color = new Color(1, 1, 1, 0.75f);
        }
    }
}