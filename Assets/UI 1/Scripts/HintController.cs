using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RatCharacterController;

public class HintController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private CharacterInput characterInput;

    private FadeScript fadeScript;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        fadeScript = GetComponent<FadeScript>();
    }

    private void FixedUpdate()
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
            BeNeutral();
        }
    }

    private void ShowLeftMouseClick(string context)
    {
        text.text = "Interact " + context;
        animator.Play("LeftClick");
    }

    private void ShowRightMouseClick(string context)
    {
        text.text = "Interact " + context;
        animator.Play("RightClick");
    }

    private void ShowSpaceJump()
    {
        text.text = "Jump Up";
        animator.Play("SpaceJump");
    }

    private void ShowCameraMovement()
    {
        text.text = "Look Around";
        animator.Play("MoveCamera");
    }

    private void ShowKeyMovment()
    {
        text.text = "To Move";
        animator.Play("MoveAround");
    }

    private void BeNeutral()
    {
        fadeScript.FadeOut();
    }
}
