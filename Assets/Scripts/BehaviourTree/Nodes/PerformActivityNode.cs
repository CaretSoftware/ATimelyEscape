using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PerformActivityNode : Node
{
    private static DummyBehaviour Instance;
    private GameObject GO = new GameObject();

    private NavMeshAgent agent;
    private bool timerDone;

    public PerformActivityNode(NavMeshAgent agent)
    {
        this.agent = agent;
        Instance = GO.AddComponent<DummyBehaviour>();
    }

public override NodeState Evaluate()
    {
        if (!timerDone)
        {
            Instance.StartCoroutine(Timer());
            agent.isStopped = true;
            return NodeState.RUNNING;
        }
        else
        {
            agent.isStopped = false;
            return NodeState.SUCCESS;
        }
    }

    private IEnumerator Timer()
    {
        timerDone = false;
        yield return new WaitForSeconds(3f);
        timerDone = true;
    }

    private class DummyBehaviour : MonoBehaviour{}
}
