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
    [SerializeField] private Transform questlog;
    private BoxCollider boxCollider;
    private TextMeshProUGUI objectiveText;
    private MeshRenderer mr;
    private RectTransform rt;
    private Vector2 objectiveSizeUI = new Vector2(200, 20);
    public bool isComplete { get; private set; }

    private void Start()
    {
        parent = GetComponentInParent<ObjectiveHolder>();
        boxCollider = GetComponent<BoxCollider>();
        mr = GetComponent<MeshRenderer>();
        mr.material = idleMaterial;
        boxCollider.isTrigger = true;
        isComplete = false;
    }

    public void ParentTriggered()
    {
        CreateAndAddObjectiveTextComponentToCanvas();
        AssignObjectiveTextValues();
        SetObjectiveActive(true);
    }

    private void AssignObjectiveTextValues()
    {
        objectiveText.fontSize = 16;
        objectiveText.fontStyle = FontStyles.Bold;
        objectiveText.alignment = TextAlignmentOptions.Center;
        rt = objectiveText.GetComponent<RectTransform>();
        rt.sizeDelta = objectiveSizeUI;
        objectiveText.text = Description;
    }

    public void CreateAndAddObjectiveTextComponentToCanvas()
    {
        objectiveText = new GameObject($"{name} textObject").AddComponent<TextMeshProUGUI>();
        objectiveText.transform.SetParent(questlog);
        AssignObjectiveTextValues();
    }

    //if player collide with this transform, mission is complete & objective list is updated.
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && transform.position == parent.currentObjective)
        {
            isComplete = true;
            objectiveText.fontStyle = FontStyles.Strikethrough;
            mr.material = completeMaterial;
            parent.UpdateCurrentObjectiveOrInactivate();
        }
    }

    public void SetObjectiveActive(bool arg)
    {
        if (objectiveText == null || gameObject == null)
            return;
        objectiveText.gameObject.SetActive(arg);
        gameObject.SetActive(arg);
    }
}