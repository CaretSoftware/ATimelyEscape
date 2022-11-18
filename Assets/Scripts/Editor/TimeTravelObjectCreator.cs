using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TimeTravelObjectCreator : EditorWindow
{
    //public GameObject[] timeTravelObjects;'
    public GameObject prefab;
    public GameObject pastPrefab;
    public GameObject futurePrefab;
    public string objectName;

    private bool showSettings;
    public bool createPrefab;
    public bool changesPrefab;
    public bool changesMaterials;
    public bool canBeMovedByPlayer;
    public bool canCollideOnTimeTravel;

    SerializedObject so;

    //SerializedProperty propTimeTravelObjects;
    SerializedProperty propPrefab;
    SerializedProperty propPastPrefab;
    SerializedProperty propFuturePrefab;
    SerializedProperty propObjectName;
    SerializedProperty propMakePrefab;
    SerializedProperty propChangesPrefab;
    SerializedProperty propChangesMaterials;
    SerializedProperty propCanBeMovedByPlayer;
    SerializedProperty propCanCollideOnTimeTravel;




    [MenuItem("Tools/TTOCreator")]
    public static void OpneTTOCreator()
    {
        GetWindow<TimeTravelObjectCreator>();
    }

    private void OnEnable()
    {
        SetPropsToSerialized();
        SceneView.duringSceneGui += DuringSceneGUI;
    }

    private void OnDisable()
    {
        //timeTravelObjects = null;
        SceneView.duringSceneGui -= DuringSceneGUI;
    }

    private void SetPropsToSerialized()
    {
        so = new SerializedObject(this);

        //propTimeTravelObjects = so.FindProperty("timeTravelObjects");
        propPrefab = so.FindProperty("prefab");
        propPastPrefab = so.FindProperty("pastPrefab");
        propFuturePrefab = so.FindProperty("futurePrefab");
        propObjectName = so.FindProperty("objectName");
        propMakePrefab = so.FindProperty("createPrefab");
        propChangesPrefab = so.FindProperty("changesPrefab");
        propChangesMaterials = so.FindProperty("changesMaterials");
        propCanBeMovedByPlayer = so.FindProperty("canBeMovedByPlayer");
        propCanCollideOnTimeTravel = so.FindProperty("canCollideOnTimeTravel");
    }

    void DuringSceneGUI(SceneView sceneView)
    {

    }


    private void OnGUI()
    {
        so.Update();
        GUI.skin.button.stretchWidth = true;

        EditorGUI.indentLevel++;
        GUILayout.Space(40);
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.Label("Name");
        EditorGUILayout.PropertyField(propObjectName, new GUIContent(""));
        GUILayout.Space(10);
        GUILayout.EndHorizontal();

        GUILayout.Space(30);
        showSettings = EditorGUILayout.Foldout(showSettings, "Settings");
        if (showSettings)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(propMakePrefab);
            EditorGUILayout.PropertyField(propChangesPrefab);
            if (!changesPrefab)
            {
                EditorGUILayout.PropertyField(propChangesMaterials);
            }
            EditorGUILayout.PropertyField(propCanBeMovedByPlayer);
            EditorGUILayout.PropertyField(propCanCollideOnTimeTravel);
            EditorGUI.indentLevel--;
            GUILayout.Space(40);
        }

        //EditorGUILayout.PropertyField(propTimeTravelObjects, new GUIContent("Time Travel Objects"));
        if (changesPrefab)
        {
            EditorGUILayout.PropertyField(propPastPrefab, new GUIContent("Past"));
            EditorGUILayout.PropertyField(propPrefab, new GUIContent("Present"));
            EditorGUILayout.PropertyField(propFuturePrefab, new GUIContent("Future"));
        }
        else
        {
            EditorGUILayout.PropertyField(propPrefab, new GUIContent("Prefab"));
        }


      
        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.Space(40);
        if (GUILayout.Button("Create", GUILayout.Height(25)))
        {
            Create();
        }
        GUILayout.Space(40);
        GUILayout.EndHorizontal();


        if (so.ApplyModifiedProperties())
        {
            SceneView.RepaintAll();
        };

        return;
    }

    private void Create()
    {
        GameObject ttoManager = CreateTTOManager();
        //if (timeTravelObjects != null && timeTravelObjects.Length > 0)
        //{
            CreateTTObjects(ttoManager);
        //}

        if (createPrefab)
        {
            PrefabUtility.SaveAsPrefabAsset(ttoManager, "Assets/Prefabs/TimeTravelObjects/" + ttoManager.name + ".prefab");
        }
    }

    private GameObject CreateTTOManager()
    {
        string name = objectName == null || objectName == "" ? "TTOManager" : "TTO_" + objectName;
        GameObject ttoManager = new GameObject(name);
        ttoManager.AddComponent<TimeTravelObjectManager>();
        Undo.RegisterCreatedObjectUndo(ttoManager, "Create object");
        return ttoManager;
    }

    private void CreateTTObjects(GameObject ttoManager)
    {
        int iterations = changesPrefab ? 3 : 1;
        for (var i = 0; i < iterations; ++i)
        {
            string name = GetTimePeriodByIndex(i) + "_" + objectName;
            GameObject tto = new GameObject(name);
            tto.AddComponent<TimeTravelObject>();
            tto.transform.parent = ttoManager.transform;
            Undo.RegisterCreatedObjectUndo(tto, "Create object");
        }
    }

    private string GetTimePeriodByIndex(int index)
    {
        switch (index)
        {
            case 0:
                return "Past";
            case 1:
                return "Present";
            case 2:
                return "Future";
        }
        return null;
    }
}

