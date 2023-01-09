using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuSelection : MonoBehaviour
{
    [SerializeField] private GameObject firstSelectedButton;

    public void Awake()
    {
        SelectButton(firstSelectedButton);
    }

    public void DeselectButton()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void SelectButton(GameObject button)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(button);
    }
}
