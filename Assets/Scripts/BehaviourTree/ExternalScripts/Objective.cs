using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//Holds two crucial components, a collider to interact with and a textMeshProUGUI for the objective list.
[RequireComponent(typeof(BoxCollider))]
public class Objective : MonoBehaviour
{
    [SerializeField] private string Description;
    [SerializeField] private Material idleMaterial;
    [SerializeField] private Material completeMaterial;
    private ObjectiveHolder parent;
    private Transform questlog;
    private BoxCollider collider;
    private TextMeshProUGUI objectiveText;
    private MeshRenderer mr;
    [HideInInspector] public bool isComplete { get; private set; }
    
    private void Start()
    {
        questlog = GameObject.Find("QuestLog").transform;
        parent = GetComponentInParent<ObjectiveHolder>();
        collider = GetComponent<BoxCollider>();
        mr = GetComponent<MeshRenderer>();
        mr.material = idleMaterial;
        collider.isTrigger = true;
        isComplete = false;
        gameObject.SetActive(false);
    }
    
    private void UpdateTextObject()
    {
        objectiveText.fontSize = 18;
        objectiveText.fontStyle = FontStyles.Bold;
        objectiveText.alignment = TextAlignmentOptions.Center;   
        objectiveText.text = Description;
    }
    
    public void AddObjective()
    {
        objectiveText = new GameObject($"{name} textObject").AddComponent<TextMeshProUGUI>();
        objectiveText.transform.parent = questlog;
        UpdateTextObject();
        gameObject.SetActive(true);
    }

    //if player collide with this transform, mission is complete & objective list is updated.
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && transform.position == parent.currentObjective)
        {
            isComplete = true;
            objectiveText.fontStyle = FontStyles.Strikethrough;
            mr.material = completeMaterial;
            parent.UpdateObjectiveList();
        }
    }

    public void Remove()
    {
        Destroy(objectiveText);
        Destroy(this);
    }
}
