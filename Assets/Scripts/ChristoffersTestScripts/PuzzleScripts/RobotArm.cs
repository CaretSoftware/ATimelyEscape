using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotArm : MonoBehaviour
{
    [SerializeField] private Animator animatorParent;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            animatorParent.SetBool("On", true);
        }
    }
    public void DoAnimation()
    {
        animator.SetBool("On", true);
    }
}
