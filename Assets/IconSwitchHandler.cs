using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconSwitchHandler : MonoBehaviour
{
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SetAnimatorBool(bool isCharged)
    {
        animator.SetBool("IsCharged", isCharged);
    }
}
