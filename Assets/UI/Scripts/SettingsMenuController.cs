using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsMenuController : MonoBehaviour
{
    public AudioMixer audioMixer;

    public TMPro.TMP_Dropdown resolutionDropdown;

    Resolution[] resolutions;

    public bool shouldPlayTextToSpeach;

    [Header("UI Sliders")]
    [SerializeField] private Slider volumeMasterSlider;
    [SerializeField] private Slider volumeMusicSlider;
    [SerializeField] private Slider volumeEffectsSlider;

    [Header("Volume Values")]
    [SerializeField] private bool shallChangeValuesAtStart;
    [SerializeField] private float startMasterVolume;
    [SerializeField] private float startMusicVolume;
    [SerializeField] private float startEffectsVolume;

    [Header("CrosshairUI")]
    [SerializeField] private GameObject crossHairCanvas;

    private void Start()
    {

        if (resolutionDropdown != null)
        {
            resolutions = Screen.resolutions;

            resolutionDropdown.ClearOptions();

            List<string> options = new List<string>();

            int currentResolutionIndex = 0;

            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + "x" + resolutions[i].height;
                options.Add(option);

                if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }

            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }
        else
        {
            Debug.Log("Message: Settings Menu is missing Dropdown for resolution");
        }

        if (shallChangeValuesAtStart)
        {
            volumeMasterSlider.value = startMasterVolume;
            volumeMusicSlider.value = startMusicVolume;
            volumeEffectsSlider.value = startEffectsVolume;
        }
        /*
        else
        {
            audioMixer.GetFloat("MasterVolume", out float value);
            volumeMasterSlider.value = value;

            audioMixer.GetFloat("MusicVolume", out float value1);
            volumeMusicSlider.value = value1;

            audioMixer.GetFloat("EffectsVolume", out float value2);
            volumeEffectsSlider.value = value2;
        }
        */
    }

    public void SetFullscreen(bool isFullscreen) // Checkbox for Fullscreen
    {
        Screen.fullScreen = isFullscreen;
    }
    
    public void SetShouldPlayTextToSpeach(bool shouldPlayTextToSpeach) // Checkbox for TextForSpeach
    {
        ButtonSoundBehaviour.shouldPlayTextToSpeach = shouldPlayTextToSpeach;
    }

    public void SetMovementControls(bool accessible)
    {
        NewRatCharacterController.NewCharacterInput.Accessibility = accessible;
    }

    public void SetCrossHairCanvas(bool on)
    {
        crossHairCanvas.SetActive(on);
    }

    public void SetCameraControls(bool boolean)
    {

    }
    
    public void SetCognitiveAssistanceGuidance(bool boolean)
    {

    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log(volume) * 20);
    }
    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log(volume) * 20);
    }
    public void SetEffectsVolume(float volume)
    {
        audioMixer.SetFloat("EffectsVolume", Mathf.Log(volume) * 20);
    }

    public void SetResolution(int index)
    {
        Resolution resolution = resolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}
