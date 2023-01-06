using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuSelection : MonoBehaviour
{
    [SerializeField] private GameObject firstSelectedButton;

    public void Awake()
    {
        if (transform.Find("Continue") && transform.Find("Continue").gameObject.activeSelf)
        {
            GameObject continueButton = transform.Find("Continue").gameObject;
            SelectButton(continueButton);
        }
        else
        {
            SelectButton(firstSelectedButton);
        }
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
