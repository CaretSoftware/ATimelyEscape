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
    private float value;

    private float timer;

    void Start()
    {
        
        _slider = GetComponent<Slider>();
        if (myUnityEvent == null)
            myUnityEvent = new TestSlider();
        if (_slider == null) return;

        data = SaveDataCollected.LoadVariableData(gameObject.name);
        if (data == null)
        {
            data = new(gameObject.name);
        }
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

    }

    private void OnDestroy()
    {
        data.UpdateVariableData(value, timer);
        SaveDataCollected.SaveVariableData(data);
    }




    [Serializable]

    public class TestSlider : UnityEvent<float> { }

}