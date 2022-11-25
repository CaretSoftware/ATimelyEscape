using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CallbackSystem;

namespace CallbackSystem
{
    public class UIEvent : Event { }    

    public class CallHintAnimation : UIEvent
    {
        public string animationName;
        public float waitForTime;
    }

    public class CubeIconStateEvent : UIEvent
    {
        public bool objectCharged;
    }

    public class OpenKeypadEvent : UIEvent
    {
        public GameObject Keypad;
        public bool open;

        public OpenKeypadEvent(GameObject keypad, bool open)
        {
            Keypad = keypad;
            this.open = open;
        }
    }

    public class CloseKeypadEvent : UIEvent
    {
        public GameObject Keypad;

        public CloseKeypadEvent(GameObject keypad)
        {
            Keypad = keypad;
        }
    }
}
