using CallbackSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VignetteModifier : MonoBehaviour
{
    [SerializeField] private float modifyAmount = 0.002f;
    private Volume postProcesingVolume;
    private Vignette vignette;

    // Start is called before the first frame update
    void Start()
    {
        postProcesingVolume = GetComponent<Volume>();
        postProcesingVolume.profile.TryGet(out vignette);
        new SetVignetteModifierEvent(this).Invoke();
    }

    private void Update()
    {
        /*if (vignette.intensity.value < 1f)
        {
            vignette.intensity.value += modifyAmount * Time.deltaTime;
        }
        */
    }

}
