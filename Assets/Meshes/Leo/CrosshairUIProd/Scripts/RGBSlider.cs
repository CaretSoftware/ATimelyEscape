using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RGBSlider : MonoBehaviour
{
    [SerializeField] Slider rgbSlider;
    [SerializeField] RawImage crossHair;
    // Start is called before the first frame update
    void Start()
    {
        rgbSlider = this.gameObject.GetComponent<Slider>();
    }
    private void Update()
    {

    }
    public void RGBSlide()
    {
        var hue = rgbSlider.value;
        crossHair.color = Color.HSVToRGB(hue, 1f, 1f);
    }
}
