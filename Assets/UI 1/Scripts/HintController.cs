using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RatCharacterController;

public class HintController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI context;

    [Header("Player")]
    [SerializeField] private CharacterInput characterInput;

    private FadeScript fadeScript;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        fadeScript = GetComponent<FadeScript>();

    }

    private void Update()
    {
        CheckToShowJump();
    }

    private void CheckToShowJump()
    {
        if (characterInput.LedgeAhead(out Vector3 hitPosition) && characterInput.Grounded())
        {
            fadeScript.FadeIn();

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("SpaceJump"))
                return;

            ShowSpaceJump();
        }
        else
        {
            fadeScript.FadeOut();
        }
    }

    public void ShowWarningTimeTravel()
    {
        context.text = "Object Blocks the Timetravel";
        animator.Play("TimeWarning");
    }

    private void ShowLeftMouseClick(string info)
    {
        context.text = "Interact " + info;
        animator.Play("LeftClick");
    }

    private void ShowRightMouseClick(string info)
    {
        context.text = "Interact " + info;
        animator.Play("RightClick");
        
    }

    private void ShowSpaceJump()
    {
        context.text = "Jump Up";
        animator.Play("SpaceJump");
    }

    private void ShowCameraMovement()
    {
        context.text = "Look Around";
        animator.Play("MoveCamera");
    }

    private void ShowKeyMovement()
    {
        context.text = "To Move";
        animator.Play("MoveAround");
    }

    public void BeNeutral()
    {
        fadeScript.FadeOut();
    }
}
