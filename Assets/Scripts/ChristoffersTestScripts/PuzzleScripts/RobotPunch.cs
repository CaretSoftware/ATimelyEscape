using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotPunch : MonoBehaviour {
    [SerializeField] private GameObject glassWall;
    [SerializeField] private GameObject notCrackedglassWall;
    private Animator animator;
    private bool pastOn;
    private bool presentOn;
    private bool futureOn;

    // Start is called before the first frame update
    void Start() {
        glassWall.SetActive(false);
        animator = GetComponent<Animator>();
    }
    private void Update() {
        if (pastOn && presentOn && futureOn) {
            animator.SetBool("On", true);
            glassWall.SetActive(true);
            notCrackedglassWall.SetActive(false);
        }
    }

    public void PastIsOn() {
        pastOn = true;
    }
    public void PresentIsOn() {
        presentOn = true;
    }
    public void FutureIsOn() {
        futureOn = true;
    }

}
