using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class FinnishPuzzle : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject finnish; 
    [SerializeField] private TextMeshProUGUI finnishText;

    private void Awake()
    {
        finnish.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            finnish.SetActive(true);
        }
    }

    public void PrevPuzzle()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void NextPuzzle()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void Update()
    {
        if (finnish.activeInHierarchy)
        {
            player.SetActive(false);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

}
