using System;
using UnityEngine;

public class FailStateScript : MonoBehaviour
{
    private static FailStateScript _instance;
    public static FailStateScript Instance
    {
        get { return _instance; }
    }
    
    private Animator animator; //Place Gameobject with this script as child of main camera.
    private CanvasGroup blackScreenCanvasGroup; //Place Gameobject as child of Canvas.
    private Transform checkpoint;
    private Transform player;
    private ImageFadeFunctions blackScreenImage;
    private ImageFadeFunctions messageImage;

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }

    private void Start()
    {
        //TODO Gretas test scen 
        player = FindObjectOfType<NewRatCharacterController.NewRatCharacterController>().transform;
        animator = GetComponent<Animator>();
        blackScreenImage = GameObject.Find("FailStateCanvas/BlackScreen").GetComponent<ImageFadeFunctions>();
        messageImage = GameObject.Find("FailStateCanvas/MessageScreen").GetComponent<ImageFadeFunctions>();

    }

    //Call on when player dies.
    public void PlayDeathVisualization(Transform checkpoint, Transform goTransform = null)
    {
        if (goTransform.tag.Equals("Scientist"))
        {
            //OnboardingHandler.ScientistDiscovered = true;
        }

        else if (goTransform.tag.Equals("Roomba"))
        {
            //OnboardingHandler.VacuumCleanerDiscovered = true;  
        }

        NewRatCharacterController.NewRatCharacterController.caughtEvent?.Invoke(true);
        this.checkpoint = checkpoint;
        animator.gameObject.SetActive(true);
        animator.SetTrigger("DeathAnimTrigger");
    }

    public void FadeToBlack()
    {
        blackScreenImage.RunFadeToBlack();
    }

    public void FadeMessageIn()
    {
        messageImage.RunFadeIn();
    }

    public void FadeMessageOut()
    {
        messageImage.RunFadeOut();
    }

    public void FadeToWhite()
    {
        blackScreenImage.RunFadeToWhite();
    }

    public void FadeBack()
    {
        NewRatCharacterController.NewRatCharacterController.caughtEvent?.Invoke(false);
        player.position = checkpoint.position;
        blackScreenImage.RunFadeOut();
        animator.gameObject.SetActive(false);
    }
}
