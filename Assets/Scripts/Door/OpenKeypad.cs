using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CallbackSystem
{
    public class OpenKeypad : MonoBehaviour
    {
        [SerializeField] private GameObject keypad;

        private void Start()
        {
            if (keypad == null)
                keypad = gameObject.transform.GetChild(0).gameObject;
           
        }

        public void Open(bool open = true) {
            Cursor.visible = open;
            Cursor.lockState = open ? CursorLockMode.Confined : CursorLockMode.Locked;
            keypad.SetActive(open);
            RatCharacterController.CharacterInput.IsPaused(open);
        }

        public void Close() { Open(false); }

        // private void Open(OpenKeypadEvent openKeypadEvent)
        // {
        //     if (openKeypadEvent.Keypad.Equals(keypad)){
        //         Open(openKeypadEvent.open);
        //     }
        // }
    }
}