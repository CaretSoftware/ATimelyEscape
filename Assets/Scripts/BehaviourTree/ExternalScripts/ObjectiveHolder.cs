using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ObjectiveHolder : MonoBehaviour
{
    //all objectives for this room.
    private List<Objective> objectives;
    private BoxCollider boxCollider;

    //the objective with the lowest index on the list.
    public Vector3 currentObjective { get; private set; }

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        objectives = new List<Objective>();
        boxCollider.isTrigger = true;
        GetChildren();
    }

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
        if (other.tag == "Player")
        {
            for (int i = 0; i < objectives.Count; i++)
                objectives[i].AddObjective();
            UpdateObjectiveList();
            GuideArrow.Instance.ToggleGuideArrow(true);
        }
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
                GuideArrow.Instance.SetTarget(objectives[i].transform);
                return;
            }
        }
        foreach (Objective obj in objectives)
            obj.ClearObjective();
        print("Clear list");
        GuideArrow.Instance.ToggleGuideArrow(false);
        
    }
}
