using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Objective : MonoBehaviour
{
    [Header("Objective description")]
    [SerializeField] private string Description;
    
    private TextMeshProUGUI objectiveText;
    private ObjectiveComplete objectiveCompleteComponent;

    [HideInInspector] public Transform objectiveTextTransform;
    [HideInInspector] public bool isComplete { get; set; }
    
    private void Start()
    {
        objectiveText = GetComponent<TextMeshProUGUI>();
        objectiveCompleteComponent = GetComponentInChildren<ObjectiveComplete>();
        isComplete = false;
        objectiveText.text = Description.ToString();
    }
}
