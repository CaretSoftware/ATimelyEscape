using System.Collections;
using System.Collections.Generic;
using CallbackSystem;
using TMPro;
using UnityEngine;

public class Incubator : MonoBehaviour {
    [SerializeField] private GameObject cubeManager;
    [SerializeField] private GameObject cubeManager2;
    [SerializeField] private GameObject cubeManager3;
    [SerializeField] private GameObject puzzleFloor;
    [SerializeField] private GameObject sign;
    [SerializeField] private GameObject bigHatch;
    [SerializeField] private GameObject smallHatch;
    [SerializeField] private GameObject cubePuzzle;
    [SerializeField] private GameObject platePuzzle;
    [SerializeField] private GameObject platePuzzle2;
    [SerializeField] private GameObject triggerRed;
    [SerializeField] private GameObject triggerRed2;
    [SerializeField] private GameObject triggerGreen;
    [SerializeField] private GameObject triggerGreen2;
    [SerializeField] private Light spotlight;
    [SerializeField] private Light spotlight2;
    [SerializeField] private Light spotlight3;
    [SerializeField] private Light spotlight4;
    [SerializeField] private Material notDone;
    [SerializeField] private Material done;
    [SerializeField] private TextMeshProUGUI instructions;
    private MeshRenderer signMr;
    private IncubateTrigger incubateTriggerRed;
    private Animator bigHatchAnim;
    private Animator cubePuzzleAnim;
    private Animator platePuzzleAnim;
    private Animator platePuzzle2Anim;
    private Animator triggerRedAnim;
    private Animator triggerGreenAnim;
    private Animator triggerGreen2Anim;
    private bool puzzleOneDone;
    public bool puzzleTwoHalfDone;
    public bool puzzleTwoDone;
    public bool puzzleThreeHalfDone;
    private bool puzzleThreeDone;
    private bool puzzleFourDone;
    public bool puzzleFiveDone;
    private bool puzzleFiveStarted;
    public bool PlateButtonInteractable { get; private set; }


    // Start is called before the first frame update
    void Start() {
        platePuzzle.SetActive(false);
        platePuzzle2.SetActive(false);
        cubeManager2.SetActive(false);
        spotlight.intensity = 0;
        spotlight2.intensity = 0;
        incubateTriggerRed = triggerRed.GetComponent<IncubateTrigger>();
        TimePeriodChanged.AddListener<TimePeriodChanged>(TimeTravel);
        signMr = sign.GetComponent<MeshRenderer>();
        bigHatchAnim = bigHatch.GetComponent<Animator>();
        cubePuzzleAnim = cubePuzzle.GetComponent<Animator>();
        platePuzzleAnim = platePuzzle.GetComponent<Animator>();
        platePuzzle2Anim = platePuzzle2.GetComponent<Animator>();
        triggerRedAnim = triggerRed.GetComponent<Animator>();
        triggerGreenAnim = triggerGreen.GetComponent<Animator>();
        triggerGreen2Anim = triggerGreen2.GetComponent<Animator>();
        signMr.material = notDone;
    }


    private void TimeTravel(TimePeriodChanged e) {
        if (e.from == TimeTravelPeriod.Present && e.to == TimeTravelPeriod.Past && !puzzleOneDone) {
            if (signMr) signMr.material = done;
            instructions.text = "GOOD";
            StartCoroutine(Delay());
            bigHatchAnim.SetBool("Open", true);
            cubePuzzleAnim.SetBool("Open", true);
            Debug.Log("STEP1");

        }
        if (e.from == TimeTravelPeriod.Past && e.to == TimeTravelPeriod.Present && puzzleOneDone) {
            if (!puzzleTwoHalfDone) {
                instructions.text = "PUSH THE CUBE TO THE RED LIGHT";
                triggerRedAnim.SetBool("Open", true);
                Debug.Log("STEP3");
            }
            if (puzzleTwoHalfDone) {
                instructions.text = "MOVE THIS VERSION OF THE CUBE TO THE GREEN LIGHT";
                spotlight.intensity = 0;
                spotlight2.intensity = 5;
                spotlight3.intensity = 5;
                Debug.Log("STEP9");
                triggerGreen2Anim.SetBool("Open", true);
            }
        }
        if (e.from == TimeTravelPeriod.Present && e.to == TimeTravelPeriod.Past && incubateTriggerRed.twoHalfDone && !puzzleThreeHalfDone) {
            triggerGreenAnim.SetBool("Open", true);
            instructions.text = "PUSH THIS PAST VERSION OF THE SAME CUBE TO THE GREEN LIGHT TO CHANGE THE DESTINY OF THE FUTURE VERSION OF THE CUBE";
            spotlight.intensity = 0;
            spotlight2.intensity = 5;
            Debug.Log("STEP5");
            OnboardingHandler.TimeTravelDiscovered = true;

        }

        if (e.from == TimeTravelPeriod.Present && e.to == TimeTravelPeriod.Past && puzzleThreeHalfDone) {
            spotlight.intensity = 0;
            spotlight2.intensity = 0;
            spotlight3.intensity = 0;
            spotlight4.intensity = 0;
            signMr.material = done;
            instructions.text = "GOOD";
            puzzleThreeDone = true;
            Invoke("PuzzleTwoDone", 1);
            Debug.Log("STEP11");
        }

    }

    private void OnDestroy() { if (EventSystem.Current != null) TimePeriodChanged.RemoveListener<TimePeriodChanged>(TimeTravel); }

    public IEnumerator Delay() {
        yield return new WaitForSeconds(3.2f);
        if (!puzzleOneDone) {
            signMr.material = notDone;
            instructions.text = "NOW PRESS \"2\"-KEY TO TIMETRAVEL ONE YEAR AHEAD";
            Invoke("SpotlightON", 2f);
            puzzleOneDone = true;
            puzzleFloor.SetActive(false);
            bigHatchAnim.SetBool("Open", false);
            Debug.Log("STEP2");

        }
        if (puzzleThreeDone && !puzzleFourDone) {
            cubeManager.SetActive(false);
            cubeManager2.SetActive(true);
            puzzleFourDone = true;
            bigHatchAnim.SetBool("OpenAgain", true);
            platePuzzleAnim.SetBool("Open", true);
            PlateButtonInteractable = true;

            StartCoroutine(Delay());
            Debug.Log("STEP12");

        }
        if (puzzleFourDone && !puzzleFiveDone) {
            puzzleFloor.SetActive(true);
            signMr.material = notDone;
            instructions.text = "PUSH THE CUBE TO THE BUTTON";
            Debug.Log("STEP13");
            OnboardingHandler.CubeChargeDiscovered = true; 
        }
        if (puzzleFiveDone && !puzzleFiveStarted) {
            cubeManager3.SetActive(true);
            bigHatchAnim.SetBool("OpenThird", true);
            platePuzzle2Anim.SetBool("Open", true);
            puzzleFiveStarted = true;
            Debug.Log("STEP15");
        }
        if (puzzleFiveStarted) {
            instructions.text = "SOLVE THE PUZZLE";
            signMr.material = notDone;
        }


    }

    public void SpotlightON() {
        spotlight.intensity = 5;
    }
    private void PuzzleTwoDone() {
        StartCoroutine(Delay());
        platePuzzle.SetActive(true);
        puzzleFloor.SetActive(true);
        bigHatchAnim.SetBool("Open", true);
        cubePuzzleAnim.SetBool("Open", false);
    }
    private void RemoveFloor() {
        puzzleFloor.SetActive(false);
    }

}
