using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RatCharacterController;
using CallbackSystem;

public class HintController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI context;

    // [Header("Player")]
    // [SerializeField] 
    //private CharacterInput characterInput;

    private FadeScript fadeScript;

    private Animator animator;

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


    private IEnumerator coroutine;

    private void UIHintListener(CallHintAnimation c)
    {
        BeVisible();

        animator.Play(c.animationName);

        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        coroutine = ShowFor(c.waitForTime);
        StartCoroutine(coroutine);
    }

    private IEnumerator ShowFor(float time)
    {
        yield return new WaitForSeconds(time);

        BeInvisible();
    }

    private void ChangeContext(string text)
    {
        context.text = text;
    }

    private void BeVisible()
    {
        fadeScript.FadeIn();
    }

    public void BeInvisible()
    {
        fadeScript.FadeOut();
    }
}
