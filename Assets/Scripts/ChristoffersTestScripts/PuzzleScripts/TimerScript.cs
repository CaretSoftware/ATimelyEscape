using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerScript : MonoBehaviour
{
    [SerializeField] private float timeLeft;
    [SerializeField] private TextMeshProUGUI timerText;
    private bool firstOn;
    private bool secondOn;

    // Update is called once per frame
    void Update()
    {
        if (firstOn && secondOn)
        {
            if(timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
                UpdateTimer(timeLeft);
            }
            else
            {
                timeLeft = 0;
                firstOn = false;
                secondOn = false;
                //Lägg in checkpointkoden
            }
        }
    }
    private void UpdateTimer(float currentTime)
    {
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void FirstButtonOn()
    {
        firstOn = true; 
    }
    public void SecondButtonOn()
    {
        secondOn = true;
    }

    public void FinnishGame()
    {
        //kod för sclutscen
    }
}
