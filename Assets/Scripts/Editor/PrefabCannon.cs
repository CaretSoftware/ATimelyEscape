using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

public class PrefabCannon : EditorWindow
{
    [MenuItem("Tools/Prefab Cannon")]
    public static void OpenPrefabCannon()
    {
        GetWindow<PrefabCannon>();
    }


    public float radius = 2f;
    public int spawnCount = 8;

    SerializedObject so;
    SerializedProperty propRadius;
    SerializedProperty propSpawnCount;

    private Vector2[] randPoints;


    private void OnEnable()
    {
        so = new SerializedObject(this);
        LoadProperties();
        GenerateRandomPoints();
        SceneView.duringSceneGui += DuringSceneGUI;
    }


    private void OnDisable()
    {
        SceneView.duringSceneGui -= DuringSceneGUI;
    }

    private void LoadProperties()
    {
        propRadius = so.FindProperty("radius");
        propSpawnCount = so.FindProperty("spawnCount");
    }

    void DuringSceneGUI(SceneView sceneView)
    {
        Handles.zTest = CompareFunction.LessEqual;
        Transform camTransform = sceneView.camera.transform;

        // make sure it reapints on mouse move
        if(Event.current.type == EventType.MouseMove)
        {
            sceneView.Repaint();
        }
        // finding exact cursor position
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        //Ray ray = new Ray(camTransform.position, camTransform.forward);

        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            // setting up tangent space
            Vector3 hitNormal = hit.normal;
            Vector3 hitTangent = Vector3.Cross(hitNormal, camTransform.forward).normalized;
            Vector3 hitBitangent = Vector3.Cross(hitNormal, hitTangent);

            // drawing points
            foreach(Vector2 point in randPoints)
            {
                // create ray for this point
                Vector3 rayOrigin = hit.point + (hitTangent * point.x + hitBitangent * point.y) * radius;
                rayOrigin += hitNormal * 2; // offset margin
                Vector3 rayDirection = -hitNormal;

                // raycast to find point on surface
                Ray pointRay = new Ray(rayOrigin, rayDirection);
                if(Physics.Raycast(pointRay, out RaycastHit pointHit))
                {
                    // draw sphere and normal on surface
                    DrawSphere(pointHit.point);
                    Handles.DrawAAPolyLine(pointHit.point, pointHit.point + pointHit.normal);
                }
            }
            Handles.DrawAAPolyLine(5, hit.point, hit.point + hit.normal);
            Handles.DrawWireDisc(hit.point, hit.normal, radius);
        }

    }

    private void OnGUI()
    {
        so.Update();
        EditorGUILayout.PropertyField(propRadius);
        propRadius.floatValue = Mathf.Max(1f, propRadius.floatValue);

        EditorGUILayout.PropertyField(propSpawnCount);
        propSpawnCount.intValue = Mathf.Max(1, propSpawnCount.intValue);

        if (so.ApplyModifiedProperties())
        {
            GenerateRandomPoints();
            SceneView.RepaintAll();
        }

        // if editor window was clicked with left mouse button
        if(Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            GUI.FocusControl(null);
            Repaint(); // repaint on editor window UI
        }
    }

    private void GenerateRandomPoints()
    {
        randPoints = new Vector2[spawnCount];

        for(var i = 0; i < spawnCount; i++)
        {
            randPoints[i] = Random.insideUnitCircle;
        }
    }

    private void DrawSphere(Vector3 pos)
    {
        Handles.SphereHandleCap(-1, pos, Quaternion.identity, 0.1f, EventType.Repaint);
    }
}
