using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;


public struct SpawnData
{
    public Vector2 pointInDisc;
    public float randAngDeg;
    public GameObject prefab;

    public void SetRandomValues(GameObject[] prefabs, bool hasRandomRotation, bool areDecals)
    {
        pointInDisc = Random.insideUnitCircle;

        if (areDecals)
        {
            randAngDeg = 90f;
        }else{
            randAngDeg = hasRandomRotation ? Random.value * 360 : 0;
        }

        prefab = prefabs.Length == 0 ? null : prefabs[Random.Range(0, prefabs.Length)];
    }
}

public class SpawnPoint
{
    public SpawnData spawnData;
    public Vector3 position;
    public Quaternion rotation;

    public SpawnPoint(Vector3 position, Quaternion rotation, SpawnData spawnData)
    {
        this.spawnData = spawnData;
        this.position = position;
        this.rotation = rotation;
    }
}


public class PrefabCannon : EditorWindow
{
    [MenuItem("Tools/Prefab Cannon")]
    public static void OpenPrefabCannon()
    {
        GetWindow<PrefabCannon>();
    }

    public float radius = 2f;
    public int spawnCount = 8;
    public Material previewMaterial;
    public bool hasRandomRotation;
    public bool areDecals;
    public GameObject[] prefabs;
    private SpawnData[] spawnDataPoints;


    SerializedObject so;
    SerializedProperty propRadius;
    SerializedProperty propSpawnCount;
    SerializedProperty propSpawnPrefabs;
    SerializedProperty propPreviewMaterial;
    SerializedProperty propHasRandomRotation;
    SerializedProperty propAreDecals;



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
        propSpawnPrefabs = so.FindProperty("prefabs");
        propPreviewMaterial = so.FindProperty("previewMaterial");
        propHasRandomRotation = so.FindProperty("hasRandomRotation");
        propAreDecals = so.FindProperty("areDecals");
    }


    void DuringSceneGUI(SceneView sceneView)
    {
        Handles.zTest = CompareFunction.LessEqual;

        // make sure it repaints on mouse move
        if (Event.current.type == EventType.MouseMove)
        {
            sceneView.Repaint();
        }

        // change radius
        bool holdingAlt = (Event.current.modifiers & EventModifiers.Alt) != 0;
        if (Event.current.type == EventType.ScrollWheel && holdingAlt)
        {
            float scrollDir = Mathf.Sign(Event.current.delta.y);

            so.Update();
            propRadius.floatValue *= 1f - scrollDir * 0.05f;
            so.ApplyModifiedProperties();
            Event.current.Use();
        }

        // if the cursor is pointing on valid ground
        Transform camTransform = sceneView.camera.transform;
        if (TryRaycastFromCamera(camTransform.up, out Matrix4x4 tangentToWorld))
        {
            List<SpawnPoint> spawnPoints = GetSpawnPoints(tangentToWorld);

            if (Event.current.type == EventType.Repaint)
            {
                DrawCircleRegion(tangentToWorld);
                DrawSpawnPreviews(spawnPoints, sceneView.camera);
            }

            // spawn on press
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.D)
            {
                TrySpawnObjects(spawnPoints);
            }
        }
    }


    private void OnGUI()
    {
        so.Update();
        GUI.skin.button.stretchWidth = true;

        EditorGUI.indentLevel++;
        GUILayout.Space(40);

        EditorGUILayout.PropertyField(propRadius);
        propRadius.floatValue = Mathf.Max(1f, propRadius.floatValue);
        GUILayout.Space(5);

        EditorGUILayout.PropertyField(propSpawnCount);
        propSpawnCount.intValue = Mathf.Max(1, propSpawnCount.intValue);
        GUILayout.Space(5);

        EditorGUILayout.PropertyField(propHasRandomRotation);
        GUILayout.Space(5);

        EditorGUILayout.PropertyField(propAreDecals);
        GUILayout.Space(10);

        EditorGUILayout.PropertyField(propSpawnPrefabs);
        GUILayout.Space(10);

        EditorGUILayout.PropertyField(propPreviewMaterial);


        if (so.ApplyModifiedProperties())
        {
            GenerateRandomPoints();
            SceneView.RepaintAll();
        }

        // if editor window was clicked with left mouse button
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            GUI.FocusControl(null);
            Repaint(); // repaint on editor window UI
        }
    }


    private void TrySpawnObjects(List<SpawnPoint> spawnPoints)
    {
        if (prefabs == null || prefabs.Length == 0)
        {
            return;
        }

        foreach (SpawnPoint spawnPoint in spawnPoints)
        {
            if (spawnPoint.spawnData.prefab != null)
            {
                GameObject spawnedPrefab = (GameObject)PrefabUtility.InstantiatePrefab(spawnPoint.spawnData.prefab);
                Undo.RegisterCreatedObjectUndo(spawnedPrefab, "Spawn Objects");
                spawnedPrefab.transform.position = spawnPoint.position;
                spawnedPrefab.transform.rotation = spawnPoint.rotation;
            }
        }

        GenerateRandomPoints(); // update points
    }


    private bool TryRaycastFromCamera(Vector2 cameraUp, out Matrix4x4 tangentToWorldMtx)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // setting up tangent space
            Vector3 hitNormal = hit.normal;
            Vector3 hitTangent = Vector3.Cross(hitNormal, cameraUp).normalized;
            Vector3 hitBitangent = Vector3.Cross(hitNormal, hitTangent);
            tangentToWorldMtx = Matrix4x4.TRS(hit.point, Quaternion.LookRotation(hitNormal, hitBitangent), Vector3.one);
            return true;
        }

        tangentToWorldMtx = default;
        return false;
    }


    private List<SpawnPoint> GetSpawnPoints(Matrix4x4 tangentToWorld)
    {
        List<SpawnPoint> hitSpawnPoints = new List<SpawnPoint>();
        foreach (SpawnData rndDataPoint in spawnDataPoints)
        {
            // create ray for this point
            Ray ptRay = GetCircleRay(tangentToWorld, rndDataPoint.pointInDisc);
            // raycast to find point on surface
            if (Physics.Raycast(ptRay, out RaycastHit ptHit))
            {
                // calculate rotation and assign to pose together with position
                Quaternion rot;
                if (areDecals)
                {
                    rot = Quaternion.identity * Quaternion.Euler(0f, -180f, 0f);
                }
                else
                {
                    Quaternion randRot = Quaternion.Euler(0f, 0f, rndDataPoint.randAngDeg);
                    rot = Quaternion.LookRotation(ptHit.normal) * (randRot * Quaternion.Euler(90f, 0f, 0f));
                }
                SpawnPoint spawnPoint = new SpawnPoint(ptHit.point, rot, rndDataPoint);
                hitSpawnPoints.Add(spawnPoint);
            }
        }

        return hitSpawnPoints;
    }


    private Ray GetCircleRay(Matrix4x4 tangentToWorld, Vector2 pointInCircle)
    {
        Vector3 normal = tangentToWorld.MultiplyVector(Vector3.forward);
        Vector3 rayOrigin = tangentToWorld.MultiplyPoint3x4(pointInCircle * radius);
        rayOrigin += normal * 2;
        Vector3 rayDirection = -normal;
        return new Ray(rayOrigin, rayDirection);
    }


    private void GenerateRandomPoints()
    {
        spawnDataPoints = new SpawnData[spawnCount];

        for (int i = 0; i < spawnCount; i++)
        {
            spawnDataPoints[i].SetRandomValues(prefabs, hasRandomRotation, areDecals);
        }
    }

    private void DrawSpawnPreviews(List<SpawnPoint> spawnPoints, Camera cam)
    {
        foreach (SpawnPoint spawnPoint in spawnPoints)
        {
            if (spawnPoint.spawnData.prefab != null)
            {
                Matrix4x4 poseToWorld = Matrix4x4.TRS(spawnPoint.position, spawnPoint.rotation, Vector3.one);
                DrawPrefab(spawnPoint.spawnData.prefab, poseToWorld, cam);
            }
            else
            {
                DrawSphere(spawnPoint.position);
            }
        }
    }


    private void DrawPrefab(GameObject prefab, Matrix4x4 poseToWorld, Camera cam)
    {
        MeshFilter[] filters = prefab.GetComponentsInChildren<MeshFilter>();
        foreach (MeshFilter filter in filters)
        {
            Matrix4x4 childToPose = filter.transform.localToWorldMatrix;
            Matrix4x4 childToWorldMtx = poseToWorld * childToPose;
            Mesh mesh = filter.sharedMesh;
            Material mat = filter.GetComponent<MeshRenderer>().sharedMaterial;
            mat.SetPass(0);
            Graphics.DrawMesh(mesh, childToWorldMtx, mat, 0, cam);
        }
    }


    private void DrawSphere(Vector3 pos)
    {
        Handles.SphereHandleCap(-1, pos, Quaternion.identity, 0.1f, EventType.Repaint);
    }


    private void DrawCircleRegion(Matrix4x4 localToWorld)
    {
        // draw circle adapted to terrain
        const int circleDetail = 128;
        Vector3[] ringPoints = new Vector3[circleDetail];
        for (int i = 0; i < circleDetail; i++)
        {
            float t = i / ((float)circleDetail - 1); // go back to 0/1 position
            const float TAU = 6.28318530718f;
            float angRad = t * TAU;
            Vector2 dir = new Vector2(Mathf.Cos(angRad), Mathf.Sin(angRad));
            Ray r = GetCircleRay(localToWorld, dir);
            if (Physics.Raycast(r, out RaycastHit cHit))
            {
                ringPoints[i] = cHit.point + cHit.normal * 0.02f;
            }
            else
            {
                ringPoints[i] = r.origin;
            }
        }
        Handles.DrawAAPolyLine(ringPoints);
    }
}

