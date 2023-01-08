using UnityEngine;

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
        //TODO Gretas test scen 
        player = FindObjectOfType<NewRatCharacterController.NewRatCharacterController>().transform;
        hyperDriveAnimator = GetComponent<Animator>();
        imageFunctionality = GameObject.Find("FailStateCanvas/BlackScreen").GetComponent<ImageFadeFunctions>();
    }

    //Call on when player dies.
    public void PlayDeathVisualization(Transform checkpoint, Transform goTransform = null)
    {
        if (goTransform.tag.Equals("Scientist"))
        {
             OnboardingHandler.ScientistDiscovered = true;
        }

        else if (goTransform.tag.Equals("Roomba"))
        {
            OnboardingHandler.VacuumCleanerDiscovered = true;  
        }

        NewRatCharacterController.NewRatCharacterController.caughtEvent?.Invoke(true);
        this.checkpoint = checkpoint;
        hyperDriveAnimator.gameObject.SetActive(true);
        hyperDriveAnimator.SetTrigger("DeathAnimTrigger");
    }

    public void FadeToBlack() { imageFunctionality.RunFadeToBlack(); }
    public void FadeToWhite() { imageFunctionality.RunFadeToWhite(); }

    public void FadeBack()
    {
        player.position = checkpoint.position;
        NewRatCharacterController.NewRatCharacterController.caughtEvent?.Invoke(false);
        imageFunctionality.RunFadeBack();
        hyperDriveAnimator.gameObject.SetActive(false);
    }
}
