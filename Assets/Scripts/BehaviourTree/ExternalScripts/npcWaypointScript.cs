using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npcWaypointScript : MonoBehaviour
{
    public float gizmoRadius = 0.2f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, gizmoRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(transform.position, new Vector3(gizmoRadius, gizmoRadius, gizmoRadius));
    }
}
