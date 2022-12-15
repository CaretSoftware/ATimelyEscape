using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDeathVisualization : MonoBehaviour
{
    private static PlayerDeathVisualization _instance;
    public static PlayerDeathVisualization Instance
    {
        get { return _instance; }
    }
    
    private Animator hyperDriveAnimator; //Place Gameobject with this script as child of main camera.
    private CanvasGroup blackScreenCanvasGroup; //Place Gameobject as child of Canvas.
    private Transform checkpoint;
    private Transform player;
    private ImageFadeFunctions imageFunctionality;


    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }

    private void Start()
    {
        player = GameObject.Find("Player").transform;
        hyperDriveAnimator = GetComponent<Animator>();
        imageFunctionality = GameObject.Find("Canvas/BlackScreen").GetComponent<ImageFadeFunctions>();
    }

    //Call on when player dies.
    public void PlayDeathVisualization(Transform checkpoint)
    {
        this.checkpoint = checkpoint;
        hyperDriveAnimator.gameObject.SetActive(true);
        hyperDriveAnimator.SetTrigger("DeathAnimTrigger");
    }

    public void FadeToBlack() { imageFunctionality.RunFadeToBlack(); }
    public void FadeToWhite() { imageFunctionality.RunFadeToWhite(); }

    public void FadeBack()
    {
        player.position = checkpoint.position;
        imageFunctionality.RunFadeBack();
        hyperDriveAnimator.ResetTrigger("DeathAnimTrigger");
        hyperDriveAnimator.gameObject.SetActive(false);
    }
}
