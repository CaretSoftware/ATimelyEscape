using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderBehaviour : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI sliderText;


    // Start is called before the first frame update
    void Start()
    {
        if(slider == null)
            slider = gameObject.GetComponent<Slider>();

        slider.onValueChanged.AddListener((currentValue) =>
        {
            //float newValue = (1 - (currentValue / slider.minValue)) * 100;
            float newValue = (currentValue / slider.maxValue) * 100;

            sliderText.text = (int) newValue + " %";
        });

        //float startValue = (1 - (slider.value / slider.minValue)) * 100;
        float startValue = (slider.value / slider.maxValue) * 100;

        sliderText.text = (int) startValue + " %";
    }
}
