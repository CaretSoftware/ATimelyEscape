using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeyPadTest : MonoBehaviour
{
    
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject keyPad;
    //[SerializeField] private GameObject hud;

    [SerializeField] private GameObject animateGameObject;
    [SerializeField] private Animator animator;

    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private string answer = "12345";

    [SerializeField] private Collider keyPadTrigger;

    /* [SerializeField] private AudioSource button;
     [SerializeField] private AudioSource correct;
     [SerializeField] private AudioSource wrong;*/

    private bool isAnimating;
    

    void Start()
    {
        keyPad.SetActive(false);
        isAnimating = false; 
    }

    public void Number(int number)
    {
        text.text += number.ToString();
        //button.Play();
    }
    public void Execute()
    {
        if(text.text == answer)
        {
            //correct.Play();
            text.text = "Door Open";
            isAnimating = true; 
        }
        else
        {
            //wrong.Play();
            text.text = "Wrong Code";
        }
    }

    public void Clear()
    {
        text.text = "";
        //button.Play();
    }
    public void Exit()
    {
        keyPad.SetActive(false);
        //hud.SetActive(true);
        player.SetActive(true);
        keyPadTrigger.enabled = false;
        Invoke("KeypadDelay", 3.0f);
    }
    private void Update()
    {
        if(text.text == "Door Open" && isAnimating)
        {
            animator.SetBool("Open", true);
        }
        if (keyPad.activeInHierarchy)
        {
            player.SetActive(false);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
    private void KeyPadDelay()
    {
        keyPadTrigger.enabled = true;
    }

}
