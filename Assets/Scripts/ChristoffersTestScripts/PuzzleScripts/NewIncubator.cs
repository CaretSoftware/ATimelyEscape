using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CallbackSystem;
using TMPro;

public class NewIncubator : MonoBehaviour
{

    [SerializeField] private GameObject sign;
    [SerializeField] private GameObject sign2;
    [SerializeField] private GameObject sign3;
    [SerializeField] private GameObject bigHatch;
    [SerializeField] private GameObject smallHatch;
    [SerializeField] private GameObject candyFeeder;
    [SerializeField] private GameObject puzzleFloor;
    [SerializeField] private GameObject step1;
    [SerializeField] private GameObject step2;
    [SerializeField] private GameObject step3;
    [SerializeField] private Material done;
    [SerializeField] private Material notDone;
    [SerializeField] private TextMeshProUGUI instructions;

    private Animator bigHatchAnim;
    private Animator smallHatchAnim;
    private Animator candyFeederAnim;
    private Animator step1Anim;
    private Animator step2Anim;
    private Animator step3Anim;
    private MeshRenderer signMr;
    private MeshRenderer sign2Mr;
    private MeshRenderer sign3Mr;
    private AudioManager audioManager;
    private NewRatCharacterController.NewCharacterInput characterInput;

    private bool puzzleOneDone;
    private bool puzzleTwoDone;
    private bool puzzleThreeDone;
    private bool puzzleFourDone;
    private bool puzzleFiveDone;
    private bool cubeButtonOn;
    private bool ratButtonOn;
    private bool charging;
    private bool welcome;

    private bool isON;

    private void Start()
    {
        TimePeriodChanged.AddListener<TimePeriodChanged>(TimeTravel);
        step2.SetActive(false);
        step3.SetActive(false);
        sign2.SetActive(false);
        sign3.SetActive(false);

        bigHatchAnim = bigHatch.GetComponent<Animator>();
        smallHatchAnim = smallHatch.GetComponent<Animator>();
        candyFeederAnim = candyFeeder.GetComponent<Animator>();
        step1Anim = step1.GetComponent<Animator>();
        step2Anim = step2.GetComponent<Animator>();
        step3Anim = step3.GetComponent<Animator>();
        signMr = sign.GetComponent<MeshRenderer>();
        sign2Mr = sign2.GetComponent<MeshRenderer>();
        sign3Mr = sign3.GetComponent<MeshRenderer>();
        characterInput = FindObjectOfType<NewRatCharacterController.NewCharacterInput>();
        audioManager = FindObjectOfType<AudioManager>();

    }
    private void Update()
    {
        if (cubeButtonOn && ratButtonOn && !puzzleFourDone)
        {
            instructions.text = "Good.";
            audioManager.Play("3");
            Step8();
            puzzleFourDone = true;
            Debug.Log("STEP7");
        }

    }

    private void OnDestroy() { if (EventSystem.Current != null) TimePeriodChanged.RemoveListener<TimePeriodChanged>(TimeTravel); }
    private void TimeTravel(TimePeriodChanged e)
    {
        if (isON)
        {
            if (e.from == TimeTravelPeriod.Present && e.to == TimeTravelPeriod.Past && !puzzleOneDone)
            {
                if (signMr) signMr.material = done;
                audioManager.Play("3");
                instructions.text = "Good";
                Invoke("Step2", 3.5f);
                //StartCoroutine(Delay());
                bigHatchAnim.SetBool("Open", true);
                step1Anim.SetBool("Open", true);
                Debug.Log("STEP1");
            }
            if (e.from == TimeTravelPeriod.Past && e.to == TimeTravelPeriod.Present && puzzleOneDone && !puzzleTwoDone)
            {
                instructions.text = "Good.";
                audioManager.Play("3");
                signMr.material = done;
                Invoke("Step4", 3.5f);
                //StartCoroutine(Delay());
                Debug.Log("STEP3");
            }
            if (e.from == TimeTravelPeriod.Present && e.to == TimeTravelPeriod.Past && puzzleTwoDone && !puzzleThreeDone)
            {
                characterInput.CanTimeTravelPresent = true; 
                instructions.text = "You see?";
                audioManager.Play("11");
                puzzleThreeDone = true;
                signMr.material = done;
                Invoke("Step5", 2f);
                //StartCoroutine(Delay());
                Debug.Log("STEP5");
            }
            if (e.from == TimeTravelPeriod.Past && e.to == TimeTravelPeriod.Present && charging && !puzzleFiveDone)
            {
                signMr.material = done;
                audioManager.Play("10");
                instructions.text = "Good. All Done. Have some Candy";
                smallHatchAnim.SetBool("Open", true);
                candyFeederAnim.SetBool("Open", true);
                puzzleFiveDone = true;
                isON = false;
                Debug.Log("STEP11");
            }
        }
        if (e.from == TimeTravelPeriod.Present && e.to == TimeTravelPeriod.Past && puzzleFiveDone)
        {
            signMr.material = notDone;
        }


    }
    private void Step2()
    {
        characterInput.CanTimeTravelPast = false;
        signMr.material = notDone;
        audioManager.Play("4");
        instructions.text = "Moving a cube changes its destiny in the future. Move the cube.";
        puzzleFloor.SetActive(false);
        bigHatchAnim.SetBool("Open", false);
        Debug.Log("STEP2");
        Invoke("Step2AndHalf", 8f);
    }
    private void Step2AndHalf()
    {
        audioManager.Play("5");
        instructions.text = "Timetravel forward in time <sprite name=\"Y\">";
        characterInput.CanTimeTravelPresent = true; 
        puzzleOneDone = true;
    }
    private void Step4()
    {
        characterInput.CanTimeTravelPresent = false;
        signMr.material = notDone;
        audioManager.Play("6");
        instructions.text = "Move the cube in this time.";
        Debug.Log("STEP4");
        Invoke("Step4AndHalf", 8f);
    }
    private void Step4AndHalf()
    {
        audioManager.Play("7");
        instructions.text = "Travel back in time. Observe the cube travelling back in time to its previous position <sprite name=\"X\">";
        Invoke("Step4TwoThirds", 5);
        
    }
    private void Step4TwoThirds()
    {
        characterInput.CanTimeTravelPast = true;
        puzzleTwoDone = true;

    }
    private void Step5()
    {
        puzzleFloor.SetActive(true);
        bigHatchAnim.SetBool("OpenAgain", true);
        step1Anim.SetBool("Open", false);
        Invoke("Step6", 3.5f);
    }
    private void Step6()
    {
        signMr.material = notDone;
        step1.SetActive(false);
        step2.SetActive(true);
        bigHatchAnim.SetBool("OpenFourth", true);
        step2Anim.SetBool("Open", true);
        Invoke("InstructionsStep8", 3.5f);
        sign.SetActive(false);
        sign2.SetActive(true);
        sign3.SetActive(true);
        Debug.Log("STEP6");
    }
    private void InstructionsStep8()
    {
        puzzleFloor.SetActive(false);
        audioManager.Play("9");
        instructions.text = " Some buttons react to cubes. Some buttons must be manually pressed";
    }
    private void Step8()
    {
        puzzleFloor.SetActive(true);
        bigHatchAnim.SetBool("OpenThird", true);
        step2Anim.SetBool("Open", false);
        Invoke("Step9", 3.5f);
        //StartCoroutine(Delay());
        Debug.Log("STEP8");
    }
    private void Step9()
    {
        sign.SetActive(true);
        signMr.material = notDone;
        sign2.SetActive(false);
        sign3.SetActive(false);
        step3.SetActive(true);
        step2.SetActive(false);
        bigHatchAnim.SetBool("OpenLast", true);
        step3Anim.SetBool("Open", true);
        Invoke("Step10Instructions", 3.5f);
        Debug.Log("STEP9");
    }
    private void Step10Instructions()
    {
        puzzleFloor.SetActive(false);
        audioManager.Play("8");
        instructions.text = "Charge this cube on the chargepad. It holds charge for a long time";
        Debug.Log("STEP10");
    }

    public void CubeButton()
    {
        cubeButtonOn = true;
        sign2Mr.material = done;
        Debug.Log("CUBEON");
    }
    public void RatButton()
    {
        ratButtonOn = true;
        sign3Mr.material = done;
        Debug.Log("RATON");
    }
    public void Charging()
    {
        charging = true;
        Debug.Log("Charging");
    }
    /*    public void DontCharge()
        {
            charging = false;
            Debug.Log("StoppedCharge");
        }*/

    public void StartText()
    {
        if (!welcome)
        {
            audioManager.Play("1");
            instructions.text = "Subject R@, welcome! Please step forward";
            welcome = true;
        }
    }
    public void IsOn()
    {
        isON = true;
    }

}


