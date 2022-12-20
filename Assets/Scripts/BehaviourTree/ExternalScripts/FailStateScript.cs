using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using CallbackSystem;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Event = CallbackSystem.Event;

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
        //player = GameObject.FindObjectOfType<NewCharacterController>().transform;
        hyperDriveAnimator = GetComponent<Animator>();
        imageFunctionality = GameObject.Find("FailStateCanvas/BlackScreen").GetComponent<ImageFadeFunctions>();
    }

    //Call on when player dies.
    public void PlayDeathVisualization(Transform checkpoint, Transform goTransform = null)
    {
        if(goTransform.tag.Equals("Scientist")){}
            //Play Scientist Event
        else if(goTransform.tag.Equals("Roomba")){}
            //Play Roomba Event
            
        //TODO @August Unsubscribe from events
        
        // catch the rat
        //NewCharacterController.caughtEvent?.Invoke(true);
        this.checkpoint = checkpoint;
        hyperDriveAnimator.gameObject.SetActive(true);
        hyperDriveAnimator.SetTrigger("DeathAnimTrigger");
    }

    public void FadeToBlack() { imageFunctionality.RunFadeToBlack(); }
    public void FadeToWhite() { imageFunctionality.RunFadeToWhite(); }

    public void FadeBack()
    {
        player.position = checkpoint.position;
        // release the rat
        //NewCharacterController.caughtEvent?.Invoke(false);
        imageFunctionality.RunFadeBack();
        hyperDriveAnimator.gameObject.SetActive(false);
    }
}
