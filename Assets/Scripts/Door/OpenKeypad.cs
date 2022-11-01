using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OpenKeypad : MonoBehaviour
{
    [SerializeField] GameObject keypad;

    private void Start()
    {
        if (keypad == null)
            keypad = gameObject.transform.GetChild(0).gameObject;
    }
    public void Open(){Open(true);}

    public void Open(bool open) { keypad.SetActive(open);}

    public void Close() { Open(false); }
}
