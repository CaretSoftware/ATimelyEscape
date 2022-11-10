using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartMenuBehaviour : MonoBehaviour
{
    private Animator startMenyAnimator;

    private void Start()
    {
        startMenyAnimator = gameObject.GetComponent<Animator>();

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

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

    // Method to quit the application anytime
    public void QuitGame()
    {
        Debug.Log("Info: Quit button has been Pressed");

        Time.timeScale = 1;

        Application.Quit();
    }
}
