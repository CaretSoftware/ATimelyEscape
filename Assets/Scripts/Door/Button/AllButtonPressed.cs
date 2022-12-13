using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllButtonPressed : DeviceController
{

    [SerializeField] private CubeButton[] buttons;
    [SerializeField] private bool allPressed;


    // Start is called before the first frame update
    private void Awake()
    {
        foreach (CubeButton button in buttons)
        {
            button.SetParent(this);
        }
    }

    public void IsAllPressed()
    {
        foreach (CubeButton button in buttons)
        {
            
            allPressed = button.IsPressed();
            if (!allPressed)
            {
                break;
            }
        }
        Debug.Log(allPressed);
        device.TurnedOn(allPressed);
    }

    public void AllPressedFalse() { allPressed = false; }   

    


}
