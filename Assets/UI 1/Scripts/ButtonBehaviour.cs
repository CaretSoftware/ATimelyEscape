using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonBehaviour : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Image image;

    [SerializeField] private Sprite neutral, pressed, selected;
    // [SerializeField] private AudioClip compressClip, unCompressClip;
    // [SerializeField] private AudioSource audioSource; 

    // Method when user has pressed the mousebutton

    private void Start()
    {
        image = gameObject.GetComponent<Image>();
        image.sprite = neutral;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        image.sprite = pressed;
    }

    // Method when user has released the mousebutton
    public void OnPointerUp(PointerEventData eventData)
    {
        image.sprite = neutral;
    }

    public void OnClicked()
    {
        Debug.Log("Clicked!");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.sprite = selected;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.sprite = neutral;
    }
}
