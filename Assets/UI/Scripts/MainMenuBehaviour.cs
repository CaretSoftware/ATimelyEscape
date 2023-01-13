using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuBehaviour : MonoBehaviour
{
    private Animator startMenyAnimator;
    private bool hasStartUp = false;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    [Header("Camera")]
    [SerializeField] private Animator cameraAnimator;

    [Header("Loading Components")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Image loadingBarFill;
    [SerializeField] private TextMeshProUGUI loadingText;

    private void Start()
    {
        Time.timeScale = SettingsManager.Instance.timeScaleValue;
        startMenyAnimator = gameObject.GetComponent<Animator>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Input.anyKey && !hasStartUp)
        {
            startMenyAnimator.SetTrigger("ToIntro");
            hasStartUp = true;
        }
    }

    // Method to load a scene by insert the index of wished scene presented in buildsettings
    public void SelectScene(int sceneIndex)
    {
        if(sceneIndex < 0 || sceneIndex > SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log("Error: Not valid SceneIndex: " + sceneIndex);
        }
        else
        {
            startMenyAnimator.Play("PlayGame");
            //StartCoroutine(LoadSceneAsync(sceneIndex)); It is activated in Animation StartMenuOutro
        }
    }

    private IEnumerator LoadSceneAsync(int sceneIndex)
    {
        yield return new WaitForSeconds(2.0f);

        loadingScreen.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);

            if(loadingBarFill != null)
                loadingBarFill.fillAmount = progressValue;

            if(loadingText != null)
                loadingText.text = (int) (progressValue * 100) + "%";

            yield return null;
        }
    }

    // Method to quit the application anytime
    public void QuitGame()
    {
        Debug.Log("Info: Quit button has been Pressed");

        Application.Quit();
    }

    private void PlayCameraTrigg(string trigger)
    {
        cameraAnimator.Play(trigger);
    }

    private void PlayClip(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);
    }
}
