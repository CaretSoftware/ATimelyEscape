using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IncubateTrigger : MonoBehaviour
{
    [SerializeField] private GameObject triggerRed;
    [SerializeField] private GameObject triggerGreen;
    [SerializeField] private GameObject sign;
    [SerializeField] private TextMeshProUGUI instructions;
    [SerializeField] private bool isRed;
    [SerializeField] private bool isGreen;
    [SerializeField] private Material done;
    [SerializeField] private Material notDone;
    [SerializeField] private Light spotlight;
    [SerializeField] private Light spotlight2;
    private MeshRenderer mr;
    private IncubateTrigger incubateTriggerRed;
    private IncubateTrigger incubateTriggerGreen;
    public bool twoHalfDone;
    public bool puzzleTwoDone;
    public bool threeHalfDone;
    private void Start()
    {
        spotlight.intensity = 0;
        spotlight2.intensity = 0;
        mr = sign.GetComponent<MeshRenderer>();
        incubateTriggerGreen = triggerGreen.GetComponent<IncubateTrigger>();
        incubateTriggerRed = triggerRed.GetComponent<IncubateTrigger>();

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Cube")
        {
            if (isRed && !puzzleTwoDone)
            {
                instructions.text = "GOOD, NOW PRESS \"1\"-KEY TO TIMETRAVEL BACK ONE YEAR AGAIN";
                twoHalfDone = true;
               
            }
            if (isGreen && !puzzleTwoDone)
            {
                mr.material = done;
                instructions.text = "GOOD";
                StartCoroutine(Delay());
                spotlight.intensity = 0;
                spotlight2.intensity = 0;
            }
            if(isRed && puzzleTwoDone)
            {
                instructions.text = "GOOD, NOW PRESS \"2\"-KEY TO TIMETRAVEL ONE YEAR AHEAD";

            }
            if (isGreen && puzzleTwoDone)
            {
                instructions.text = "GOOD, NOW PRESS \"1\"-KEY TO TIMETRAVEL ONE YEAR BACK AGAIN TO SEE THAT THE PAST VERSION OF THE CUBE IS IN ITS ORIGINAL PLACE";
                threeHalfDone = true; 
            }
        }
    }
    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(2f);

            mr.material = notDone;
            instructions.text = "PUSH THE CUBE TO THE RED LIGHT";
            spotlight.intensity = 5;
            puzzleTwoDone = true;
            incubateTriggerRed.puzzleTwoDone = true; 

    }
}
