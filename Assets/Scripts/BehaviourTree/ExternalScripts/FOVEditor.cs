using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyFOV))]
public class FOVEditor : Editor
{
    private Vector3 viewAngle01, viewAngle02;
    private void OnSceneGUI()
    {
        EnemyFOV fov = (EnemyFOV)target;
        Handles.color = Color.yellow;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.ChaseRadius);

        viewAngle01 = DirectionFromAngle(fov.transform.eulerAngles.y, -fov.Angle / 2);
        viewAngle02 = DirectionFromAngle(fov.transform.eulerAngles.y, fov.Angle / 2);

        Handles.color = Color.gray;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.FacingViewRange);
        Handles.color = Color.yellow;
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle01 * fov.FacingViewRange);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle02 * fov.FacingViewRange);
        
        if(fov.playerDetected)
        {
            Handles.color = Color.blue;
            Handles.DrawLine(fov.transform.position, fov.player.position);
        }
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
