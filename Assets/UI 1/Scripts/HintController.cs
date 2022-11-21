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
    private CharacterInput characterInput;

    private FadeScript fadeScript;

    private Animator animator;

    private void Start()
    {
        CallHintAnimation.AddListener<CallHintAnimation>(HintAnimationListener);

        animator = GetComponent<Animator>();
        fadeScript = GetComponent<FadeScript>();
        characterInput = FindObjectOfType<CharacterInput>();
    }

    private void Update()
    {
        //CallMamma mamy = new CallMamma() { animationName = "jump", context = "Jump Up", waitForTime = 3f};
        //mamy.Invoke();
        //CheckToShowJump();
    }

    private void CheckToShowJump() // Remove to fire off in relevent Classes-method like in CharacterInput
    {
        if (characterInput.LedgeAhead(out Vector3 hitPosition) && characterInput.Grounded())
        {
            ShowSpaceJump();
        }
        else
        {
            BeInvisible();
        }
    }

    private IEnumerator coroutine;

    private void HintAnimationListener(CallHintAnimation c)
    {
        BeVisible();

        if (animator.GetCurrentAnimatorStateInfo(0).IsName(c.animationName))
            return;

        context.text = c.context;
        animator.Play(c.animationName);

        if(coroutine != null)
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

    private void BeVisible()
    {
        fadeScript.FadeIn();
    }

    public void BeInvisible()
    {
        fadeScript.FadeOut();
    }

    private void ShowSpaceJump()
    {
        BeVisible();

        
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("SpaceJump"))
            return;

        context.text = "Jump Up";
        animator.Play("SpaceJump");
    }

    public void ShowWarningTimeTravel()
    {
        BeVisible();

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("TimeWarning"))
            return;

        context.text = "Object Blocks the Timetravel";
        animator.Play("TimeWarning");
    }

    private void ShowLeftMouseClick(string info)
    {
        BeVisible();

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("LeftClick"))
            return;

        context.text = "Interact " + info;
        animator.Play("LeftClick");
    }

    private void ShowRightMouseClick(string info)
    {
        BeVisible();

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("RightClick"))
            return;

        context.text = "Interact " + info;
        animator.Play("RightClick");   
    }

    private void ShowCameraMovement()
    {
        BeVisible();

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("MoveCamera"))
            return;

        context.text = "Look Around";
        animator.Play("MoveCamera");
    }

    private void ShowKeyMovement()
    {
        BeVisible();

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("MoveAround"))
            return;

        context.text = "To Move";
        animator.Play("MoveAround");
    }
}
