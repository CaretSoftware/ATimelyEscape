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

    private bool puzzleOneDone;
    private bool puzzleTwoDone;
    private bool puzzleThreeDone;
    private bool puzzleFourDone;
    private bool puzzleFiveDone;
    private bool cubeButtonOn;
    private bool ratButtonOn;
    private bool charging;

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

    }
    private void Update()
    {
        if (cubeButtonOn && ratButtonOn && !puzzleFourDone)
        {
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
                instructions.text = "Good";
                Invoke("Step2", 3.5f);
                //StartCoroutine(Delay());
                bigHatchAnim.SetBool("Open", true);
                step1Anim.SetBool("Open", true);
                Debug.Log("STEP1");
            }
            if (e.from == TimeTravelPeriod.Past && e.to == TimeTravelPeriod.Present && puzzleOneDone && !puzzleTwoDone)
            {
                instructions.text = " Good.";
                signMr.material = done;
                Invoke("Step4", 3.5f);
                //StartCoroutine(Delay());
                Debug.Log("STEP3");
            }
            if (e.from == TimeTravelPeriod.Present && e.to == TimeTravelPeriod.Past && puzzleTwoDone && !puzzleThreeDone)
            {
                instructions.text = " Good.";
                puzzleThreeDone = true;
                signMr.material = done;
                Invoke("Step5", 2f);
                Invoke("Step6", 5.5f);
                //StartCoroutine(Delay());
                Debug.Log("STEP5");
            }
            if (e.from == TimeTravelPeriod.Past && e.to == TimeTravelPeriod.Present && charging && !puzzleFiveDone)
            {
                signMr.material = done;
                instructions.text = "Good. All Done";
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
        signMr.material = notDone;
        instructions.text = " Push the cube to change its position in both this time and the future. " +
            //skapa ett pushcube event här:
            "Then press Y-button to travel one year ahead in time to and see that the cube is in the same spot";
        puzzleFloor.SetActive(false);
        bigHatchAnim.SetBool("Open", false);
        Debug.Log("STEP2");
        puzzleOneDone = true;
    }
    private void Step4()
    {
        signMr.material = notDone;
        instructions.text = " Now push the cube again." +
            //cubepushevent
            "Then use the X-button to travel back in time to see that the cube is in its original spot in this time";
        Debug.Log("STEP4");
        puzzleTwoDone = true;
    }
    private void Step5()
    {
        puzzleFloor.SetActive(true);
        bigHatchAnim.SetBool("OpenAgain", true);
        step1Anim.SetBool("Open", false);
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
        instructions.text = " To the left is a cubebutton that interacts with cubes. +" +
            "To the right is a button that you can interact with";
    }
    private void Step8()
    {
        sign.SetActive(true);
        sign2.SetActive(false);
        sign3.SetActive(false);
        puzzleFloor.SetActive(true);
        signMr.material = notDone;
        bigHatchAnim.SetBool("OpenThird", true);
        step2Anim.SetBool("Open", false);
        Invoke("Step9", 3.5f);
        //StartCoroutine(Delay());
        Debug.Log("STEP8");
    }
    private void Step9()
    {
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
        instructions.text = "This is a charger. You can charge a future cube by putting the past cube on the charger";
        Debug.Log("STEP10");
    }

    public void PickUp()
    {
        instructions.text = "Good. Now Use <sprite name=\"X\" color=#55FF55FF> to time travel one year to the past";
        Debug.Log("STEP0");
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
    public void DontCharge()
    {
        charging = false;
        Debug.Log("StoppedCharge");
    }
    public void IsOn()
    {
        isON = true;
    }

}


