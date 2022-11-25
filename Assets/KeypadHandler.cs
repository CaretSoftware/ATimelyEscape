using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RatCharacterController;

public class KeypadHandler : MonoBehaviour
{
    [SerializeField] private GameObject keypadUI;

    // Start is called before the first frame update
    void Start()
    {
        keypadUI = gameObject.transform.GetChild(0).gameObject;
    }

    public void OpenKeypad()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        keypadUI.GetComponent<Animator>().Play("Open");
        CharacterInput.IsPaused(true);
    }

    public void CloseKeypad()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        keypadUI.GetComponent<Animator>().Play("Close");
        CharacterInput.IsPaused(false);
    }
}
