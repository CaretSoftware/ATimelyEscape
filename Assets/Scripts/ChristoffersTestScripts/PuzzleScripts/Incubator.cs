using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CallbackSystem;
using TMPro;

public class Incubator : MonoBehaviour
{
    [SerializeField] private GameObject puzzleOne;
    [SerializeField] private GameObject sign;
    [SerializeField] private GameObject bigHatch;
    [SerializeField] private GameObject smallHatch;
    [SerializeField] private GameObject cubePuzzle;
    [SerializeField] private GameObject PlatePuzzle;
    [SerializeField] private GameObject triggerRed;
    [SerializeField] private GameObject triggerGreen;
    [SerializeField] private Light spotlight;
    [SerializeField] private Light spotlight2;
    [SerializeField] private Material notDone;
    [SerializeField] private Material done;
    [SerializeField] private TextMeshProUGUI instructions;
    private MeshRenderer signMr;
    private IncubateTrigger incubateTrigger;
    private Animator bigHatchAnim;
    private Animator cubePuzzleAnim;
    private Animator platePuzzleAnim;
    private Animator triggerRedAnim;
    private Animator triggerGreenAnim;
    private bool puzzleOneDone;
    private bool puzzleTwoDone;
    private bool puzzleThreeDone; 

    // Start is called before the first frame update
    void Start()
    {
        spotlight.intensity = 0;
        spotlight2.intensity = 0;
        incubateTrigger = GetComponentInChildren<IncubateTrigger>();
        TimePeriodChanged.AddListener<TimePeriodChanged>(TimeTravel);
        signMr = sign.GetComponent<MeshRenderer>();
        bigHatchAnim = bigHatch.GetComponent<Animator>();
        cubePuzzleAnim = cubePuzzle.GetComponent<Animator>();
        platePuzzleAnim = PlatePuzzle.GetComponent<Animator>();
        triggerRedAnim = triggerRed.GetComponent<Animator>();
        triggerGreenAnim = triggerGreen.GetComponent<Animator>();
        signMr.material = notDone;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void TimeTravel(TimePeriodChanged e)
    {
        if (e.from == TimeTravelPeriod.Present && e.to == TimeTravelPeriod.Past && !puzzleOneDone)
        {
            signMr.material = done;
            instructions.text = "GOOD";
            StartCoroutine(Delay());
            bigHatchAnim.SetBool("Open", true);
            cubePuzzleAnim.SetBool("Open", true);

        }
        if (e.from == TimeTravelPeriod.Past && e.to == TimeTravelPeriod.Present && puzzleOneDone)
        {
            instructions.text = "PUSH THE CUBE TO THE RED LIGHT";
            triggerRedAnim.SetBool("Open", true);
        }
        if (e.from == TimeTravelPeriod.Present && e.to == TimeTravelPeriod.Past && incubateTrigger.twoHalfDone)
        {
            triggerGreenAnim.SetBool("Open", true);
            instructions.text = "PUSH THIS PAST VERSION OF THE SAME CUBE TO THE GREEN LIGHT TO CHANGE THE DETINY OF THE FUTURE VERSION OF THE CUBE";
            spotlight.intensity = 0;
            spotlight2.intensity = 5;

        }
        if (e.from == TimeTravelPeriod.Past && e.to == TimeTravelPeriod.Present && incubateTrigger.puzzleTwoDone)
        {
            instructions.text = "MOVE THIS VERSION OF THE CUBE TO THE GREEN LIGHT";
            spotlight.intensity = 0;
            spotlight2.intensity = 5;
            

        }
        if (e.from == TimeTravelPeriod.Present && e.to == TimeTravelPeriod.Past && incubateTrigger.threeHalfDone)
        {
            signMr.material = done;
            instructions.text = "GOOD";
            StartCoroutine(Delay());
            bigHatchAnim.SetBool("Open", true);
            cubePuzzleAnim.SetBool("Open", false);
        }

    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(3.2f);
        if (!puzzleOneDone)
        {
            signMr.material = notDone;
            instructions.text = "NOW PRESS \"2\"-KEY TO TIMETRAVEL ONE YEAR AHEAD";
            Invoke("SpotlightON", 2f);
            puzzleOneDone = true;
            puzzleOne.SetActive(false);

        }
        if(puzzleOneDone && puzzleTwoDone)
        {
            puzzleThreeDone = true;
            StartCoroutine(Delay());
            bigHatchAnim.SetBool("Open", true);
            platePuzzleAnim.SetBool("Open", true);
            triggerGreenAnim.SetBool("Open", false);
            triggerRedAnim.SetBool("Open", false);

        }
        if (puzzleOneDone && puzzleTwoDone && puzzleThreeDone)
        {
            signMr.material = notDone;
            instructions.text = "PUSH THE CUBE TO THE BUTTON";
        }
    }

/*    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Cube")
        {
            instructions.text = "GOOD, NOW PRESS \"1\"-KEY TO TIMETRAVEL BACK ONE YEAR AGAIN";
        }
    }*/
    public void SpotlightON()
    {
        spotlight.intensity = 5;
    }
}
