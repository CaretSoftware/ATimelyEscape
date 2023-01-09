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
    private ImageFadeFunctions imageFunctionality;
    
    public Animator Animator {get { return animator; } }

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
        imageFunctionality = GameObject.Find("FailStateCanvas/BlackScreen").GetComponent<ImageFadeFunctions>();
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
        imageFunctionality.RunFadeToBlack(); 
        animator.SetBool("DeathAnimationIsPlaying", true);
    }
    public void FadeToWhite() { imageFunctionality.RunFadeToWhite(); }

    public void FadeBack()
    {
        NewRatCharacterController.NewRatCharacterController.caughtEvent?.Invoke(false);
        player.position = checkpoint.position;
        imageFunctionality.RunFadeBack();
        animator.gameObject.SetActive(false);
        animator.SetBool("DeathAnimationIsPlaying", false);
    }
}
