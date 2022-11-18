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
        public string context;
        public float waitForTime;
    }
}
