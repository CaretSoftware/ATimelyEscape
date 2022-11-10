using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveGoal : Quest.QuestGoal
{
    public string objective;

    public override string GetDescription()
    {
        return $"Finished objective: {objective}";
    }

    public override void Initialize()
    {
        base.Initialize();
        //EventManager.Instance.AddListener<ObjectiveEvent>(OnFinishingObjective);
    }

    private void OnFinishingObjective(ObjectiveEvent eventInfo)
    {
        if(eventInfo.ObjectiveName == objective)
        {
            CurrentAmount++;
            Evaluate();
        }
    }
}

