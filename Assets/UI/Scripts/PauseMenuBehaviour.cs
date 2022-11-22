using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using RatCharacterController;

public class PauseMenuBehaviour : MonoBehaviour
{
    private IEnumerator currentCoroutine;

    private bool paused;

    private Animator pauseMenyAnimator;

    [SerializeField] private Slider slider;

    private void Start()
    {
        Time.timeScale = 1;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        pauseMenyAnimator = gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !paused)
        {
            PauseGame();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && paused)
        {
            UnPauseGame();
        }
    }

    public void PauseGame()
    {
        if (paused)
        {
            Debug.Log("Error: Is Already Paused");
            return;
        }

        CharacterInput.IsPaused(true);

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        pauseMenyAnimator.Play("Pause");

        paused = true;

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = PauseTime();

        StartCoroutine(currentCoroutine);

        if (slider == null)
            slider = GetComponentInChildren<Slider>();

        if (slider != null && CameraController.Instance != null)
            slider.value = CameraController.Instance.MouseSensitivity;
    }

    private float pauseDelay = 1f;

    // To Slow down gamepspeed with unscaledDeltaTime
    private IEnumerator PauseTime()
    {
        float time = 0;
        float startScale = Time.timeScale;

        while (time < pauseDelay)
        {
            time += Time.unscaledDeltaTime;

            Time.timeScale = Mathf.Lerp(startScale, 0, time / pauseDelay);

            yield return null;
        }
    }

    public void UnPauseGame()
    {
        //Debug.Log("Info: Unpaused Game");

        pauseMenyAnimator.Play("UnPause");

        paused = false;

        CharacterInput.IsPaused(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = UnPauseTime();

        StartCoroutine(currentCoroutine);
    }

    // To speed up gamepspeed with unscaledDeltaTime 
    private IEnumerator UnPauseTime()
    {
        float time = 0;
        float startScale = Time.timeScale;

        while (time < pauseDelay)
        {
            time += Time.unscaledDeltaTime;

            Time.timeScale = Mathf.Lerp(startScale, 1, time / pauseDelay);

            yield return null;
        }
    }

    // Method to load a scene by insert the index of wished scene presented in buildsettings
    public void SelectScene(int sceneIndex)
    {
        if (sceneIndex < 0 || sceneIndex > SceneManager.sceneCount)
        {
            Debug.Log("Error: Not valid SceneIndex: " + sceneIndex);
        }
        else
        {
            SceneManager.LoadScene(sceneIndex); // Later program LoadSceneAsync() for imporved loading experience with loadingScreen
        }
    }

    // Method to quit the application anytime
    public void QuitGame()
    {
        Debug.Log("Info: Quit button has been Pressed");

        Application.Quit();
    }

    public bool isPaused()
    {
        return paused;
    }
}
