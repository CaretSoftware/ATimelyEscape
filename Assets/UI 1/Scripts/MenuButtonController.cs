using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonController : MonoBehaviour
{
    public int index;

    private bool keyDown;
    [SerializeField] int maxIndex;

    //public AudioSource audioSource;

    // Update is called once per frame

    private Vector3 lastMousePosition;
    public bool mouseInUse;

    void Update()
    {
        if (Input.mousePosition != lastMousePosition)
        {
            lastMousePosition = Input.mousePosition;
            mouseInUse = true;
        }

        if (Input.GetAxis("Vertical") != 0)
        {
            mouseInUse = false;

            if (!keyDown)
            {
                if(Input.GetAxis ("Vertical") < 0)
                {
                    if(index < maxIndex)
                    {
                        index++;
                    }
                    else
                    {
                        index = 0;
                    }
                }
                else if(Input.GetAxis ("Vertical") > 0)
                {
                    if(index > 0)
                    {
                        index--;
                    }
                    else
                    {
                        index = maxIndex;
                    }
                }
                keyDown = true;
            }
        }
        else
        {
            keyDown = false;
        }
    }
}
