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

    private CameraController cameraController;

    private FadeScript fadeScript;

    private Animator animator;

    private bool shownBasics;
    private bool hintsActive = true;

    private void Start()
    {
        animator = GetComponent<Animator>();
        fadeScript = GetComponent<FadeScript>();

        cameraController = CameraController.Instance;
    }

    private void Update()
    {
        CheckToShowJump();
    }

    private void CheckToShowJump()
    {
        if (characterInput.LedgeAhead(out Vector3 hitPosition) && characterInput.Grounded())
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("SpaceJump"))
                return;

            fadeScript.FadeIn();

            ShowSpaceJump();
        }
        else
        {
            fadeScript.FadeOut();
        }
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

    private void BeNeutral()
    {
        fadeScript.FadeOut();
    }
}
