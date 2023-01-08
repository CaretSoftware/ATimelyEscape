using System.Collections;
using System.Collections.Generic;
using CallbackSystem;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OnboardingUI : MonoBehaviour {
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private float fadeLength;
    private List<Button> unlockedButtons = new List<Button>();
    private RuntimeSceneManager sceneManager;

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
        if (e.DebugText != null) print(e.DebugText);

        switch (e.DebugText) {
            case "interactions":
                CreateButton(11, "CUBE & CLIMB TUTORIAL");
                StartCoroutine(FadeNotification(true, "Cubes and climbing Tutorial Unlocked!"));
                break;
            case "charge":
                CreateButton(12, "CUBE CHRAGE TUTORIAL");
                StartCoroutine(FadeNotification(true, "Cube charge Tutorial Unlocked!"));
                break;
            case "timeTravel":
                CreateButton(13, "TIME TRAVEL TUTORIAL");
                StartCoroutine(FadeNotification(true, "Time Travel Tutorial Unlocked!"));
                break;
            case "scientist":
                CreateButton(14, "SCIENTIST TUTORIAL");
                StartCoroutine(FadeNotification(true, "Scientist Tutorial Unlocked!"));
                break;
            case "vacuumCleaner":
                CreateButton(15, "VACUUM CLEANER TUTORIAL");
                StartCoroutine(FadeNotification(true, "Vacuum Cleaner Tutorial Unlocked!"));
                break;
        }
    }

    private void CreateButton(int sceneIndex, string name) {
        GameObject buttonObject = Instantiate(buttonPrefab, transform) as GameObject;
        Button button = buttonObject.GetComponent<Button>();
        button.onClick.AddListener(delegate {
            FindObjectOfType<NewRatCharacterController.NewRatCharacterController>().paused = false;
            sceneManager.LoadOnboardingRoom(sceneIndex);
            TimeTravelManager.currentPeriod = startPeriods[sceneIndex - 11];
            TimeTravelManager.ReloadCurrentTimeTravelPeriod();
            print("onboard button clicked");
        });
        button.GetComponentInChildren<TextMeshProUGUI>().text = name;
        button.transform.parent = transform;
        button.gameObject.SetActive(false);
        unlockedButtons.Add(button);
    }

    private IEnumerator FadeNotification(bool fadeIn, string text = null) {
        if (text != null) notificationText.text = text;
        Color32 initialColor = notificationText.color;
        float alpha = fadeIn ? 255 : 0;
        Color32 targetColor = new Color32(initialColor.r, initialColor.g, initialColor.b, (byte)alpha);

        float elapsedTime = 0f;

        while (elapsedTime < fadeLength) {
            elapsedTime += Time.deltaTime;

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
