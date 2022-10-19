using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CaptureNode : Node
{
    [SerializeField] private float captureDistance;
    private NavMeshAgent agent;
    private Transform target;
    public CaptureNode(NavMeshAgent agent, Transform target)
    {
        this.agent = agent;
        this.target = target;
    }

    public override NodeState Evaluate()
    {
        float dist = Vector3.Distance(target.position, agent.transform.position);
        if (dist > captureDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
            return NodeState.RUNNING;
        }
        else
        {
            agent.isStopped = true;
            return NodeState.SUCCESS;
        }
    }

    private IEnumerator LookAtTarget()
    {
        Quaternion lookRotation = Quaternion.LookRotation(target.position - agent.transform.position);
        float time = 0;

        while(time < 1)
        {
            agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, lookRotation, time);
            time += Time.deltaTime * 2;
            yield return null;
        }
        agent.transform.rotation = lookRotation;
    }
}
