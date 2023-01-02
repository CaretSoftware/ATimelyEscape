using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconSwitchHandler : MonoBehaviour
{
    [SerializeField] private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ChangeAnimatorCharge(bool isCharged)
    {
        if (isCharged)
            animator.SetTrigger("Light");
        else
            animator.SetTrigger("Heavy");
    }
}
