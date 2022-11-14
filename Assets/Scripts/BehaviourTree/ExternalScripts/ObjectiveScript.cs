using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ObjectiveScript : MonoBehaviour
{
    [SerializeField] private string Description;
    [SerializeField] private GameObject objectiveTextObject;
    
    private TextMeshProUGUI objectiveText;
    private GameObject completedText;

    [HideInInspector] public Transform objectiveTextTransform;
    [HideInInspector] public bool complete;
    
    private void Start()
    {
        complete = false;
        objectiveText = GetComponent<TextMeshProUGUI>();
        objectiveText.text = Description.ToString();
        objectiveTextObject.SetActive(false);
        completedText.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            objectiveTextObject.SetActive(true);
            completedText.SetActive(true);
        }
    }
}
