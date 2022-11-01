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

    private bool paused;

    [SerializeField] private Slider slider;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !paused && isPauseMenu) {
            paused = true;
            PauseGame();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && paused && isPauseMenu) {
            paused = false;
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
        Time.timeScale = 0f;
        
        if (slider == null)
            slider = GetComponentInChildren<Slider>();
        
        if (slider != null)
            slider.value = CameraController.Instance.MouseSensitivity;

        Debug.Log("Info: Paused game- TimeScale " + Time.timeScale);

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(true);
        }
        player.SetActive(false);
    }

    public void UnPauseGame()
    {
        Time.timeScale = 1f;

        Debug.Log("Info: Unpaused game - TimeScale " + Time.timeScale);

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
        player.SetActive(true);
    }

    // Method to quit the application anytime
    public void QuitGame()
    {
        Debug.Log("Info: Quit button has been Pressed");

        Time.timeScale = 1;

        Application.Quit();
    }
}
