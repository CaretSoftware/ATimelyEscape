using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class DigitalClock : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI clockText;

    [Header("Timer Values")]
    [Range(0,60)]
    [SerializeField] private int seconds;
    [Range(0, 60)]
    [SerializeField] private int minutes;
    [Range(0, 50)]
    [SerializeField] private int hours;
/*    [Range(0, 365)]
    [SerializeField] private int days;
    [Range(0, 50)]
    [SerializeField] private int years;*/
    [SerializeField] private Color fontColor;

    private float currentSeconds;
    private int clockDefault;

    void Start()
    {
        clockText.color = fontColor;
        clockDefault = 0;
        clockDefault += (seconds + (minutes * 60) + (hours * 60 * 60));
                        //(days * 60 * 60 * 24) + (years * 60 * 60 * 24 * 365));
        currentSeconds = clockDefault;
    }

    void Update()
    {
        if((currentSeconds -= Time.deltaTime) <= 0)
        {
            TimeUp();
        }
        else
        {
            clockText.text = TimeSpan.FromSeconds(currentSeconds).ToString(@"hh\:mm\:ss");
        }
    }
    private void TimeUp()
    {
        clockText.text = "00:00:00";
    }
}
