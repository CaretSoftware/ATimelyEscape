using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PatrolNavAgent : MonoBehaviour
{
    private const float distCheck = 0.1f;
    [SerializeField] private Transform[] patrolPoints;
    [Range(0, 10)][SerializeField] private float speed;
    private NavMeshAgent agent;
    private int targetIndex = 0;
    private int prevIndex;
    
    private System.Random random = new();
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
    }

    private void Update()
    {
        if (Vector3.Distance(agent.transform.position, patrolPoints[targetIndex].position) < distCheck)
        {
            UpdatePatrolPoint();
            prevIndex = targetIndex;
        }
        else
            agent.SetDestination(patrolPoints[targetIndex].position);
    }
    
    private void UpdatePatrolPoint()
    {
        int value = random.Next(0, patrolPoints.Length);
        if (value == prevIndex)
            UpdatePatrolPoint();
        else
            targetIndex = value;
    }
}
