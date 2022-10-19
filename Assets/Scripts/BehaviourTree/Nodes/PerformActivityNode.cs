using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PerformActivityNode : Node
{
    private NavMeshAgent agent;
    private static DummyBehaviour Instance;
    GameObject GO = new GameObject();
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
            agent.isStopped = false;
            return NodeState.RUNNING;
        }
        else
        {
            agent.isStopped = true;
            return NodeState.SUCCESS;
        }
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(3f);
        timerDone = true;
    }

    private class DummyBehaviour : MonoBehaviour{}
}
