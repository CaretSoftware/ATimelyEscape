using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IncubateTrigger : MonoBehaviour
{
    [SerializeField] private GameObject puzzle;
    [SerializeField] private GameObject triggerRed;
    [SerializeField] private GameObject triggerGreen;
    [SerializeField] private GameObject triggerRed2;
    [SerializeField] private GameObject triggerGreen2;
    [SerializeField] private GameObject sign;
    [SerializeField] private TextMeshProUGUI instructions;
    [SerializeField] private bool isRed;
    [SerializeField] private bool isGreen;
    [SerializeField] private bool isRed2;
    [SerializeField] private bool isGreen2;
    [SerializeField] private Material done;
    [SerializeField] private Material notDone;
    [SerializeField] private Light spotlight;
    [SerializeField] private Light spotlight2;
    private MeshRenderer mr;
    private IncubateTrigger incubateTriggerRed;
    private IncubateTrigger incubateTriggerGreen;
    private Incubator puzzleIncubator;
    private Animator triggerRedAnim;
    private Animator triggerGreenAnim;
    private Animator triggerRed2Anim;
    private Animator triggerGreen2Anim;
    public bool twoHalfDone;
    public bool puzzleTwoDone;
    private void Start()
    {
        spotlight.intensity = 0;
        spotlight2.intensity = 0;
        mr = sign.GetComponent<MeshRenderer>();
        incubateTriggerGreen = triggerGreen.GetComponent<IncubateTrigger>();
        incubateTriggerRed = triggerRed.GetComponent<IncubateTrigger>();
        puzzleIncubator = puzzle.GetComponent<Incubator>();
        triggerRedAnim = triggerRed.GetComponent<Animator>();
        triggerGreenAnim = triggerGreen.GetComponent<Animator>();
        triggerRed2Anim = triggerRed2.GetComponent<Animator>();
        triggerGreen2Anim = triggerGreen2.GetComponent<Animator>();

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Cube")
        {
            if (isRed && !puzzleTwoDone && !puzzleIncubator.puzzleTwoHalfDone)
            {
                instructions.text = "GOOD, NOW PRESS \"1\"-KEY TO TIMETRAVEL BACK ONE YEAR AGAIN";
                twoHalfDone = true;
                Debug.Log("STEP4");

            }
            if (isGreen && !puzzleTwoDone)
            {
                mr.material = done;
                instructions.text = "GOOD";
                StartCoroutine(Delay());
                spotlight.intensity = 0;
                spotlight2.intensity = 0;
                Debug.Log("STEP6");
            }
            if(isRed2)
            {
                instructions.text = "GOOD, NOW PRESS \"2\"-KEY TO TIMETRAVEL ONE YEAR AHEAD";
                puzzleIncubator.puzzleTwoHalfDone = true;
                Debug.Log("STEP8");
                triggerRed2Anim.SetBool("Open", false);

            }
            if (isGreen2)
            {
                instructions.text = "GOOD, NOW PRESS \"1\"-KEY TO TIMETRAVEL ONE YEAR BACK AGAIN TO SEE THAT THE PAST VERSION OF THE CUBE IS IN ITS ORIGINAL PLACE";
                puzzleIncubator.puzzleThreeHalfDone = true; 
                Debug.Log("STEP10");
                triggerGreen2Anim.SetBool("Open", true);
            }
        }
    }
    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(2f);

            mr.material = notDone;
            instructions.text = "NOW, PUSH THE CUBE TO THE RED LIGHT AGAIN";
            spotlight.intensity = 5;
            puzzleTwoDone = true;
            incubateTriggerRed.puzzleTwoDone = true;
            incubateTriggerGreen.puzzleTwoDone = true;
            triggerRedAnim.SetBool("Open", false);
            triggerGreenAnim.SetBool("Open", false);
            triggerRed2Anim.SetBool("Open", true);
            Debug.Log("STEP7");


    }
}
