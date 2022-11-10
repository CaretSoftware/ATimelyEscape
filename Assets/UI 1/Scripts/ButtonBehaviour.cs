using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonBehaviour : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Image image;
    private UnityEngine.UI.Button button;

    [Header("Text To Speach Clips")]
    [SerializeField] private AudioClip speach;
    
    [Header("Button Clips")]
    [SerializeField] private AudioClip click;
    [SerializeField] private AudioClip hover;

    private AudioSource source;
    
    [Header("Button State Sprites")]
    [SerializeField] private Sprite neutral;
    [SerializeField] private Sprite pressed;
    [SerializeField] private Sprite selected;
    // [SerializeField] private AudioClip compressClip, unCompressClip;
    // [SerializeField] private AudioSource audioSource; 

    // Method when user has pressed the mousebutton

    [Header("Overhaul")]
    [SerializeField] private int thisIndex;
    [SerializeField] private MenuButtonController menuButtonController;

    private void Start()
    {
        image = gameObject.GetComponent<Image>();
        image.sprite = neutral;
        source = GetComponent<AudioSource>();
        button = GetComponent<UnityEngine.UI.Button>();
    }

    private void Update()
    {
        if(menuButtonController.index == thisIndex)
        {
            if(Input.GetAxis("Submit") == 1 || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                ToPressedSprite();
                button.Select();
            }
            else
            {
                ToSelectedSprite();
            }
        }
        else
        {
            ToNeutralSprite();
        }
    }

    private void ActiveMouse(bool active)
    {
        if (active) 
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        Cursor.visible = active;
    }

    // MouseHandler Methods /Clicking
    public void OnPointerDown(PointerEventData eventData)
    {
        ToPressedSprite();
        source.PlayOneShot(click);
    }

    // Method when user has released the mousebutton
    public void OnPointerUp(PointerEventData eventData)
    {
        ToNeutralSprite();
    }

    // Hover sound

    public void OnPointerEnter(PointerEventData eventData)
    {
        ToSelectedSprite();
        source.PlayOneShot(hover);

        if(speach != null)
            source.PlayOneShot(speach);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToNeutralSprite();
    }

    // Methods for change buttons sprites depending its state
    public void ToNeutralSprite()
    {
        image.sprite = neutral;
    }

    public void ToSelectedSprite()
    {
        image.sprite = selected;
    }

    // Audio clips när man har selected

    public void ToPressedSprite()
    {
        image.sprite = pressed;
    }
}
