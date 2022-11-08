using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonBehaviour : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Image image;
    private Button button;

    [SerializeField] private Sprite neutral, pressed, selected;
    // [SerializeField] private AudioClip compressClip, unCompressClip;
    // [SerializeField] private AudioSource audioSource; 

    // Method when user has pressed the mousebutton

    private void Start()
    {
        button = gameObject.GetComponent<Button>();
        image = gameObject.GetComponent<Image>();
        image.sprite = neutral;
    }

    // MouseHandler Methods /Clicking
    public void OnPointerDown(PointerEventData eventData)
    {
        ToPressedSprite();
    }

    // Method when user has released the mousebutton
    public void OnPointerUp(PointerEventData eventData)
    {
        ToSelectedSprite();
    }

    public void OnClicked()
    {
        Debug.Log("Clicked!");
    }

    // Hover sound

    public void OnPointerEnter(PointerEventData eventData)
    {
        ToSelectedSprite();
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
