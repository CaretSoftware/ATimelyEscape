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

    [SerializeField] private VariablesID data;

    [SerializeField] private int sliderSegments;


    private Slider _slider;
    private float value;
    private float maxValue;
    private float minValue;

    private float timer;

    void Start()
    {

        _slider = GetComponent<Slider>();
        if (myUnityEvent == null)
            myUnityEvent = new TestSlider();
        if (_slider == null) return;

        data = SaveDataCollected.LoadVariableData(gameObject.name + ".data");
        if (data == null)
        {
            data = new(gameObject.name);
        }
        maxValue = _slider.maxValue;
        minValue = _slider.minValue;
        RoundValue();
    }


    private void Update()
    {
        timer += Time.deltaTime;
    }
    private void OnDisable()
    {
        data.UpdateVariableData(value, timer);
        timer = 0f;
    }


    public void OnValueChange()
    {

        data.UpdateVariableData(value, timer);
        timer = 0f;
        RoundValue();
        myUnityEvent.Invoke(value);

    }

    private void OnDestroy()
    {
        data.UpdateVariableData(value, timer);
        SaveDataCollected.SaveVariableData(data);
    }

    [ContextMenu("Load")]

    private void Load()
    {
        data = SaveDataCollected.LoadVariableData(gameObject.name + ".data");
    }

    private void RoundValue()
    {
        value = Mathf.Round((_slider.value) * (sliderSegments / (maxValue - minValue))) / (sliderSegments / (maxValue - minValue));
        _slider.value = value;
    }

    [Serializable]

    public class TestSlider : UnityEvent<float> { }

}