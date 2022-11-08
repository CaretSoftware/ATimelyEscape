using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DigitalClock : MonoBehaviour
{
    [SerializeField] GameObject textDisplay;
    [SerializeField] bool isFutureTimer;
    private bool takingAway = false;
    private TextMeshProUGUI textMeshPro;

    public int secondsLeft;
    public bool isOn;
    public bool futureDone; 
    void Start()
    {
        futureDone = false;
        isOn = false; 
        textMeshPro = textDisplay.GetComponent<TextMeshProUGUI>();
        textMeshPro.text = "" + secondsLeft;
    }
    private void Update()
    {
        if(!takingAway && secondsLeft > 0 && isOn)
        {
            StartCoroutine(TimerTake());
        }
    }

    IEnumerator TimerTake()
    {
        takingAway = true;
        yield return new WaitForSeconds(1);
        secondsLeft -= 1;
        textMeshPro.text = "" + secondsLeft;
        takingAway = false;
        if(secondsLeft <= 0 && isFutureTimer)
        {
            futureDone = true;
        }

    }
}
