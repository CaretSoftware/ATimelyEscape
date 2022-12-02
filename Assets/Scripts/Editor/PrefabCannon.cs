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
    public GameObject[] spawnPrefabs;
    public Material previewMaterial;

    SerializedObject so;
    SerializedProperty propRadius;
    SerializedProperty propSpawnCount;
    SerializedProperty propSpawnPrefabs;
    SerializedProperty propPreviewMaterial;

    /*
    public struct SpawnData
    {
        public Vector2 pointInDisc;
        public float randAngDeg;
        public GameObject spawnPrefab;

        public void SetRandomValues(GameObject[] spawnPrefabs)
        {
            pointInDisc = Random.insideUnitCircle;
            randAngDeg = Random.value * 360;
            spawnPrefab = spawnPrefabs[Random.Range(0, spawnPrefabs.Length)];
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

        private SpawnData[] randPoints;


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
        propSpawnPrefabs = so.FindProperty("spawnPrefabs");
        propPreviewMaterial = so.FindProperty("previewMaterial");
    }

    void DuringSceneGUI(SceneView sceneView)
    {
        Handles.zTest = CompareFunction.LessEqual;
        Transform camTransform = sceneView.camera.transform;

        // make sure it repaints on mouse move
        if(Event.current.type == EventType.MouseMove)
        {
            sceneView.Repaint();
        }

        
        bool holdingAlt = (Event.current.modifiers & EventModifiers.Alt) != 0;
        if (Event.current.type == EventType.ScrollWheel && holdingAlt)
        {
            float scrollDir = Mathf.Sign(Event.current.delta.y);

            so.Update();
            propRadius.floatValue *= 1f + scrollDir * 0.05f;
            so.ApplyModifiedProperties();
            Repaint();
            Event.current.Use();
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


            Ray GetTangentRay(Vector2 tangentSpacePos)
            {
                Vector3 rayOrigin = hit.point + (hitTangent * tangentSpacePos.x + hitBitangent * tangentSpacePos.y) * radius;
                rayOrigin += hitNormal * 2; // offset margin
                Vector3 rayDirection = -hitNormal;
                return new Ray(rayOrigin, rayDirection);
            }

            List<SpawnPoint> hitPoses = new List<SpawnPoint>(); 

            // drawing points
            foreach(SpawnData rndDataPoint in randPoints)
            {


                // create ray for this point
                Ray pointRay = GetTangentRay(rndDataPoint.pointInDisc);

                // raycast to find point on surface
                if(Physics.Raycast(pointRay, out RaycastHit pointHit))
                {
                    // calculate rotation and assign to pose together with position
                    Quaternion randRot = Quaternion.Euler(0f, 0f, rndDataPoint.randAngDeg);
                    Quaternion rot = Quaternion.LookRotation(pointHit.normal) * (randRot * Quaternion.Euler(90f, 0f, 0f));
                    SpawnPoint spawnPoint = new SpawnPoint(pointHit.point, rot);
                    hitPoses.Add(pose);

                    // draw sphere and normal on surface
                    DrawSphere(pointHit.point);


                    // mesh
                    if (spawnPrefabs != null && spawnPrefabs.Length > 0 && spawnPrefabs[0] != null)
                    {
                        Matrix4x4 poseToWorld = Matrix4x4.TRS(spawnPoint.position, spawnPoint.rotation, Vector3.one);
                        MeshFilter[] filters = spawnPrefabs[0].GetComponentsInChildren<MeshFilter>();
                        foreach (MeshFilter filter in filters)
                        {
                            Matrix4x4 childToPose = filter.transform.localToWorldMatrix;
                            Matrix4x4 childToWorldMtx = poseToWorld * childToPose;
                            Mesh mesh = filter.sharedMesh;
                            Material mat = spawnPrefabs[0].GetComponent<MeshRenderer>().sharedMaterial;
                            mat.SetPass(0);
                            Graphics.DrawMeshNow(mesh, childToWorldMtx);
                        }
                    }

                    //Mesh mesh = spawnPrefabs[0].GetComponent<MeshFilter>().sharedMesh;
                    //Material mat = spawnPrefabs[0].GetComponent<MeshRenderer>().sharedMaterial;
                    // previewMaterial.SetPass(0);
                    //mat.SetPass(0);
                    //Graphics.DrawMeshNow(mesh, pose.position, pose.rotation);
                }
            }
            Handles.DrawAAPolyLine(5, hit.point, hit.point + hit.normal);






            // spawn on press
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.D)
            {
                TrySpawnObjects(hitPoses);
            }


            // draw circle adapted to terrain
            const int circleDetail = 128;
            Vector3[] ringPoints = new Vector3[circleDetail];
            for(int i = 0; i < circleDetail; i++)
            {
                float t = i / ((float) circleDetail - 1); // go back to 0/1 position
                const float TAU = 6.28318530718f;
                float angRad = t * TAU;
                Vector2 dir = new Vector2(Mathf.Cos(angRad), Mathf.Sin(angRad));
                Ray r = GetTangentRay(dir);
                if(Physics.Raycast(r, out RaycastHit cHit))
                {
                    ringPoints[i] = cHit.point + cHit.normal * 0.02f;
                }
                else
                {
                    ringPoints[i] = r.origin;
                }
            }
            Handles.DrawAAPolyLine(ringPoints);


            //Handles.DrawWireDisc(hit.point, hit.normal, radius);
        }





    }

    private void OnGUI()
    {
        so.Update();
        EditorGUILayout.PropertyField(propRadius);
        propRadius.floatValue = Mathf.Max(1f, propRadius.floatValue);

        EditorGUILayout.PropertyField(propSpawnCount);
        propSpawnCount.intValue = Mathf.Max(1, propSpawnCount.intValue);

        EditorGUILayout.PropertyField(propSpawnPrefabs);

        EditorGUILayout.PropertyField(propPreviewMaterial);


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

    private void TrySpawnObjects(List<SpawnPoint> spawnPoints)
    {
        if(spawnPrefabs == null)
        {
            return;
        }

        foreach(SpawnPoint spawnPoint in spawnPoints)
        {
            // spawn prefab
            //int ranIndex = Random.Range(0, spawnPrefabs.Length);

            if(spawnPoint.spawnData.spawnPrefab != null)
            {
            GameObject spawnedThing = (GameObject)PrefabUtility.InstantiatePrefab(spawnPoint.spawnData.spawnPrefab);
            Undo.RegisterCreatedObjectUndo(spawnedThing, "Spawn Objects");
            spawnedThing.transform.position = spawnPoint.position;
            spawnedThing.transform.rotation = spawnPoint.rotation;
            }
        }

        GenerateRandomPoints(); // update points
    }

    private List<SpawnPoint> GetSpawnPoints(Matrix4x4 tangentToWorld)
    {
        List<SpawnPoint> hitSpawnPoints = new List<SpawnPoint>();
        foreach(SpawnData rndDataPoint in spawnDataPoints)
        {
            // create ray for this point
            Ray ptRay = GetCircleray(tangentToWorld, rndDataPoint.pointInDisc);
            // raycast to find point on surface
            if( Physics.Raycast(ptRay, out RaycastHit ptHit))
            {
                // calculate rotation and assign to pose together with position
                Quaternion randRot = Quaternion.Euler(0f, 0f, rndDataPoint.randAngDeg);
                Quaternion rot = Quaternion.LookRotation(ptHit.normal) * (randRot * Quaternion.Euler(90f, 0f, 0f));
                SpawnPoint spawnPoint = new SpawnPoint(ptHit.point, rot, rndDataPoint);
                hitSpawnPoints.Add(spawnPoint);
            }
        }
    }


    private void DrawSpawnPreviews(List<SpawnPoint> spawnPoints, Camera cam)
    {
        foreach (SpawnPoint spawnPoint in spawnPoints)
        {
            if(spawnPrefabs != null && spawnPrefabs.Length > 0)
            {
                Matrix4x4 poseToWorld = Matrix4x4.TRS(spawnPoint.position, spawnPoint.rotation, Vector3.one);
                DrawPrefab(spawnPoint.spawnData.spawnPrefab, poseToWorld, cam);
            }
            else
            {
                DrawSphere(spawnPoint.position);
            }
        }
    }

    private static void DrawPrefab(GameObject prefab, Matrix4x4 poseToWorld, Camera cam)
    {

    }

    private void GenerateRandomPoints()
    {
        randPoints = new SpawnData[spawnCount];

        for(var i = 0; i < spawnCount; i++)
        {
            randPoints[i].SetRandomValues(spawnPrefabs);
        }
    }

    private void DrawSphere(Vector3 pos)
    {
        Handles.SphereHandleCap(-1, pos, Quaternion.identity, 0.1f, EventType.Repaint);
    }
    */
}
