using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllButtonPressed : MonoBehaviour
{

    [SerializeField] private Button[] buttons;
    [SerializeField] private bool allPressed;
    private Device door;


    // Start is called before the first frame update
    private void Awake()
    {
        foreach (Button button in buttons)
        {
            button.SetParent(this);
        }
    }

    public void IsAllPressed()
    {
        foreach (Button button in buttons)
        {
            allPressed = button.IsPressed();
            if (!allPressed)
            {
                break;
            }
        }
        door.TurnedOn(allPressed);
    }

    public void AllPressedFalse() { allPressed = false; }   

    public void SetDoor(Door door)
    {
        this.door = door;   
    }


}
