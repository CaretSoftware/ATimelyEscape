using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CallbackSystem;

public class CallTutorialHints : MonoBehaviour
{
    public void StartHintAnimation(string name)
    {
        CallHintAnimation call = new CallHintAnimation() { animationName = name };
        call.Invoke();
    }
}
