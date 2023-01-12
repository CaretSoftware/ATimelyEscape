using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsMenuController : MonoBehaviour
{
    public bool isPauseMenu;

    public AudioMixer audioMixer;
    public TMPro.TMP_Dropdown resolutionDropdown;
    Resolution[] resolutions;

    [Header("UI Sliders")]
    [SerializeField] private Slider volumeMasterSlider;
    [SerializeField] private Slider volumeMusicSlider;
    [SerializeField] private Slider volumeEffectsSlider;
    [SerializeField] private Slider timeScaleSlider;

    [Header("UI Crosshair")]
    [SerializeField] private GameObject crossHairCanvas;

    [Header("UI Toggle")]
    [SerializeField] private Toggle textToSpeach;
    [SerializeField] private Toggle movementAccessiblityControl;
    [SerializeField] private Toggle crosshair;
    [SerializeField] private Toggle navigationAssist;

    private void Start()
    {
        volumeMasterSlider.value = SettingsManager.Instance.masterVolume;
        volumeMusicSlider.value = SettingsManager.Instance.musicVolume;
        volumeEffectsSlider.value = SettingsManager.Instance.effectVolume;
        timeScaleSlider.value = SettingsManager.Instance.timeScaleValue;

        SetTextToSpeachActive(SettingsManager.Instance.textToSpeachActive);
        SetMovementControls(SettingsManager.Instance.movmentAccessiblityActive);
        SetCrossHairCanvas(SettingsManager.Instance.crossHairCanvasActive);
        SetNavigationAssistActive(SettingsManager.Instance.navigationAssistActive);

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
    }
    
    public void SetTextToSpeachActive(bool active) // Checkbox for TextForSpeach
    {
        SettingsManager.Instance.textToSpeachActive = active;
        textToSpeach.isOn = active;
        ButtonSoundBehaviour.shouldPlayTextToSpeach = active;
    }

    public void SetDeltaTime(float value)
    {
        if (value < 0)
            SettingsManager.Instance.timeScaleValue = 0;
        SettingsManager.Instance.timeScaleValue = value;
    }

    public void SetMovementControls(bool accessible)
    {
        SettingsManager.Instance.movmentAccessiblityActive = accessible;
        movementAccessiblityControl.isOn = accessible;
        if(isPauseMenu)
            NewRatCharacterController.NewCharacterInput.Accessibility = accessible;
    }

    public void SetCrossHairCanvas(bool active)
    {
        SettingsManager.Instance.crossHairCanvasActive = active;
        crosshair.isOn = active;
        if(crossHairCanvas != null)
            crossHairCanvas.SetActive(active);
    }

    public void SetNavigationAssistActive(bool active)
    {
        SettingsManager.Instance.navigationAssistActive = active;
        navigationAssist.isOn = active;
        if(isPauseMenu)
            CognitiveAssistanceTriggerHandler.SetNavigationActive(navigationAssist.isOn);
    }

    public void SetMasterVolume(float volume)
    {
        SettingsManager.Instance.masterVolume = volume;
        audioMixer.SetFloat("MasterVolume", Mathf.Log(volume) * 20);
    }
    public void SetMusicVolume(float volume)
    {
        SettingsManager.Instance.musicVolume = volume;
        audioMixer.SetFloat("MusicVolume", Mathf.Log(volume) * 20);
    }
    public void SetEffectsVolume(float volume)
    {
        SettingsManager.Instance.effectVolume = volume;
        audioMixer.SetFloat("EffectsVolume", Mathf.Log(volume) * 20);
    }

    public void SetResolution(int index)
    {
        Resolution resolution = resolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    public void SetFullscreen(bool isFullscreen) // Checkbox for Fullscreen
    {
        Screen.fullScreen = isFullscreen;
    }
}
