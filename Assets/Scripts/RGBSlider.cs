using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RGBSlider : MonoBehaviour
{
    [SerializeField] Slider rgbSlider;
    [SerializeField] public Image crossHair;
    //[SerializeField] public Image crossHairInGame;
    [SerializeField] public Color color;
    //[SerializeField] private LayerMask layerMask;

    void Start()
    {
        rgbSlider = this.gameObject.GetComponent<Slider>();
    }

    public void RGBSlide()
    {
        var hue = rgbSlider.value;
        crossHair.color = Color.HSVToRGB(hue, 1f, 1f);
        //crossHairInGame.color = Color.HSVToRGB(hue, 1f, 1f);
    }

}
