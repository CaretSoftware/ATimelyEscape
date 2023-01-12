using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    [Header("Volume Values")]
    public float masterVolume;
    public float musicVolume;
    public float effectVolume;

    [Header("Accessibility Values")]
    public bool textToSpeachActive;
    public bool crossHairCanvasActive;
    public bool movmentAccessiblityActive;
    public bool navigationAssistActive;

    [Header("Overhaul Values")]
    public float timeScaleValue = 1;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
