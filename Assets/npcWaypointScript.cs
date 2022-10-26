using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npcWaypointScript : MonoBehaviour
{
    public float gizmoRadius = 0.3f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, gizmoRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, new Vector3(gizmoRadius, gizmoRadius, gizmoRadius));
    }
}
