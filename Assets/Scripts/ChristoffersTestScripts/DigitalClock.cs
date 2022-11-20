using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DigitalClock : MonoBehaviour
{
    [Header("Time Settings")]
    [SerializeField] private int seconds;
    [SerializeField]private int minutes;
    [SerializeField]private int hours;
    [SerializeField]private int days;
    [SerializeField]private int weeks;
    [SerializeField]private int months;
    [SerializeField] private int years;

    [Header("Text Settings")]
    [SerializeField] GameObject textDisplay;
    [SerializeField] bool isFutureTimer;
    public bool takingAway;
    private TextMeshProUGUI textMeshPro;

    public int time;
    public bool isOn;
    public bool futureDone;
    void Start()
    {
        time = years * TimeToString.Year + months * TimeToString.Month + weeks * TimeToString.Week
                + days * TimeToString.Day + hours * TimeToString.Hour + minutes * TimeToString.Minute + seconds;
        futureDone = false;
        isOn = false; 
        textMeshPro = textDisplay.GetComponent<TextMeshProUGUI>();
        textMeshPro.text = TimeToString.TimeAsString(time);
    }
    private void Update()
    {
        if(!takingAway && time > 0 && isOn)
        {
            StartCoroutine(TimerTake());
        }
    }

    IEnumerator TimerTake()
    {
        takingAway = true;
        yield return new WaitForSeconds(1);
        time -= 1;
        textMeshPro.text = TimeToString.TimeAsString(time);
        takingAway = false;

        if(time <= 0 && isFutureTimer)
        {
            futureDone = true;
        }

    }

    
}
