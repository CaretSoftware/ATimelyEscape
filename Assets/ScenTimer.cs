using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class ScenTimer : MonoBehaviour
{
    [SerializeField] private RuntimeSceneManager runtimeSceneManager;

    private float timer;
    private float previusTime;

    private int currentScen;
    private int previusScen;

    private int hours;
    private int minutes;
    private int seconds;
    private int milliseconds;

    private string fileName = "Assets/TestData/TimeInScens";

    StreamWriter writer;

    private void Start()
    {
        timer = 0f; previusTime = 0f;
        currentScen = runtimeSceneManager.GetCurrentSceneIndex();
        previusScen = currentScen;
        int counter = 0;
        while (File.Exists(fileName + string.Format("{0:D2}", counter) + ".txt"))
        {
            counter++;
        }
        StringBuilder temfil = new StringBuilder();
        temfil.Append(fileName).Append(string.Format("{0:D2}", counter)).Append(".txt");
        fileName = temfil.ToString();

        writer = new StreamWriter(fileName);

        writer.Write(DataStart());

        writer.Close(); 

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.unscaledDeltaTime;
        currentScen = runtimeSceneManager.GetCurrentSceneIndex();
        if(currentScen != previusScen)
        {
            previusScen = currentScen;
            writer = File.AppendText(fileName);

            writer.WriteLine(DataEnd());

            writer.Write(DataStart());
            writer.Close();

            previusTime = timer;
        }
    }

    private void OnDisable()
    {
        writer = File.AppendText(fileName);

        writer.WriteLine(DataEnd());

    }

    private StringBuilder DataStart()
    {
        return new StringBuilder().Append(currentScen).Append(": ").Append(TimeToString(timer)).Append(" - ");
    }

    private StringBuilder DataEnd()
    {
        return new StringBuilder().Append(TimeToString(timer)).Append(": Total Time: ").Append(TimeToString(timer - previusTime)).Append(",");
    }

    private string TimeToString(float t)
    {
        hours = (int)(t / 3600);
        minutes = (int)(t / 60) % 60;
        seconds = (int)t % 60;
        milliseconds = ((int)(t * 1000)) % 1000;

        return string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D3}",
            hours,
            minutes,
            seconds,
            milliseconds);
    }
}
