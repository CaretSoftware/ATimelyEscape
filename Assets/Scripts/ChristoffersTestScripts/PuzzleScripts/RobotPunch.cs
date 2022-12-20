using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotPunch : MonoBehaviour
{
    private Animator animator;
    private bool pastOn;
    private bool presentOn;
    private bool futureOn; 

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if(pastOn && presentOn && futureOn)
        {
            animator.SetBool("On", true);
        }
    }

    public void PastIsOn()
    {
        pastOn = true;
    }
    public void PresentIsOn()
    {
        presentOn = true;
    }
    public void FutureIsOn()
    {
        futureOn = true;
    }

}
