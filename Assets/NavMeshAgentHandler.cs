using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CallbackSystem;

public class NavMeshAgentHandler : MonoBehaviour
{
    private EnemyAI ai;
    [SerializeField] private bool active;
    void Start()
    {
        ai = GetComponent<EnemyAI>();
        TimePeriodChanged.AddListener<TimePeriodChanged>(TimeTravel);
        active = true;
        ai.activeAI = active;
    }
    private void OnDestroy()
    {
        TimePeriodChanged.RemoveListener<TimePeriodChanged>(TimeTravel);
    }

    private void TimeTravel(TimePeriodChanged args)
    {
        //args.to == TimeTravelPeriod.Past
        if (active)
        {
            ai.activeAI = true;
            ai.animator.enabled = true;
            ai.agent.isStopped = false;
        }
        else
        {
            ai.activeAI = false;
            ai.animator.enabled = false;
            ai.agent.isStopped = true;
        }
    }

    /*
    private void Update()
    {
        if (active)
        {
            ai.activeAI = true;
            ai.animator.enabled = true;
            ai.agent.isStopped = false;
        }
        else
        {
            ai.activeAI = false;
            ai.animator.enabled = false;
            ai.agent.isStopped = true;
        }
    }
    */
}
