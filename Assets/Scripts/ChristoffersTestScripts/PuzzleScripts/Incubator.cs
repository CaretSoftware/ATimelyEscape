using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CallbackSystem;
using TMPro;

public class Incubator : MonoBehaviour
{
    [SerializeField] private GameObject sign;
    [SerializeField] private GameObject bigHatch;
    [SerializeField] private GameObject smallHatch;
    [SerializeField] private GameObject step2;
    [SerializeField] private Light spotlight;
    [SerializeField] private Material notDone;
    [SerializeField] private Material done;
    [SerializeField] private TextMeshProUGUI instructions;
    private MeshRenderer signMr;
    private bool puzzleOneDone; 

    // Start is called before the first frame update
    void Start()
    {
        spotlight.intensity = 0; 
        TimePeriodChanged.AddListener<TimePeriodChanged>(StepOne);
        TimePeriodChanged.AddListener<TimePeriodChanged>(StepTwo);
        signMr = sign.GetComponent<MeshRenderer>();
        signMr.material = notDone;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void StepOne(TimePeriodChanged e)
    {
        if (e.from == TimeTravelPeriod.Present && e.to == TimeTravelPeriod.Past && !puzzleOneDone)
        {
            signMr.material = done;
            instructions.text = "GOOD";
            StartCoroutine(Delay());
            bigHatch.GetComponent<Animator>().SetBool("Open", true);
            step2.GetComponent<Animator>().SetBool("Open", true);

        }
    }
    private void StepTwo(TimePeriodChanged e)
    {
        if (e.from == TimeTravelPeriod.Past && e.to == TimeTravelPeriod.Present && puzzleOneDone)
        {
            instructions.text = "PUSH THE CUBE HERE";
        }
    }
    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(3.2f);
        signMr.material = notDone;
        instructions.text = "NOW USE \"2\" TO TIMETRAVEL ONE YEAR AHEAD";
        spotlight.intensity = 1;
        puzzleOneDone = true;
    }
}
