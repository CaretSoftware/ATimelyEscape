using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HintController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ShowLeftMouseClick()
    {
        text.text = "Interact";
        animator.Play("LeftClick");
    }

    public void ShowRightMouseClick()
    {
        text.text = "Interact";
        animator.Play("RightClick");
    }

    public void ShowSpaceJump()
    {
        text.text = "To Jump";
        animator.Play("SpaceJump");
    }

    public void ShowCameraMovement()
    {
        text.text = "Look Around";
        animator.Play("MoveCamera");
    }

    public void ShowKeyMovment()
    {
        text.text = "To Move";
        animator.Play("MoveAround");
    }

    public void BeNeutral()
    {
        animator.Play("Neutral");
    }
}
