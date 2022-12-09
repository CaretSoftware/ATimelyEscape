using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ObjectiveHolder : MonoBehaviour
{
    //all objectives for this room.
    private List<Objective> objectives;
    private BoxCollider boxCollider;

    //the objective with the lowest index on the list.
    [HideInInspector] public Vector3 currentObjective { get; private set; }

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
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
                return;
            }
        }
        foreach (Objective obj in objectives)
            obj.ClearObjective();
        print("Clear list");
    }
}
