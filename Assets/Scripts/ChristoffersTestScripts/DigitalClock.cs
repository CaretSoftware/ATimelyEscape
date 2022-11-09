using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DigitalClock : MonoBehaviour
{
    [Header("Time Settings")]
    [SerializeField] private int seconds = 60;
    [SerializeField]private int Minutes = 60;
    [SerializeField]private int Hours = 60;
    [SerializeField]private int Days = 24;
    [SerializeField]private int Weeks = 7;
    [SerializeField]private int Months = 30;
    [SerializeField]private int Years = 365;
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
        textMeshPro.text = TimeToString.TimeAsString(secondsLeft);
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
        textMeshPro.text = TimeToString.TimeAsString(secondsLeft);
        takingAway = false;
        if(secondsLeft <= 0 && isFutureTimer)
        {
            futureDone = true;
        }

    }

    
}
