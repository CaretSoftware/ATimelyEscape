using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoredBehaviour : StateMachineBehaviour
{
    private const float EndOfAnimationValue = .98f;
    private const float StartOfAnimationValue = 0.02f;
    private const float TransitionTime = 0.2f;

    [SerializeField] private float timeUntilBored;
    [SerializeField] private int numberOfBoredAnimations;

    private bool isBored;
    private float idleTime;
    private int boredAnimation;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ResetIdle();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!isBored)
        {
            idleTime += Time.deltaTime;
            //Check to make sure we're not in the middle of an animation.
            if (idleTime > timeUntilBored && stateInfo.normalizedTime % 1 < StartOfAnimationValue)
            {
                isBored = true;
                boredAnimation = Random.Range(1, numberOfBoredAnimations + 1);
                boredAnimation = boredAnimation * 2 - 1;

                animator.SetFloat("boredAnimation", boredAnimation - 1);
            }
        }
        else if(stateInfo.normalizedTime % 1 > EndOfAnimationValue)
        {
            ResetIdle();
        }
        animator.SetFloat("boredAnimation", boredAnimation, TransitionTime, Time.deltaTime);

    }

    private void ResetIdle()
    {
        if (isBored)
            boredAnimation--;
        isBored = false;
        idleTime = 0;

    }
}
