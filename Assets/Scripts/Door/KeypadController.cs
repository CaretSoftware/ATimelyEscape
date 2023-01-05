using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text;
using System;
using CallbackSystem;
using RatCharacterController;
using EventSystem = UnityEngine.EventSystems.EventSystem;

public class KeypadController : DeviceController
{
    [Header("Values")]
    [SerializeField] private int maxDigits;
    [SerializeField] private int combination;

    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI screen;
    //[SerializeField] private UnityEngine.UI.Button[] keypadKeys;
    //[SerializeField] private int keypadColumns = 3;

    [Header("LockIcons")]
    [SerializeField] private GameObject lockClosed;
    [SerializeField] private GameObject lockOpen;

    [SerializeField] private GameObject stateOpen;
    [SerializeField] private GameObject stateClosed;

    [Header("Door to Open")]
    [SerializeField] private Door2 door;

    private Collider trigger;

    /*
    private KeyCode[] keyCodes = {
        KeyCode.Alpha0,
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Alpha8,
        KeyCode.Alpha9,
        KeyCode.Keypad0,
        KeyCode.Keypad1,
        KeyCode.Keypad2,
        KeyCode.Keypad3,
        KeyCode.Keypad4,
        KeyCode.Keypad5,
        KeyCode.Keypad6,
        KeyCode.Keypad7,
        KeyCode.Keypad8,
        KeyCode.Keypad9,
     };

    private const int keyAmount = 10;
   
    private int currentKeypadKeysIndex;
    private int previousKeypadIndex;
    private bool usingKeyboardDigits = true;
    */
    private Animator keyPadAnimator;
    private NewRatCharacterController.NewRatCharacterController newRatCharacterController;

    private void Start()
    {
        keyPadAnimator = GetComponent<Animator>();

        newRatCharacterController = FindObjectOfType<NewRatCharacterController.NewRatCharacterController>();

        trigger = gameObject.transform.GetComponentInParent<BoxCollider>();

        lockClosed.SetActive(true);
        lockOpen.SetActive(false);
    }

    public void KeypadDigitInput(int number)
    {
        if (screen.text.Length < maxDigits)
        {
            StringBuilder sb = new();
            sb.Append(screen.text).Append(number);
            screen.text = sb.ToString();

            /*
            if(screen.text.Length == combination.ToString().Length)
            {
                KeypadEnterInput();
            }
            */
        }
    }

    public void KeypadDeleteInput()
    {
        if (screen.text != null && screen.text != "")
        {
            screen.text = screen.text.Remove(screen.text.Length - 1, 1);
        }
    }

    public void KeypadEnterInput()
    {  
        if (screen.text.Equals(combination.ToString()))
        {
            keyPadAnimator.Play("RightCode");

            if(door != null) 
                door.SetDoor(true);

            if (trigger != null) 
                trigger.enabled = false;

            SwitchState();

            if(device != null)
                device.TurnedOn(true);
            else
                Debug.Log("Error: Missing device component");

            newRatCharacterController.KeypadInteraction = false;

        }
        else
        {
            keyPadAnimator.Play("WrongCode");
            screen.text = "";
        }     
    }
    private void DisableKeypad()
    {
        CharacterInput.IsPaused(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Destroy(gameObject);
    }

    private void SwitchState()
    {
        stateOpen.SetActive(!stateOpen.activeInHierarchy);
        stateClosed.SetActive(!stateClosed.activeInHierarchy);
    }

    /*
    private void Update()
    {
        for (int i = 0; i < keyCodes.Length; i++)
        {
            if (Input.GetKeyDown(keyCodes[i]))
            {
                
                if(i < keyAmount)
                {
                    KeypadDigitInput(i);                    
                }
                else
                {
                    KeypadDigitInput(i - keyAmount);     
                }
                break;
            }
        }
        if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace))
        {
            // To change buttons Appearance to Pressed
            keypadKeys[currentKeypadKeysIndex].gameObject.GetComponent<ButtonBehaviour>().ToPressedSprite();
            KeypadDeleteInput();
        }
        else if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            if (usingKeyboardDigits)
            {
                // To change buttons Appearance to Pressed
                keypadKeys[currentKeypadKeysIndex].gameObject.GetComponent<ButtonBehaviour>().ToPressedSprite();
                KeypadEnterInput();
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentKeypadKeysIndex = (currentKeypadKeysIndex + 1) % keypadKeys.Length;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentKeypadKeysIndex = (currentKeypadKeysIndex - 1 + keypadKeys.Length) % keypadKeys.Length;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentKeypadKeysIndex = (currentKeypadKeysIndex + keypadColumns) % keypadKeys.Length;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow)) 
        { 
            currentKeypadKeysIndex = (currentKeypadKeysIndex - keypadColumns + keypadKeys.Length) % keypadKeys.Length;
        }else if (Input.anyKeyDown)
        {
            usingKeyboardDigits = true;
        }
        if (currentKeypadKeysIndex != previousKeypadIndex)
        {
            // To change previousbuttons Appearence to Neutral
            keypadKeys[previousKeypadIndex].gameObject.GetComponent<ButtonBehaviour>().ToNeutralSprite();

            if (usingKeyboardDigits)
            {
                currentKeypadKeysIndex = previousKeypadIndex;
            }
            else
            {
                previousKeypadIndex = currentKeypadKeysIndex;
            }
            keypadKeys[currentKeypadKeysIndex].Select();

            // To change buttons Appearence to Selected
            keypadKeys[currentKeypadKeysIndex].gameObject.GetComponent<ButtonBehaviour>().ToSelectedSprite();

            usingKeyboardDigits = false;
        }
        if (usingKeyboardDigits)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
    */
}
