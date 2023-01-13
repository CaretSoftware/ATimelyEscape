using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuBehaviour : MonoBehaviour {
    // This is a delegate. It can be Invoked when needed (Done currently in the Input class)
    // all methods that subscribe to the delegate gets called when Invoked.
    // In this class the method PausePressed() is subscribed to it.
    public delegate void PauseDelegate(bool paused);
    public static PauseDelegate pauseDelegate;
    
    private IEnumerator currentCoroutine;

    public bool isPaused { get; set; }

    private Animator pauseMenyAnimator;

    private NewRatCharacterController.NewRatCharacterController newRatCharacterController;

    [SerializeField] private MenuSelection menuSelection;
    [SerializeField] private GameObject buttonToSelected;

    private void Start()
    {
        newRatCharacterController = FindObjectOfType<NewRatCharacterController.NewRatCharacterController>();

        pauseDelegate += PausePressed;
        
        Time.timeScale = SettingsManager.Instance.timeScaleValue;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        pauseMenyAnimator = gameObject.GetComponent<Animator>();
    }

    private void OnDestroy() {
        pauseDelegate -= PausePressed;
    }

    private void PausePressed(bool paused) {
        if (paused)
            PauseGame();
        else
            UnPauseGame();
    }

    // private void Update()
    // {
        // if (Input.GetKeyDown(KeyCode.Escape) && !paused)
        // {
        // }
        // else if (Input.GetKeyDown(KeyCode.Escape) && paused)
        // {
        // }
    // }

    public void PauseGame() {

        //Cursor.lockState = CursorLockMode.Confined;
        //Cursor.visible = true;

        newRatCharacterController.paused = true;

        menuSelection.SelectButton(buttonToSelected);

        CallbackSystem.PauseEvent pauseEvent = new CallbackSystem.PauseEvent { paused = true };
        pauseMenyAnimator.Play("Pause");

        // paused = true;

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = PauseTime();

        StartCoroutine(currentCoroutine);
    }

    private float pauseDelay = 1f;

    // To Slow down game speed with unscaledDeltaTime
    private IEnumerator PauseTime()
    {
        float time = 0;
        float startScale = Time.timeScale;

        while (time < pauseDelay) {
            time += Time.unscaledDeltaTime;

            Time.timeScale = Mathf.Lerp(startScale, 0, time / pauseDelay);

            yield return null;
        }
    }

    public void UnPauseGame() {

        //CallbackSystem.PauseEvent pauseEvent = new CallbackSystem.PauseEvent { paused = false };
        //pauseEvent.Invoke();
        newRatCharacterController.paused = false;

        pauseMenyAnimator.Play("UnPause");

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = UnPauseTime();

        StartCoroutine(currentCoroutine);
    }

    // To speed up game speed with unscaledDeltaTime 
    private IEnumerator UnPauseTime()
    {
        float time = 0;
        float startScale = Time.timeScale;

        while (time < pauseDelay) {
            time += Time.unscaledDeltaTime;

            Time.timeScale = Mathf.Lerp(startScale, SettingsManager.Instance.timeScaleValue, time / pauseDelay);

            yield return null;
        }
    }

    // Method to load a scene by insert the index of wished scene presented in buildsettings
    public void SelectScene(int sceneIndex) {
        if (sceneIndex < 0 || sceneIndex > SceneManager.sceneCount) {
            Debug.Log("Error: Not valid SceneIndex: " + sceneIndex);
        }
        else
        {
            SceneManager.LoadScene(sceneIndex); // Later program LoadSceneAsync() for imporved loading experience with loadingScreen
        }
    }

    // Method to quit the application anytime
    public void QuitGame() {
        Debug.Log("Info: Quit button has been Pressed");
        Application.Quit();
    }
}
