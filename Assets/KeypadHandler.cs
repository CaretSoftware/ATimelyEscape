using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeypadHandler : MonoBehaviour
{
    [SerializeField] private GameObject keypadUI;

    private NewRatCharacterController.NewRatCharacterController newRatCharacterController;
    // Start is called before the first frame update
    void Start()
    {
        newRatCharacterController = FindObjectOfType<NewRatCharacterController.NewRatCharacterController>();
        keypadUI = gameObject.transform.GetChild(0).gameObject;
    }

    public void OpenKeypad()
    {
        //Cursor.visible = true;
        //Cursor.lockState = CursorLockMode.Confined;

        newRatCharacterController.KeypadInteraction = true;
        keypadUI.GetComponent<Animator>().Play("Open");
        //CharacterInput.IsPaused(true);
    }

    public void CloseKeypad()
    {
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
        newRatCharacterController.KeypadInteraction = false;
        keypadUI.GetComponent<Animator>().Play("Close");
        //CharacterInput.IsPaused(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
            OpenKeypad();
    }

    /*
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            CloseKeypad();
    }
    */
}
