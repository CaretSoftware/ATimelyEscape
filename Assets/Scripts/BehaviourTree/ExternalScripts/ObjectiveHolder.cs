using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObjectiveHolder : MonoBehaviour
{
    //all objectives for this room.
    [SerializeField] private List<Objective> objectives;
    [SerializeField] private GuideArrow guideArrow;
    [SerializeField] private GameObject canvas;

    //the objective with the lowest index on the list.
    public Vector3 currentObjective { get; private set; }
    
    public void Triggered()
    {
        SetCanvasComponentsActive(true);
        guideArrow.ToggleGuideArrow(true);
        
        for (int i = 0; i < objectives.Count; i++)
            objectives[i].ParentTriggered();

        UpdateCurrentObjectiveOrInactivate();
    }

    //call after an objective is achieved.
    public void UpdateCurrentObjectiveOrInactivate()
    {
        for (int i = 0; i < objectives.Count; i++)
        {
            if (!objectives[i].isComplete)
            {
                currentObjective = objectives[i].transform.position;
                guideArrow.SetTarget(objectives[i].transform);
                return;
            }
        }
        guideArrow.ToggleGuideArrow(false);
        SetCanvasComponentsActive(false);
    }

    public void SetCanvasComponentsActive(bool arg)
    {
        if (objectives.Count == 0 || objectives == null)
            return;

        for (int i = 0; i < objectives.Count; i++)
            objectives[i].SetObjectiveActive(arg);
        SetCanvasActive(arg);
    }

    public void SetCanvasActive(bool arg)
    {
        if(canvas != null)
            canvas.gameObject.SetActive(arg);
    }
}
