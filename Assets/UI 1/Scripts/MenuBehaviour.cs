using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuBehaviour : MonoBehaviour
{
    [SerializeField] private bool isPauseMenu = false;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject player;

    [SerializeField] private Slider slider;

    private IEnumerator currentCoroutine;

    private bool paused = false;

    private Animator pauseMenyAnimator;

    private void Start()
    {
        pauseMenyAnimator = gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !paused && isPauseMenu) {
            PauseGame();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && paused && isPauseMenu) {
            UnPauseGame();
        } 
    }

    // Method to load a scene by insert the index of wished scene presented in buildsettings
    public void SelectScene(int sceneIndex)
    {
        if(sceneIndex < 0 || sceneIndex > SceneManager.sceneCount)
        {
            Debug.Log("Error: Not valid SceneIndex: " + sceneIndex);
        }
        else
        {
            SceneManager.LoadScene(sceneIndex); // Later program LoadSceneAsync() for imporved loading experience with loadingScreen
        }
    }

    public void PauseGame()
    {
        if (paused)
        {
            Debug.Log("Error: Is Already Paused");
            return;
        }
        Debug.Log("Info: Paused Game");

        pauseMenyAnimator.Play("Pause");

        paused = true;

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
            
        currentCoroutine = PauseTime();

        StartCoroutine(currentCoroutine);
        
        if (slider == null)
            slider = GetComponentInChildren<Slider>();

        //if (slider != null)
            //slider.value = CameraController.Instance.MouseSensitivity;

        /*
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(true);
        }
        */
        //player.SetActive(false);
    }

    private float pauseDelay = 1f;

    // To Slow down gamepspeed with unscaledDeltaTime
    private IEnumerator PauseTime()
    {
        float time = 0;
        float startScale = Time.timeScale;

        while(time < pauseDelay)
        {
            time += Time.unscaledDeltaTime;

            Time.timeScale = Mathf.Lerp(startScale, 0, time /pauseDelay);

            yield return null;
        }
    }

    public void UnPauseGame()
    {
        Debug.Log("Info: Unpaused Game");

        pauseMenyAnimator.Play("UnPause");

        paused = false;

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = UnPauseTime();

        StartCoroutine(currentCoroutine);

        /*
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
        */
        //player.SetActive(true);
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

    // Method to quit the application anytime
    public void QuitGame()
    {
        Debug.Log("Info: Quit button has been Pressed");

        Time.timeScale = 1;

        Application.Quit();
    }
}
