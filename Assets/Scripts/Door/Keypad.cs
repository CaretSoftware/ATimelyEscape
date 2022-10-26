using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;
using System;

public class Keypad : MonoBehaviour
{
    [SerializeField] private int maxDigits;
    [SerializeField] private int combination;
    [SerializeField] private TextMeshProUGUI screen;
    [SerializeField] private UnityEngine.UI.Button[] keypadKeys;
    [SerializeField] private int keypadColumns = 3;

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

    public void KeypadDigitInput(int number)
    {
        if (screen.text.ToCharArray().Length < maxDigits)
        {
            StringBuilder sb = new();
            sb.Append(screen.text).Append(number);
            screen.text = sb.ToString();
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
        StringBuilder sb = new();
        sb.Append("FUCK");
        if (screen.text.Equals(combination.ToString()))
        {
            sb.Append(" YEAH");
        }
        sb.Append("!!!");
        Debug.Log(sb.ToString());
    }

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
            KeypadDeleteInput();
        }
        else if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            if (usingKeyboardDigits)
            {
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
            if (usingKeyboardDigits)
            {
                currentKeypadKeysIndex = previousKeypadIndex;
            }
            else
            {
                previousKeypadIndex = currentKeypadKeysIndex;
            }
            keypadKeys[currentKeypadKeysIndex].Select();
            usingKeyboardDigits = false;
        }
        if (usingKeyboardDigits)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
        

    }
}
