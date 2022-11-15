using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveHolder : MonoBehaviour
{
    [SerializeField] private List<Objective> objectives;
    [SerializeField] private GameObject triggerObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            for (int i = 0; i < objectives.Count; i++)
                if (!objectives.Contains(objectives[i]))
                    Instantiate<Objective>(objectives[i], transform);
    }

    public void CheckIfDone()
    {
        for (int i = 0; i < objectives.Count; i++)
        {
            if (!objectives[i].isComplete)
                return;
            else
                objectives.Clear();
        }
    }
}
