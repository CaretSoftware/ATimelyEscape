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
    public bool lockCursor = true;
    private void Update()
    {
        // pressing esc toggles between hide/show
        if (Input.GetKeyDown(KeyCode.Space))
        {
            lockCursor = !lockCursor;
        }

        Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !lockCursor;
    }
    public void RGBSlide()
    {
        var hue = rgbSlider.value;
        crossHair.color = Color.HSVToRGB(hue, 1f, 1f);
    }
}
