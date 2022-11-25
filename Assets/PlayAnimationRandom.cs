using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimationRandom : MonoBehaviour
{
    private Animator animator;

    public bool isRandom = true;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        int random = Random.Range(0, 100);

        if(isRandom && random == 50)
            animator.Play("Fly");

        if (!isRandom)
        {
            animator.Play("Fly");
        }
    }
}
