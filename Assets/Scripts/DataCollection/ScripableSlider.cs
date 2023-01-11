using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScripableSlider : MonoBehaviour
{
    [SerializeField] private TestSlider myUnityEvent = new TestSlider();

    public TestSlider OnValueChanged { get { return myUnityEvent; } set { myUnityEvent = value; } }

    private Slider _slider;

    private float value;

    private float timer;
    private float previusTime;

    private int hours;
    private int minutes;
    private int seconds;
    private int milliseconds;

    private string path;
    private string fileName;

   

    StreamWriter writer;

    void Start()
    {
        path = Application.persistentDataPath + "/";
        _slider = GetComponent<Slider>();
        if (myUnityEvent == null)
            myUnityEvent = new TestSlider();
        if (_slider == null) return;
        myUnityEvent.AddListener(Ping);
        timer = 0;
        string temp = gameObject.name;
        EnsureDirectoryExists(path + temp + "/");
        int count = 0;
        while(File.Exists(path + temp + "/" + string.Format("{0:D2}", count) + "_" + temp + ".txt"))
        {
            count++;
        }
        StringBuilder temfil= new StringBuilder();
        temfil.Append(path).Append(temp).Append("/").Append(string.Format("{0:D2}", count)).Append("_").Append(temp).Append(".txt");

        fileName= temfil.ToString();    
        
        writer = new StreamWriter(fileName);
    
        
        value = _slider.value;


        writer.Write(DataStart());

        writer.Close();

        previusTime = timer;

        

    }


    private void Update()
    {
        timer += Time.unscaledDeltaTime;
        if (Input.GetMouseButtonUp(0) && value != _slider.value)
        {

            writer = File.AppendText(fileName);
            
            writer.WriteLine(DataEnd());
           
            value = _slider.value;
            myUnityEvent.Invoke(value);
            writer.Write(DataStart()); 
            writer.Close();


            previusTime = timer;
        }


    }
    private void OnDisable()
    {
        writer = File.AppendText(fileName);

        writer.WriteLine(DataEnd());

        value = _slider.value;
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

    void Ping(float value)
    {
        Debug.Log("Ping" + value);
    }

    private StringBuilder DataStart()
    {
        return new StringBuilder().Append(value).Append(": ").Append(TimeToString(timer)).Append(" - ");
    }

    private StringBuilder DataEnd()
    {
        return new StringBuilder().Append(TimeToString(timer)).Append(": Total Time: ").Append(TimeToString(timer - previusTime)).Append(",");
    }

    private static void EnsureDirectoryExists(string filePath)
    {
        FileInfo fi = new FileInfo(filePath);
        if (!fi.Directory.Exists)
        {
            Directory.CreateDirectory(fi.DirectoryName);
            
        }
    }

    [Serializable]

    public class TestSlider : UnityEvent<float> { }

}