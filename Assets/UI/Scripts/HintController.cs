using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RatCharacterController;
using CallbackSystem;

public class HintController : MonoBehaviour
{
    //[SerializeField] private TextMeshProUGUI context;

    private FadeScript fadeScript;
    private Animator animator;
    private IEnumerator coroutine;

    private void Start()
    {
        CallHintAnimation.AddListener<CallHintAnimation>(UIHintListener);

        animator = GetComponent<Animator>();
        fadeScript = GetComponent<FadeScript>();
        //characterInput = FindObjectOfType<CharacterInput>();
    }

    // Nden 
    //CallMamma mamy = new CallMamma() { animationName = "jump",  waitForTime = 3f};
    //mamy.Invoke();

    private void UIHintListener(CallHintAnimation c)
    {
        BeVisible();
        animator.Play(c.animationName);
    }

    private void BeVisible()
    {
        fadeScript.FadeIn();
    }

    private void BeInvisible()
    {
        fadeScript.FadeOut();
    }
}
