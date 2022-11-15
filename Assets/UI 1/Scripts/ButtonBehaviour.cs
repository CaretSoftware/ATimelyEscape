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

    [Header("Overhaul")]
    [SerializeField] private int thisIndex;
    [SerializeField] private MenuButtonController menuButtonController;
    [SerializeField] private GameObject selectionHint;

    private void Start()
    {
        if(selectionHint != null)
            selectionHint.SetActive(false);

        image = gameObject.GetComponent<UnityEngine.UI.Image>();
        image.sprite = neutral;
        source = GetComponent<AudioSource>();
        button = GetComponent<UnityEngine.UI.Button>();
    }

    private bool beenResetedAfterMouseMoving = false;

    private void Update()
    {
        if (!button.interactable)
        {
            return;
        }

        if (menuButtonController.mouseInUse && !beenResetedAfterMouseMoving)
        {
            beenResetedAfterMouseMoving = true;
            ToNeutralSprite();
        }

        if (!menuButtonController.mouseInUse)
        {
            beenResetedAfterMouseMoving = false;

            if (menuButtonController.index == thisIndex)
            {
                if (Input.GetAxis("Submit") == 1 )
                {
                    ToPressedSprite();
                    button.onClick.Invoke();
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
    }

    // MouseHandler Methods /Clicking
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!button.IsInteractable())
            return;

        source.PlayOneShot(click);
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
        source.PlayOneShot(hover);

        if (speach != null)
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
        if(selectionHint != null)
            selectionHint.SetActive(false);
    }

    public void ToSelectedSprite()
    {
        image.sprite = selected;
        if (selectionHint != null)
            selectionHint.SetActive(true);
    }

    // Audio clips när man har selected

    public void ToPressedSprite()
    {
        image.sprite = pressed;
    }
}
