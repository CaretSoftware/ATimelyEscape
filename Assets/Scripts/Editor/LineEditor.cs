using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LineEditor : EditorWindow
{
    private GameObject line = null;
    private LineRenderer lineRenderer = null;
    private int vertexCount = 0;
    public List<Transform> points;

    SerializedObject so;
    SerializedProperty propPoints;

    [MenuItem("Tools/LineEditor")]
    public static void OpenLevelGenerator()
    {
        GetWindow<LineEditor>();
    }

    private void OnEnable()
    {
        SetPropsToSerialized();
        SceneView.duringSceneGui += DuringSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= DuringSceneGUI;
    }

    private void SetPropsToSerialized()
    {
        so = new SerializedObject(this);

        propPoints = so.FindProperty("points");
    }


    // Update is called once per frame
    /*void Update()
    {
        var pointList = new List<Vector3>();
        for (float ratio = 0; ratio <= 1; ratio += 1.0f / vertexCount)
        {
            var tangentLineVertex1 = Vector3.Lerp(point1.position, point2.position, ratio);
            var tangentLineVertex2 = Vector3.Lerp(point2.position, point3.position, ratio);
            var bezierPoint = Vector3.Lerp(tangentLineVertex1, tangentLineVertex2, ratio);
            pointList.Add(bezierPoint);
        }
        lineRenderer.positionCount = pointList.Count;
        lineRenderer.SetPositions(pointList.ToArray());
    }*/

    void DuringSceneGUI(SceneView sceneView)
    {
        if (points.Count > 1 && points[0] != null && points[1] != null)
        {
            OnDrawHandles();
        }
    }


    private void OnGUI()
    {
        so.Update();
        GUI.skin.button.stretchWidth = true;

        if (line != null)
        {
            GUILayout.Space(20);
            /*GUILayout.BeginHorizontal();
            GUILayout.Space(40);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(propPoints, new GUIContent("Points"));
            if (EditorGUI.EndChangeCheck())
            {
                if()
            }
            GUILayout.Space(40);
            GUILayout.EndHorizontal();*/



        }

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.Space(40);
        if (GUILayout.Button("Create new", GUILayout.Height(25)))
        {
            CreateNewLine();
        }
        GUILayout.Space(40);
        GUILayout.EndHorizontal();
        GUILayout.Space(20);


        if (so.ApplyModifiedProperties())
        {
            SceneView.RepaintAll();
        };

        return;
    }

    private void AddTransformToNull()
    {
        for (int i = 0; i < points.Count; i++)
        {
            if (points[i] == null)
            {
                Debug.Log("I was here");
                GameObject point = new GameObject("Point");
                point.transform.parent = line.gameObject.transform;
                points[i] = point.transform;
            }
        }
    }

    private void CreateNewLine()
    {
        line = new GameObject("Line");
        Undo.RegisterCreatedObjectUndo(line, "Create New Line");

        points = new List<Transform>();
        lineRenderer = line.AddComponent<LineRenderer>();
        /*GameObject point = new GameObject("Point");
        Undo.RegisterCreatedObjectUndo(point, "Create New Line");
        point.transform.parent = line.gameObject.transform;

        points.Add(point);*/
    }

    private void LoadExistingLine()
    {

    }


    private void OnDrawHandles()
    {
        for (int i = 0; i < points.Count - 1; i++)
        {
            if (points[i] && points[i + 1])
            {
                Handles.color = Color.green;
                Handles.DrawLine(points[i].position, points[i + 1].position);

                Handles.color = Color.red;

                if (points[i + 2])
                    for (float ratio = 0.5f / vertexCount; ratio < 1; ratio += 1.0f / vertexCount)
                    {
                        Handles.DrawLine(Vector3.Lerp(points[i].position, points[i + 1].position, ratio), Vector3.Lerp(points[i + 1].position, points[i + 2].position, ratio));
                    }
            }
        }
    }
}
