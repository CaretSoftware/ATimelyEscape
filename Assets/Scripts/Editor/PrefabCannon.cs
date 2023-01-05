using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
// used Freya Holmér tutorials on tools dev. Made adjustments to fit our game such as decal rotation
// https://www.youtube.com/watch?v=pZ45O2hg_30&t=3s&ab_channel=FreyaHolm%C3%A9r

public struct SpawnData
{
    public Vector2 pointInDisc;
    public float randAngDeg;
    public GameObject prefab;

    public void SetRandomValues(List<GameObject> prefabs, bool hasRandomRotation, bool areDecals)
    {
        pointInDisc = Random.insideUnitCircle;

        if (areDecals)
        {
            randAngDeg = 90f;
        }
        else
        {
            randAngDeg = hasRandomRotation ? Random.value * 360 : 0;
        }

        prefab = prefabs.Count == 0 ? null : prefabs[Random.Range(0, prefabs.Count)];
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
    public bool hasRandomRotation;
    public bool decalMode;
    public List<GameObject> prefabs;

    private SpawnData[] _spawnDataPoints;
    private List<GameObject[]> _objectsToRemove;
    private GameObject _prefabCannonFolder;
    private const string EditorPrefsPre = "PREFAB_CANNON_";

    private SerializedObject so;
    private SerializedProperty propRadius;
    private SerializedProperty propSpawnCount;
    private SerializedProperty propSpawnPrefabs;
    private SerializedProperty propHasRandomRotation;
    private SerializedProperty propAreDecals;


    private void OnEnable()
    {
        SceneView.duringSceneGui += DuringSceneGUI;
        so = new SerializedObject(this);
        LoadProperties();
        LoadPreviousSettings();
        GenerateRandomPoints();
        _prefabCannonFolder = TryFindObjectByName("Prefab Cannon Objects");
    }

    private void OnDisable()
    {
        SaveProperties();
        SceneView.duringSceneGui -= DuringSceneGUI;
    }

    private void LoadProperties()
    {
        propRadius = so.FindProperty("radius");
        propSpawnCount = so.FindProperty("spawnCount");
        propHasRandomRotation = so.FindProperty("hasRandomRotation");
        propAreDecals = so.FindProperty("decalMode");
        propSpawnPrefabs = so.FindProperty("prefabs");
    }

    private void LoadPreviousSettings()
    {
        radius = EditorPrefs.GetFloat(EditorPrefsPre + "radius", 1f);
        spawnCount = EditorPrefs.GetInt(EditorPrefsPre + "spawnCount", 5);
        hasRandomRotation = EditorPrefs.GetBool(EditorPrefsPre + "hasRandomRotation", true);
        decalMode = EditorPrefs.GetBool(EditorPrefsPre + "decalMode", false);

        LoadPrefabs();
    }

    private void SaveProperties()
    {
        EditorPrefs.SetFloat(EditorPrefsPre + "radius", radius);
        EditorPrefs.SetInt(EditorPrefsPre + "spawnCount", spawnCount);
        EditorPrefs.SetBool(EditorPrefsPre + "hasRandomRotation", hasRandomRotation);
        EditorPrefs.SetBool(EditorPrefsPre + "decalMode", decalMode);

        SaveGameObjectArray(EditorPrefsPre + "prefabs", prefabs);
    }

    private void LoadPrefabs()
    {
        if (prefabs == null)
        {
            prefabs = new List<GameObject>();
        }
        string[] paths = EditorPrefs.GetString(EditorPrefsPre + "prefabs").Split(',');
        for (int i = 0; i < paths.Length - 1; i++)
        {
            GameObject go = LoadByPath(paths[i]);
            if (go != null && !prefabs.Contains(go))
            {
                prefabs.Add(go);
            }
        }
    }

    private void SaveGameObjectArray(string key, List<GameObject> arr)
    {
        string arrayValue = "";
        for (var i = 0; i < arr.Count; ++i)
        {
            if (arr[i] != null)
            {
                arrayValue += AssetDatabase.GetAssetPath(arr[i]) + ",";
            }
        }
        EditorPrefs.SetString(key, arrayValue);
    }

    private GameObject LoadByPath(string name)
    {
        if (name != "")
        {
            return (GameObject)AssetDatabase.LoadAssetAtPath(name, typeof(GameObject));
        }
        return null;
    }

    private GameObject TryFindObjectByName(string name)
    {
        return GameObject.Find(name);
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
            GenerateRandomPoints();
            so.ApplyModifiedProperties();
            Event.current.Use();
        }

        // if the cursor is pointing on valid ground
        Transform camTransform = sceneView.camera.transform;
        if (TryRaycastFromCamera(camTransform.up, out Matrix4x4 tangentToWorld))
        {
            List<SpawnPoint> spawnPoints = GetSpawnPoints(camTransform.rotation, tangentToWorld);

            if (Event.current.type == EventType.Repaint)
            {
                DrawCircleRegion(tangentToWorld);
                DrawSpawnPreviews(spawnPoints);
            }

            // spawn on press
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.W)
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
        propRadius.floatValue = Mathf.Max(0.1f, propRadius.floatValue);
        GUILayout.Space(5);

        EditorGUILayout.PropertyField(propSpawnCount);
        propSpawnCount.intValue = Mathf.Max(1, propSpawnCount.intValue);
        GUILayout.Space(5);

        EditorGUILayout.PropertyField(propHasRandomRotation);
        GUILayout.Space(5);

        EditorGUILayout.PropertyField(propAreDecals);
        GUILayout.Space(10);

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(propSpawnPrefabs);
        GUILayout.Space(10);
        if (EditorGUI.EndChangeCheck())
        {
            GenerateRandomPoints();
        }

        GUILayout.BeginHorizontal();
        GUILayout.Space(70);
        if (GUILayout.Button("Randomize points", GUILayout.Height(25)))
        {
            GenerateRandomPoints();
        }
        GUILayout.Space(70);
        GUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.Space(70);
        if (GUILayout.Button("Remove last created", GUILayout.Height(25)))
        {
            RemoveLastCreated();
        }
        GUILayout.Space(70);
        GUILayout.EndHorizontal();
        GUILayout.Space(20);

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
        if (prefabs == null || prefabs.Count == 0)
        {
            return;
        }

        if (_objectsToRemove == null)
        {
            _objectsToRemove = new List<GameObject[]>();
        }

        if (_prefabCannonFolder == null)
        {
            _prefabCannonFolder = new GameObject("Prefab Cannon Objects");
        }

        GameObject[] newObjects = new GameObject[spawnPoints.Count];
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            if (spawnPoints[i].spawnData.prefab != null)
            {
                GameObject spawnedPrefab = (GameObject)PrefabUtility.InstantiatePrefab(spawnPoints[i].spawnData.prefab);
                newObjects[i] = spawnedPrefab;
                spawnedPrefab.transform.position = spawnPoints[i].position;
                spawnedPrefab.transform.rotation = spawnPoints[i].rotation;
                spawnedPrefab.transform.parent = _prefabCannonFolder.transform;
            }
        }
        _objectsToRemove.Add(newObjects);
        GenerateRandomPoints();
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

    private List<SpawnPoint> GetSpawnPoints(Quaternion camRot, Matrix4x4 tangentToWorld)
    {
        List<SpawnPoint> hitSpawnPoints = new List<SpawnPoint>();
        foreach (SpawnData rndDataPoint in _spawnDataPoints)
        {
            // create ray for this point
            Ray ptRay = GetCircleRay(tangentToWorld, rndDataPoint.pointInDisc);
            // raycast to find point on surface
            if (Physics.Raycast(ptRay, out RaycastHit ptHit))
            {
                // calculate rotation and assign to pose together with position
                Quaternion rot;
                if (decalMode)
                {
                    float randAng = hasRandomRotation ? Random.value * 360f : camRot.z;
                    rot = new Quaternion(camRot.x, camRot.y, camRot.z, camRot.w) * Quaternion.Euler(0f, 0f, randAng);
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
        _spawnDataPoints = new SpawnData[spawnCount];

        for (int i = 0; i < spawnCount; i++)
        {
            if (prefabs != null)
            {
                _spawnDataPoints[i].SetRandomValues(prefabs, hasRandomRotation, decalMode);
            }
        }
    }

    private void DrawSpawnPreviews(List<SpawnPoint> spawnPoints)
    {
        foreach (SpawnPoint spawnPoint in spawnPoints)
        {
            DrawSphere(spawnPoint.position);
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
            float t = i / ((float)circleDetail - 1);
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

    
    // Added simple functionality for removing created objects since this version
    // of unity crashes when using "Undo". Removed call to Undo.RecordObject at
    // object creation.
    private void RemoveLastCreated()
    {
        if (_objectsToRemove != null && _objectsToRemove.Count > 0)
        {
            GameObject[] toRemove = _objectsToRemove[_objectsToRemove.Count - 1];
            for (int i = 0; i < toRemove.Length; i++)
            {
                DestroyImmediate(toRemove[i]);
            }
            _objectsToRemove.RemoveAt(_objectsToRemove.Count - 1);
        }
    }

}
