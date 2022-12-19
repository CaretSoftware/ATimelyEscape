using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FailStateScript : MonoBehaviour
{
    private static FailStateScript _instance;
    public static FailStateScript Instance
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
        player = GameObject.Find("Player/Rat Mesh").transform;
        hyperDriveAnimator = GetComponent<Animator>();
        imageFunctionality = GameObject.Find("FailStateCanvas/BlackScreen").GetComponent<ImageFadeFunctions>();
    }

    //Call on when player dies.
    public void PlayDeathVisualization(Transform checkpoint)
    {
        print($"animations triggered");
        this.checkpoint = checkpoint;
        hyperDriveAnimator.gameObject.SetActive(true);
        hyperDriveAnimator.SetTrigger("DeathAnimTrigger");
    }

    public void FadeToBlack() { imageFunctionality.RunFadeToBlack(); }
    public void FadeToWhite() { imageFunctionality.RunFadeToWhite(); }

    public void FadeBack()
    {
        //player.gameObject.SetActive(false);
        player.position = checkpoint.position;
        //player.gameObject.SetActive(true);
        imageFunctionality.RunFadeBack();
        hyperDriveAnimator.gameObject.SetActive(false);
    }
}
