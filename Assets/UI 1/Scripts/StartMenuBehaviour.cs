using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class StartMenuBehaviour : MonoBehaviour
{
    private Animator startMenyAnimator;

    [Header("Loading Components")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Image loadingBarFill;
    [SerializeField] private TextMeshProUGUI loadingText;

    private void Start()
    {
        Time.timeScale = 1;
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
            startMenyAnimator.Play("Outro");
            //StartCoroutine(LoadSceneAsync(sceneIndex)); It is activated in Animation StartMenuOutro
        }
    }

    private IEnumerator LoadSceneAsync(int sceneIndex)
    {
        loadingScreen.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);

            loadingBarFill.fillAmount = progressValue;
            loadingText.text = (int) (progressValue * 100) + "%";
            yield return null;
        }
    }

    // Method to quit the application anytime
    private void QuitGame()
    {
        Debug.Log("Info: Quit button has been Pressed");

        Application.Quit();
    }
}
