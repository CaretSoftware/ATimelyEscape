using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideValueSliders : MonoBehaviour
{
    // Start is called before the first frame update
    public Canvas canvas;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            canvas.enabled = !canvas.enabled;
        }
    }
}
