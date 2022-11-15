using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ObjectiveHolder : MonoBehaviour
{
    //all objectives for this room.
    [SerializeField] private List<Objective> objectives;

    private BoxCollider collider;

    //the objective with the lowest index on the list.
    [HideInInspector] public Vector3 currentObjective { get; private set; }

    private void Start()
    {
        collider = GetComponent<BoxCollider>();
        collider.isTrigger = true;
    }
    /*
    private void GetChildren()
    {
        for (int i = 0; i < transform.childCount; i++)
            objectives[i] = transform.GetChild(i);
    }
    */

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
        print("Clear list");
        //objectives.Clear();
        //destroy objective;
    }
}
