using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RatCharacterController;

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
        animator = GetComponent<Animator>();
        fadeScript = GetComponent<FadeScript>();
        characterInput = FindObjectOfType<CharacterInput>();
    }

    private void Update()
    {
        CheckToShowJump();
    }

    private void CheckToShowJump()
    {
        if (characterInput.LedgeAhead(out Vector3 hitPosition) && characterInput.Grounded())
        {
            ShowSpaceJump();
        }
        else
        {
            BeNeutral();
        }
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

    private void BeVisible()
    {
        fadeScript.FadeIn();
    }

    public void BeNeutral()
    {
        fadeScript.FadeOut();
    }
}
