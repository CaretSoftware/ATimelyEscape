using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LineEditor : EditorWindow
{
    public int vertexCount = 12;

    private List<GameObject> _points;
    private GameObject _line = null;
    private LineRenderer _lineRenderer = null;
    private Camera _camTf = null;
    private SerializedObject so;
    private SerializedProperty propVertexCount;


    [MenuItem("Tools/LineEditor")]
    public static void OpenLineEditor()
    {
        GetWindow<LineEditor>();
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += DuringSceneGUI;
        SetPropsToSerialized();
        LoadSettings();
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= DuringSceneGUI;
        SaveSettings();
        _line = null;
        _points = null;
    }

    private void SaveSettings()
    {
        EditorPrefs.SetInt("LINEEDITOR_VertexCount", vertexCount);
    }

    private void LoadSettings()
    {
        vertexCount = EditorPrefs.GetInt("LINEEDITOR_VertexCount", 20);
    }

    private void SetPropsToSerialized()
    {
        so = new SerializedObject(this);
        propVertexCount = so.FindProperty("vertexCount");
    }

    void DuringSceneGUI(SceneView sceneView)
    {
        if (!_camTf)
        {
            _camTf = sceneView.camera;
        }

        SetLineSelected();

        if (_line != null)
        {
            if (_points != null && _points.Count > 1)
            {
                LoadExistingLine(_line);
                GenerateLine();
                OnDrawHandles();
            }
        }

    }

    private void OnGUI()
    {
        so.Update();
        GUI.skin.button.stretchWidth = true;

        if (_line != null)
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
                if (_points.Count > 1)
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
        if (GUILayout.Button("Load selected", GUILayout.Height(25)))
        {
            SetLineSelected();
            LoadExistingLine(_line);
        }
        GUILayout.Space(40);
        GUILayout.EndHorizontal();

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


    private void SetLineSelected()
    {
        GameObject selectedObject = Selection.activeGameObject;
        if (selectedObject && selectedObject != _line && selectedObject.transform.CompareTag("Line"))
        {
            _line = selectedObject;
        }
    }

    private void AddPoint()
    {
        GameObject point = new GameObject("Point");
        Undo.RegisterCreatedObjectUndo(point, "Create New Line");
        point.transform.parent = _line.gameObject.transform;
        if (_points.Count > 0)
        {
            point.transform.position = _points[_points.Count - 1].transform.position;
        }
        else
        {
            point.transform.position = _line.transform.position;
        }
        _points.Add(point);
    }

    private void RemovePoint()
    {
        if (_points.Count > 0)
        {
            GameObject point = null;
            GameObject selectedObject = Selection.activeGameObject;
            if (selectedObject && selectedObject.transform.parent && selectedObject.transform.parent.CompareTag("Line"))
            {
                point = selectedObject;
            }
            else
            {
                point = _points[_points.Count - 1];
            }
            _points.Remove(point);
            DestroyImmediate(point);
        }
    }

    private void GenerateLine()
    {
        var pointList = new List<Vector3>();

        for (int i = 0; i < _points.Count - 2; i += 2)
        {
            for (float ratio = 0; ratio <= 1; ratio += 1.0f / vertexCount)
            {
                if (_points[i] && _points[i + 1] && _points[i + 2])
                {
                    var tangentLineVertex1 = Vector3.Lerp(_points[i].transform.position, _points[i + 1].transform.position, ratio);
                    var tangentLineVertex2 = Vector3.Lerp(_points[i + 1].transform.position, _points[i + 2].transform.position, ratio);
                    var bezierPoint = Vector3.Lerp(tangentLineVertex1, tangentLineVertex2, ratio);
                    pointList.Add(bezierPoint);
                }
            }
            _lineRenderer.positionCount = pointList.Count;
            _lineRenderer.SetPositions(pointList.ToArray());
        }
    }

    private void CreateNewLine()
    {
        _line = new GameObject("Line");
        _line.AddComponent<SpriteRenderer>();
        Undo.RegisterCreatedObjectUndo(_line, "Create New Line");
        _line.transform.position = _camTf.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.7f));

        _line.tag = "Line";
        _points = new List<GameObject>();
        _lineRenderer = _line.AddComponent<LineRenderer>();

        var width = 0.05f;
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0, width);
        curve.AddKey(1, width);
        _lineRenderer.widthCurve = curve;
    }

    private void LoadExistingLine(GameObject selectedObject)
    {
        if (!selectedObject)
        {
            return;
        }

        _line = selectedObject;
        _lineRenderer = _line.GetComponent<LineRenderer>();
        _points = new List<GameObject>();

        foreach (Transform child in _line.transform)
        {
            _points.Add(child.gameObject);
        }
    }

    private void OnDrawHandles()
    {
        for (int i = 0; i < _points.Count - 1; i++)
        {
            Handles.color = Color.green;
            if (_points[i] && _points[i + 1])
            {
                Handles.DrawLine(_points[i].transform.position, _points[i + 1].transform.position);
            }

        }

        for (int i = 0; i < _points.Count - 2; i += 2)
        {
            Handles.color = Color.red;
            for (float ratio = 0.5f / vertexCount; ratio < 1; ratio += 1.0f / vertexCount)
            {
                if (_points[i] && _points[i + 1] && _points[i + 2])
                {
                    Handles.DrawLine(Vector3.Lerp(_points[i].transform.position, _points[i + 1].transform.position, ratio), Vector3.Lerp(_points[i + 1].transform.position, _points[i + 2].transform.position, ratio));
                }
            }
        }
    }

}
