using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotPunch : MonoBehaviour {
    [SerializeField] private GameObject glassWall;
    [SerializeField] private GameObject notCrackedglassWall;
    private Animator animator;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start() {
        audioSource = GetComponent<AudioSource>();
        glassWall.SetActive(false);
        animator = GetComponent<Animator>();
    }

    public void IsOn() {
        animator.SetBool("On", true);
        Invoke("CrackGlass", 1.15f);
    }
    private void CrackGlass()
    {
        audioSource.Play();
        glassWall.SetActive(true);
        notCrackedglassWall.SetActive(false);
    }
}
