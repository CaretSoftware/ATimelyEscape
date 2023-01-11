using NaughtyAttributes;
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

    [SerializeField]private VariablesID data;

    private Slider _slider;

    private float previusValue;
    private float value;

    private float timer;

    private int hours;
    private int minutes;
    private int seconds;

    private string path;
    private string fileName;

   

    StreamWriter writer;

    void Start()
    {
        
        _slider = GetComponent<Slider>();
        if (myUnityEvent == null)
            myUnityEvent = new TestSlider();
        if (_slider == null) return;
        myUnityEvent.AddListener(Ping);
        data = new(gameObject.name);
        value = _slider.value;
    }


    private void Update()
    {
        timer += Time.deltaTime;
    }
    private void OnDisable()
    {
        data.UpdateVariableData(value, timer);
    }

  
    public void OnValueChange()
    {
        data.UpdateVariableData(value, timer);
        timer = 0f;
        value = _slider.value;
        myUnityEvent.Invoke(value);

        Debug.Log(value);
    }

    private void OnDestroy()
    {
        data.UpdateVariableData(value, timer);
    }

    void Ping(float value)
    {
        Debug.Log("Ping" + value);
    }




    [Serializable]

    public class TestSlider : UnityEvent<float> { }

}