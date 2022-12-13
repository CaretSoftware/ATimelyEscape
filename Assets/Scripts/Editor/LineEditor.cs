using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LineEditor : EditorWindow
{
    private GameObject line = null;
    private LineRenderer lineRenderer = null;
    public List<GameObject> points;
    public int vertexCount = 12;
    private Camera camTf = null;
    
    SerializedObject so;
    SerializedProperty propVertexCount;


    [MenuItem("Tools/LineEditor")]
    public static void OpenLineEditor()
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

        propVertexCount = so.FindProperty("vertexCount");
    }

    void DuringSceneGUI(SceneView sceneView)
    {
        if (!camTf)
        {
            camTf = sceneView.camera;
        }

        SetLineSelected();

        if (line != null)
        {
            if (points != null && points.Count > 1)
            { 
                LoadExistingLine(line);
                GenerateLine();
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
                GenerateLine();
            }
            GUILayout.Space(10);

            if (GUILayout.Button("Remove point", GUILayout.Height(25)))
            {
                RemovePoint();
                if(points.Count > 1)
                {
                    GenerateLine();
                }
            }
            GUILayout.Space(40);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Space(40);
            EditorGUILayout.PropertyField(propVertexCount, new GUIContent("Vertex count"));
            propVertexCount.intValue = Mathf.Max(0, propVertexCount.intValue);
            propVertexCount.intValue = Mathf.Min(100, propVertexCount.intValue);
            GUILayout.Space(40);
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
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

        /*GUILayout.BeginHorizontal();
        GUILayout.Space(40);
        if (GUILayout.Button("Load selected", GUILayout.Height(25)))
        {
            SetLineSelected();
        }
        GUILayout.Space(40);
        GUILayout.EndHorizontal();
        GUILayout.Space(20);*/


        if (so.ApplyModifiedProperties())
        {
            SceneView.RepaintAll();
        };

        return;
    }

    
    private void SetLineSelected()
    {
        GameObject selectedObject = Selection.activeGameObject;
        if (selectedObject && selectedObject != line && selectedObject.transform.CompareTag("Line"))
        {
            line = selectedObject;
            //LoadExistingLine(selectedObject);
        }
    }

    private void AddPoint()
    {
        GameObject point = new GameObject("Point");
        Undo.RegisterCreatedObjectUndo(point, "Create New Line");
        point.transform.parent = line.gameObject.transform;
        if(points.Count > 0)
        {
            point.transform.position = points[points.Count - 1].transform.position;
        }
        else
        {
            point.transform.position = line.transform.position;
        }
        points.Add(point);
    }

    private void RemovePoint()
    {
        if (points.Count > 0)
        {
            GameObject point = null;
            GameObject selectedObject = Selection.activeGameObject;
            if (selectedObject && selectedObject.transform.parent && selectedObject.transform.parent.CompareTag("Line"))
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
                if (points[i] && points[i + 1] && points[i + 2])
                {
                    var tangentLineVertex1 = Vector3.Lerp(points[i].transform.position, points[i + 1].transform.position, ratio);
                    var tangentLineVertex2 = Vector3.Lerp(points[i + 1].transform.position, points[i + 2].transform.position, ratio);
                    var bezierPoint = Vector3.Lerp(tangentLineVertex1, tangentLineVertex2, ratio);
                    pointList.Add(bezierPoint);
                }
            }
            lineRenderer.positionCount = pointList.Count;
            lineRenderer.SetPositions(pointList.ToArray());
        }
    }


    private void CreateNewLine()
    {
        line = new GameObject("Line");
        line.AddComponent<SpriteRenderer>();
        Undo.RegisterCreatedObjectUndo(line, "Create New Line");
        line.transform.position = camTf.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.7f));

        line.tag = "Line";

        points = new List<GameObject>();
        lineRenderer = line.AddComponent<LineRenderer>();

        var width = 0.05f;
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0, width);
        curve.AddKey(1, width);
        lineRenderer.widthCurve = curve;
    }

    private void LoadExistingLine(GameObject selectedObject)
    {
        line = selectedObject;
        lineRenderer = line.GetComponent<LineRenderer>();

        points = new List<GameObject>();
        foreach (Transform child in line.transform)
        {
        points.Add(child.gameObject);
        }
    }


    private void OnDrawHandles()
    {
        for (int i = 0; i < points.Count - 1; i++)
        {
            Handles.color = Color.green;
            if(points[i] && points[i + 1])
            {
                Handles.DrawLine(points[i].transform.position, points[i + 1].transform.position);
            }

        }

        for (int i = 0; i < points.Count - 2; i += 2)
        {
            Handles.color = Color.red;
            for (float ratio = 0.5f / vertexCount; ratio < 1; ratio += 1.0f / vertexCount)
            {
                if (points[i] && points[i + 1] && points[i + 2])
                {
                    Handles.DrawLine(Vector3.Lerp(points[i].transform.position, points[i + 1].transform.position, ratio), Vector3.Lerp(points[i + 1].transform.position, points[i + 2].transform.position, ratio));
                }
            }
        }
    }

    
    private bool PointIsNull()
    {
        for(int i = 0; i < points.Count; i++)
        {
            if(points[i] == null)
            {
                return true;
            }
        }

        return false;
    }
}
