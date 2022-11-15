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
    private GameObject textObject;
    private TextMeshProUGUI objectiveText;
    private MeshRenderer mr;
    [HideInInspector] public bool isComplete { get; private set; }
    
    private void Start()
    {
        questlog = GameObject.Find("QuestLog").transform;
        parent = GetComponentInParent<ObjectiveHolder>();
        collider = GetComponent<BoxCollider>();
        objectiveText = new TextMeshProUGUI();
        textObject = new GameObject();
        textObject.AddComponent<TextMeshProUGUI>(objectiveText);
        mr = GetComponent<MeshRenderer>();
        mr.material = idleMaterial;
        collider.isTrigger = true;
        isComplete = false;
        UpdateText();
    }

    private void UpdateText()
    {
        objectiveText.text = Description.ToString();
        objectiveText.fontStyle = FontStyles.Bold;
    }

    public void AddObjective()
    {
        Instantiate<GameObject>(textObject, questlog);
        enabled = true;
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
}
