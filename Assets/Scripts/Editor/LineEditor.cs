using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LineEditor : EditorWindow
{
    private GameObject line = null;
    private LineRenderer lineRenderer = null;
    public List<GameObject> points;
    public AnimationCurve lineWidth = null;
    public int vertexCount = 12;


    SerializedObject so;
    SerializedProperty propLineWidth;
    SerializedProperty propVertexCount;

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
        line = null;
        points = null;
        SceneView.duringSceneGui -= DuringSceneGUI;
    }

    private void SetPropsToSerialized()
    {
        so = new SerializedObject(this);

        propLineWidth = so.FindProperty("lineWidth");
        propVertexCount = so.FindProperty("vertexCount");
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
        if (line != null)
        {
            if (points.Count > 1)
            {
                OnDrawHandles();
            }
        }
    }


    private void OnGUI()
    {
        so.Update();
        GUI.skin.button.stretchWidth = true;

        if (line != null)
        {
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Space(40);
            if (GUILayout.Button("Add point", GUILayout.Height(25)))
            {
                AddPoint();
            }
            GUILayout.Space(10);

            if (GUILayout.Button("Remove point", GUILayout.Height(25)))
            {
                RemovePoint();
            }
            GUILayout.Space(40);
            GUILayout.EndHorizontal();

            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Space(40);
            if (GUILayout.Button("Generate Line", GUILayout.Height(25)))
            {
                GenerateLine();
            }
            GUILayout.Space(40);
            GUILayout.EndHorizontal();
            GUILayout.Space(20);
           
           
            EditorGUI.indentLevel += 2;
            EditorGUILayout.LabelField("Line width");
            EditorGUI.indentLevel -= 2;
            GUILayout.BeginHorizontal();
            GUILayout.Space(40);
            if(lineRenderer != null)
            {
            lineRenderer.widthCurve = EditorGUILayout.CurveField(lineWidth);
            }
            GUILayout.Space(40);
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Space(40);
            
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(propVertexCount, new GUIContent("Vertex count"));
            propVertexCount.intValue = Mathf.Max(0, propVertexCount.intValue);
            propVertexCount.intValue = Mathf.Min(100, propVertexCount.intValue);

            GUILayout.Space(40);
            GUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                GenerateLine();
            }
           

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

    private void AddPoint()
    {
        GameObject point = new GameObject("Point");
        Undo.RegisterCreatedObjectUndo(point, "Create New Line");
        point.transform.parent = line.gameObject.transform;
        point.transform.position = line.gameObject.transform.position;
        points.Add(point);
    }

    private void RemovePoint()
    {
        if (points.Count > 0)
        {
            GameObject point = null;
            GameObject selectedObject = Selection.activeGameObject;
            if (selectedObject && selectedObject.transform.parent && selectedObject.transform.parent.tag == "Line")
            {
                point = selectedObject;
            }
            else
            {
                point = points[points.Count - 1];
            }
            points.Remove(point);
            DestroyImmediate(point);
        }
    }

    private void GenerateLine()
    {
        var pointList = new List<Vector3>();

        for (int i = 0; i < points.Count - 2; i += 2)
        {
            for (float ratio = 0; ratio <= 1; ratio += 1.0f / vertexCount)
            {
                var tangentLineVertex1 = Vector3.Lerp(points[i].transform.position, points[i + 1].transform.position, ratio);
                var tangentLineVertex2 = Vector3.Lerp(points[i + 1].transform.position, points[i + 2].transform.position, ratio);
                var bezierPoint = Vector3.Lerp(tangentLineVertex1, tangentLineVertex2, ratio);
                pointList.Add(bezierPoint);
            }
            lineRenderer.positionCount = pointList.Count;
            lineRenderer.SetPositions(pointList.ToArray());
        }
    }


    private void CreateNewLine()
    {
        line = new GameObject("Line");
        Undo.RegisterCreatedObjectUndo(line, "Create New Line");
        line.tag = "Line";

        points = new List<GameObject>();
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
            Handles.color = Color.green;
            Handles.DrawLine(points[i].transform.position, points[i + 1].transform.position);

        }

        for (int i = 0; i < points.Count - 2; i += 2)
        {
            Handles.color = Color.red;
            for (float ratio = 0.5f / vertexCount; ratio < 1; ratio += 1.0f / vertexCount)
            {
                Handles.DrawLine(Vector3.Lerp(points[i].transform.position, points[i + 1].transform.position, ratio), Vector3.Lerp(points[i + 1].transform.position, points[i + 2].transform.position, ratio));
            }
        }
    }
}
