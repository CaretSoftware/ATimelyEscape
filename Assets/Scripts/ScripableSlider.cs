using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScripableSlider : MonoBehaviour
{
    [SerializeField] private TestSlider myUnityEvent = new TestSlider();

    public TestSlider OnValueChanged { get { return myUnityEvent; } set { myUnityEvent = value; } }

    private Slider _slider;

    void Start()
    {
        _slider = GetComponent<Slider>();
        if (myUnityEvent == null)
            myUnityEvent = new TestSlider();

        if (_slider == null) return;

        myUnityEvent.AddListener(Ping);

    }

    private void OnMouseUp()
    {
        myUnityEvent.Invoke(_slider.value);
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            myUnityEvent.Invoke(_slider.value);
        }
    }

    void Ping(float value)
    {
        Debug.Log("Ping" + value);
    }

    [Serializable]

    public class TestSlider : UnityEvent<float> { }

}