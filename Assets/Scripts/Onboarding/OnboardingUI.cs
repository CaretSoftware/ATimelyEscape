using System.Collections;
using System.Collections.Generic;
using CallbackSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// @author Emil Wessman & Greta Hassler
/// </summary>
public class OnboardingUI : MonoBehaviour {
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private float fadeLength;
    private List<Button> unlockedButtons = new List<Button>();
    private RuntimeSceneManager sceneManager;
    private const int ONBOARDING_INDEX_OFFSET = 11,
    INCUBATOR_SCENE_INDEX = 2,
    CUBE_CLIMB_INDEX = 11,
    CHARGE_INDEX = 12,
    TIME_TRAVEL_INDEX = 13,
    SCIENTIST_INDEX = 14,
    ROOMBA_INDEX = 15;

    private TimeTravelPeriod[] startPeriods = new TimeTravelPeriod[]{
        TimeTravelPeriod.Past,
        TimeTravelPeriod.Past,
        TimeTravelPeriod.Past,
        TimeTravelPeriod.Past,
        TimeTravelPeriod.Present
    };

    private void Start() {
        sceneManager = FindObjectOfType<RuntimeSceneManager>();
        DebugEvent.AddListener<DebugEvent>(OnDiscoveredNewTutorial);
        PauseMenuBehaviour.pauseDelegate += OnPauseOrUnPause;
    }

    private void OnDiscoveredNewTutorial(DebugEvent e) {
        switch (e.DebugText) {
            case "interactions":
                CreateButton(CUBE_CLIMB_INDEX, "CUBE & CLIMB TUTORIAL");
                StartCoroutine(FadeNotification(true, "Cubes and climbing Tutorial Unlocked!"));
                break;
            case "charge":
                CreateButton(CHARGE_INDEX, "CUBE CHRAGE TUTORIAL");
                StartCoroutine(FadeNotification(true, "Cube charge Tutorial Unlocked!"));
                break;
            case "timeTravel":
                CreateButton(TIME_TRAVEL_INDEX, "TIME TRAVEL TUTORIAL");
                StartCoroutine(FadeNotification(true, "Time Travel Tutorial Unlocked!"));
                break;
            case "scientist":
                CreateButton(SCIENTIST_INDEX, "SCIENTIST TUTORIAL");
                StartCoroutine(FadeNotification(true, "Scientist Tutorial Unlocked!"));
                break;
            case "vacuumCleaner":
                CreateButton(ROOMBA_INDEX, "ROOMBA TUTORIAL");
                StartCoroutine(FadeNotification(true, "Roomba Tutorial Unlocked!"));
                break;
        }
    }

    /// <summary>
    /// Creates buttons based with delgates triggering the right onboarding room to be told. In case of main tutorial room (incubator)
    /// a notification saying the action is not allowed will be shown.
    /// </summary>
    /// <param name="sceneIndex">the index of the tutorial scene</param>
    /// <param name="name">The text to display on the button in the UI</param>
    private void CreateButton(int sceneIndex, string name) {
        GameObject buttonObject = Instantiate(buttonPrefab, transform) as GameObject;
        Button button = buttonObject.GetComponent<Button>();
        button.onClick.AddListener(delegate {
            if (sceneManager.CurrentSceneIndex != INCUBATOR_SCENE_INDEX) {
                FindObjectOfType<NewRatCharacterController.NewRatCharacterController>().paused = false;
                sceneManager.LoadOnboardingRoom(sceneIndex);
                TimeTravelManager.currentPeriod = startPeriods[sceneIndex - ONBOARDING_INDEX_OFFSET];
                TimeTravelManager.ReloadCurrentTimeTravelPeriod();
            } else StartCoroutine(FadeNotification(true, "Separate tutorials are unavailable during main tutorial"));

        });
        button.GetComponentInChildren<TextMeshProUGUI>().text = name;
        button.transform.parent = transform;
        button.gameObject.SetActive(false);
        unlockedButtons.Add(button);
    }

    /// <summary>
    /// Fades in or out a notification message on the UI
    /// </summary>
    /// <param name="fadeIn">Whether to fade in or out</param>
    /// <param name="text">The message</param>
    /// <returns></returns>
    private IEnumerator FadeNotification(bool fadeIn, string text = null) {
        if (text != null) notificationText.text = text;
        Color32 initialColor = notificationText.color;
        float alpha = fadeIn ? 255 : 0;
        Color32 targetColor = new Color32(initialColor.r, initialColor.g, initialColor.b, (byte)alpha);

        float elapsedTime = 0f;

        while (elapsedTime < fadeLength) {
            elapsedTime += Time.unscaledDeltaTime;

            notificationText.color = Color32.Lerp(initialColor, targetColor, elapsedTime / fadeLength);
            yield return null;
        }

        if (fadeIn) StartCoroutine(FadeNotification(false));
    }

    private void OnPauseOrUnPause(bool paused) {
        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
        foreach (var button in unlockedButtons) button.gameObject.SetActive(paused);
    }

    private void OnDestroy() {
        if (EventSystem.Current != null) DebugEvent.RemoveListener<DebugEvent>(OnDiscoveredNewTutorial);

        PauseMenuBehaviour.pauseDelegate -= OnPauseOrUnPause;
    }
}
