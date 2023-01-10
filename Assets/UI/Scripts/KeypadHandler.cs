using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
//using UnityEngine.UIElements;

public class KeypadHandler : MonoBehaviour
{
    [SerializeField] private GameObject keypadUI;
    [SerializeField] private GameObject firstSelectedGameObject;
    private NewRatCharacterController.NewRatCharacterController newRatCharacterController;
    private static KeypadHandler currentKeypadHandler;
    
    void Start() 
    {
        newRatCharacterController = FindObjectOfType<NewRatCharacterController.NewRatCharacterController>();
        keypadUI = gameObject.transform.GetChild(0).gameObject;
        PauseMenuBehaviour.pauseDelegate += Paused;
    }

    private void OnDestroy()
    {
        PauseMenuBehaviour.pauseDelegate -= Paused;
    }

    private void Paused(bool paused)
    {
        if (!paused && currentKeypadHandler != null && currentKeypadHandler == this && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(firstSelectedGameObject);
        }
    }

    public void OpenKeypad()
    {
        currentKeypadHandler = this;
        
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(firstSelectedGameObject);

        NewRatCameraController.SetLookTarget(transform);

        newRatCharacterController.KeypadInteraction = true;
        
        keypadUI.GetComponent<Animator>().Play("Open");
    }

    public void CloseKeypad()
    {
        currentKeypadHandler = null;
        ReleasePlayer();
        keypadUI.GetComponent<Animator>().Play("Close");
    }

    public void ReleasePlayer() 
    {
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
