using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonBehaviour : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    private UnityEngine.UI.Image image;
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

    private void Start()
    {
        image = gameObject.GetComponent<UnityEngine.UI.Image>();
        image.sprite = neutral;
        source = GetComponent<AudioSource>();
        button = GetComponent<UnityEngine.UI.Button>();
    }

    private bool beenResetedAfterMouseMoving = false;
    private bool playedHoverClip = false;

    private void Update()
    {
        
    }

    // MouseHandler Methods /Clicking
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!button.IsInteractable())
            return;

        PlayClickClip();
        ToPressedSprite();
    }

    // Method when user has released the mousebutton
    public void OnPointerUp(PointerEventData eventData)
    {
        ToNeutralSprite();
    }

    // Hover sound

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!button.IsInteractable())
            return;

        ToSelectedSprite();
        PlayHoverClip();
        PlaySpeachClip();
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

    private void PlayHoverClip()
    {
        if (hover != null)
        {
            source.PlayOneShot(hover);
        }
        else
        {
            Debug.Log("Error: Missing hover sound");
        }
    }

    private void PlayClickClip()
    {
        if (click != null)
        {
            source.PlayOneShot(click);
        }
        else
        {
            Debug.Log("Error: Missing click sound");
        }
    }

    private void PlaySpeachClip()
    {
        if (speach != null)
        {
            source.PlayOneShot(speach);
        }
        else
        {
            Debug.Log("Error: Missing speach sound");
        }
    }
}
