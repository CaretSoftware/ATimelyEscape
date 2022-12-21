using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TimerScript : MonoBehaviour
{
    [SerializeField] private float timer; 
    [SerializeField] private TextMeshProUGUI[] timerTexts;
    [SerializeField] private Transform checkpoint; 
    private float timeLeft;
    private bool firstOn;
    private bool secondOn;

    private void Start()
    {
        timeLeft = timer;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (firstOn && secondOn)
        {
            if (timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
                UpdateTimer(timeLeft);
            }
            else
            {
                timeLeft = 0;
                firstOn = false;
                secondOn = false;
                Invoke("RestartTimer", 1);
                
                FailStateScript.Instance.PlayDeathVisualization(checkpoint.transform, transform);
            }
        }
    }

    private void UpdateTimer(float currentTime)
    {
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);
  
        timerTexts[0].text = string.Format("{0:00}:{1:00}", minutes, seconds);
        timerTexts[1].text = string.Format("{0:00}:{1:00}", minutes, seconds);
        timerTexts[2].text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    private void RestartTimer()
    {
        timeLeft = timer;
        firstOn = true;
        secondOn = true;
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
        SceneManager.LoadScene(0);
    }
}
