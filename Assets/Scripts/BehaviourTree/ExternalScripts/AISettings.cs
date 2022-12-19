using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISettings : MonoBehaviour
{
    
    
    private EnemyAI[] scientists;
    private PatrolNavAgent[] roombas;

    private void Start()
    {
        scientists = FindObjectsOfType<EnemyAI>();
        roombas = FindObjectsOfType<PatrolNavAgent>();
    }

    public void ScientistChaseRange(float range)
    {
        foreach (var scientist in scientists)
            scientist.ChaseRange = range;
    }

    public void ScientistCaptureRange(float range)
    {
        foreach (var scientist in scientists)
            scientist.CaptureRange = range;
    }

    public void RoombaMovementSpeed(float speed)
    {
        foreach (var roomba in roombas)
            roomba.MovementSpeed = speed;
    }
}
