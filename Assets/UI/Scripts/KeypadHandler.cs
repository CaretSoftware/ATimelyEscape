using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
//using UnityEngine.UIElements;

public class KeypadHandler : MonoBehaviour
{
    [SerializeField] private GameObject keypadUI;

    private NewRatCharacterController.NewRatCharacterController newRatCharacterController;
    private NewRatCameraController cameraController;
    [SerializeField] private GameObject firstSelectedGameObject;

    //[SerializeField] private PanelSettings panelSettings;
    
    void Start() {
        cameraController = FindObjectOfType<NewRatCameraController>();
        newRatCharacterController = FindObjectOfType<NewRatCharacterController.NewRatCharacterController>();
        keypadUI = gameObject.transform.GetChild(0).gameObject;
    }

    public void OpenKeypad()
    {
        if (EventSystem.current != null) 
            EventSystem.current.SetSelectedGameObject(firstSelectedGameObject);

        NewRatCameraController.SetLookTarget(transform);
        
        newRatCharacterController.KeypadInteraction = true;
        
        keypadUI.GetComponent<Animator>().Play("Open");
        //CharacterInput.IsPaused(true);
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
    }

    public void CloseKeypad()
    {
        ReleasePlayer();
        
        keypadUI.GetComponent<Animator>().Play("Close");
        //CharacterInput.IsPaused(false);
    }

    public void ReleasePlayer() 
    {
        Debug.Log("CLOSED KEYPAD");
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
        NewRatCameraController.UnlockLookTarget();
        newRatCharacterController.KeypadInteraction = false;
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.CompareTag("Player"))
            OpenKeypad();
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            CloseKeypad();
    }
}
