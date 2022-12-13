using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using CallbackSystem;

public class OnboardingUI : MonoBehaviour {
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private float fadeLength;
    private List<UnityEngine.UI.Button> unlockedButtons = new List<UnityEngine.UI.Button>();


    private void Start() {
        DebugEvent.AddListener<DebugEvent>(ListenForEvent);
        PauseEvent.AddListener<PauseEvent>(OnPauseOrUnPause);
        //DebugEvent e = new DebugEvent() { DebugText = "Banana start" };
        //e.Invoke();
    }

    private void OnDestroy() {
        //DebugEvent.RemoveListener<DebugEvent>(ListenForEvent);
    }

    private void ListenForEvent(DebugEvent e) {
        if (e.DebugText != null) {
            print(e.DebugText);
        }

        switch (e.DebugText) {
            case "climbing":
                CreateButton(2, "CLIMBING TUTORIAL");
                StartCoroutine(FadeNotification(true, "Climbing Tutorial Unlocked!"));
                break;
            case "cube":
                CreateButton(11, "CUBE TUTORIAL");
                StartCoroutine(FadeNotification(true, "Cube Tutorial Unlocked!"));
                break;
            case "timeTravel":
                CreateButton(12, "TIME TRAVEL TUTORIAL");
                StartCoroutine(FadeNotification(true, "Time Travel Tutorial Unlocked!"));
                break;
            case "scientist":
                CreateButton(13, "SCIENTIST TUTORIAL");
                StartCoroutine(FadeNotification(true, "Scientist Tutorial Unlocked!"));
                break;
            case "vacuumCleaner":
                CreateButton(14, "VACUUM CLEANER TUTORIAL");
                StartCoroutine(FadeNotification(true, "Vacuum Cleaner Tutorial Unlocked!"));
                break;
            case "darkness":
                CreateButton(15, "DARKNESS TUTORIAL");
                StartCoroutine(FadeNotification(true, "Darkness Tutorial Unlocked!"));
                break;
        }
    }

    private void CreateButton(int sceneIndex, string name) {
        print("Create button");
        GameObject buttonObject = Instantiate(buttonPrefab, transform) as GameObject;
        UnityEngine.UI.Button button = buttonObject.GetComponent<UnityEngine.UI.Button>();
        button.onClick.AddListener(delegate {
            FindObjectOfType<PauseMenuBehaviour>().UnPauseGame();
            SceneManager.LoadScene(sceneIndex);
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

    private void OnPauseOrUnPause(PauseEvent e) { foreach (var button in unlockedButtons) button.gameObject.SetActive(e.paused); }
}
