using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideValueSliders : MonoBehaviour
{
    // Start is called before the first frame update
    public Canvas canvas;

    private void Start() {
        canvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            canvas.enabled = !canvas.enabled;
        }
    }
}
