using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintController : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ShowLeftMouseClick()
    {
        animator.Play("LeftClick");
    }

    public void ShowRightMouseClick()
    {
        animator.Play("RightClick");
    }

    public void ShowSpaceJump()
    {
        animator.Play("SpaceJump");
    }

    public void BeNeutral()
    {
        animator.Play("Neutral");
    }
}
