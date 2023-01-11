using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObjectiveHolder : MonoBehaviour
{
    //all objectives for this room.
    [SerializeField] private List<Objective> objectives;
    //private BoxCollider boxCollider;
    //private Material material;

    //the objective with the lowest index on the list.
    public Vector3 currentObjective { get; private set; }

    private void Start()
    {
        //GetChildren();
    }

    /*

private void GetChildren()
{
    GameObject child;
    for (int i = 0; i < transform.childCount; i++)
    {
        child = transform.GetChild(i).gameObject;
        objectives.Add(child.GetComponent<Objective>());
    }
}

//when triggered, update the questlog.
private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player"))
    {
        for (int i = 0; i < objectives.Count; i++)
            objectives[i].AddObjective();
        UpdateObjectiveList();
        //GuideArrow.Instance.ToggleGuideArrow(true);
    }
}
*/

    public void Triggered()
    {
        for (int i = 0; i < objectives.Count; i++)
        {
            objectives[i].SetCanvasActive(true);
            objectives[i].AddObjectiveTextComponent();
        }
            UpdateObjectiveList();
            //GuideArrow.Instance.ToggleGuideArrow(true);
    }

    //call after an objective is achieved.
    public void UpdateObjectiveList()
    {
        for (int i = 0; i < objectives.Count; i++)
        {
            if (objectives[i].isComplete)
                continue;
            else
            {
                currentObjective = objectives[i].transform.position;
                //GuideArrow.Instance.SetTarget(objectives[i].transform);
                return;
            }
        }
        ClearList();
        //GuideArrow.Instance.ToggleGuideArrow(false);
        
    }

    public void ClearList()
    {
        if (objectives.Count == 0 || objectives == null)
            return;
        foreach (Objective obj in objectives)
        {
            obj.SetObjectiveAtive(false);
            obj.SetCanvasActive(false);
        }
        
        print("Clear list");
    }
}
