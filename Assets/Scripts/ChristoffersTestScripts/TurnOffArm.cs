using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffArm : MonoBehaviour
{
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ArmOff()
    {
        animator.SetBool("Off", true);
    }
}
