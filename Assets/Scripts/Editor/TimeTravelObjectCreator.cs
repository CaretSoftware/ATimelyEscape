using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TimeTravelObjectCreator : EditorWindow
{
    //public GameObject[] timeTravelObjects;
    public GameObject defaultPrefab;
    public GameObject pastPrefab;
    public GameObject futurePrefab;
    public Material[] pastMaterials;
    public Material[] presentMaterials;
    public Material[] futureMaterials;
    public string objectName;

    private bool showSettings;
    public bool createPrefab;
    public bool changesPrefab;
    public bool changesMaterials;
    public bool canBeMovedByPlayer;
    public bool canCollideOnTimeTravel;

    private string guid_01;

    SerializedObject so;

    //SerializedProperty propTimeTravelObjects;
    SerializedProperty propPrefab;
    SerializedProperty propPastPrefab;
    SerializedProperty propFuturePrefab;
    SerializedProperty propPastMaterials;
    SerializedProperty propPresentMaterials;
    SerializedProperty propFutureMaterials;

    SerializedProperty propObjectName;
    SerializedProperty propMakePrefab;
    SerializedProperty propChangesPrefab;
    SerializedProperty propChangesMaterials;
    SerializedProperty propCanBeMovedByPlayer;
    SerializedProperty propCanCollideOnTimeTravel;


    [MenuItem("Tools/TTOCreator")]
    public static void OpenTTOCreator()
    {
        GetWindow<TimeTravelObjectCreator>();
    }

    private void OnEnable()
    {
        SetPropsToSerialized();
        LoadPreviousSettings();
        SceneView.duringSceneGui += DuringSceneGUI;
    }

    private void OnDisable()
    {
        SaveSettings();
        SceneView.duringSceneGui -= DuringSceneGUI;
    }

    private void SetPropsToSerialized()
    {
        so = new SerializedObject(this);

        //propTimeTravelObjects = so.FindProperty("timeTravelObjects");
        propPrefab = so.FindProperty("defaultPrefab");
        propPastPrefab = so.FindProperty("pastPrefab");
        propFuturePrefab = so.FindProperty("futurePrefab");
        propObjectName = so.FindProperty("objectName");
        propMakePrefab = so.FindProperty("createPrefab");
        propChangesPrefab = so.FindProperty("changesPrefab");
        propChangesMaterials = so.FindProperty("changesMaterials");
        propCanBeMovedByPlayer = so.FindProperty("canBeMovedByPlayer");
        propCanCollideOnTimeTravel = so.FindProperty("canCollideOnTimeTravel");
        propPastMaterials = so.FindProperty("pastMaterials");
        propPresentMaterials = so.FindProperty("presentMaterials");
        propFutureMaterials = so.FindProperty("futureMaterials");
    }

    private void LoadPreviousSettings()
    {
        objectName = EditorPrefs.GetString("TTO_CREATOR_objectName", "");
        createPrefab = EditorPrefs.GetBool("TTO_CREATOR_createPrefab", false);
        changesPrefab = EditorPrefs.GetBool("TTO_CREATOR_changesPrefab", false);
        canBeMovedByPlayer = EditorPrefs.GetBool("TTO_CREATOR_canBeMovedByPlayer", false);
        canCollideOnTimeTravel = EditorPrefs.GetBool("TTO_CREATOR_canCollideOnTimeTravel", false);
        showSettings = EditorPrefs.GetBool("TTO_CREATOR_showSettings", false);

        pastPrefab = LoadByPath("TTO_pastPrefab");
        defaultPrefab = LoadByPath("TTO_defaultPrefab");
        futurePrefab = LoadByPath("TTO_futurePrefab");
    }

    /*private void LoadMaterialArray(string key, Material[] materials)
    {
        string materialsStr = EditorPrefs.GetString(key, "");
        materials = materialsStr.Split(',');
        for (var i = 0; i < materials.Length; ++i)
        {
            materialsValue += AssetDatabase.GetAssetPath(materials[i]) + ",";
        }
        EditorPrefs.SetString(key, materialsValue);
    }*/

    private void SaveSettings()
    {
        EditorPrefs.SetString("TTO_CREATOR_objectName", objectName);
        EditorPrefs.SetBool("TTO_CREATOR_createPrefab", createPrefab);
        EditorPrefs.SetBool("TTO_CREATOR_changesPrefab", changesPrefab);
        EditorPrefs.SetBool("TTO_CREATOR_canBeMovedByPlayer", canBeMovedByPlayer);
        EditorPrefs.SetBool("TTO_CREATOR_canCollideOnTimeTravel", canCollideOnTimeTravel);
        EditorPrefs.SetBool("TTO_CREATOR_showSettings", showSettings);
        EditorPrefs.SetString("TTO_pastPrefab", AssetDatabase.GetAssetPath(pastPrefab));
        EditorPrefs.SetString("TTO_defaultPrefab", AssetDatabase.GetAssetPath(defaultPrefab));
        EditorPrefs.SetString("TTO_futurePrefab", AssetDatabase.GetAssetPath(futurePrefab));

        if (pastMaterials != null)
        {
            SaveMaterialArray("TTO_pastMaterials", pastMaterials);
        }

        if (presentMaterials != null)
        {
            SaveMaterialArray("TTO_presentMaterials", presentMaterials);
        }

        if (futureMaterials != null)
        {
            SaveMaterialArray("TTO_futureMaterials", futureMaterials);
        }
    }

    private void SaveMaterialArray(string key, Material[] materials)
    {
        string materialsValue = "";
        for (var i = 0; i < materials.Length; ++i)
        {
            materialsValue += AssetDatabase.GetAssetPath(materials[i]) + ",";
        }
        EditorPrefs.SetString(key, materialsValue);
    }

    private GameObject LoadByPath(string editorPrefsKey)
    {
        string prefabName = EditorPrefs.GetString(editorPrefsKey, "");
        if (prefabName != "")
        {
            return (GameObject)AssetDatabase.LoadAssetAtPath(prefabName, typeof(GameObject));
        }
        return null;
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
            EditorGUIUtility.labelWidth = 180f;
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(propMakePrefab, new GUIContent("Make prefab"));
            EditorGUILayout.PropertyField(propChangesPrefab, new GUIContent("Changes prefab"));
            if (!changesPrefab)
            {
                EditorGUILayout.PropertyField(propChangesMaterials, new GUIContent("Changes materials"));
            }
            else
            {
                changesMaterials = false;
            }

            EditorGUILayout.PropertyField(propCanBeMovedByPlayer, new GUIContent("Can be moved by player"));
            EditorGUILayout.PropertyField(propCanCollideOnTimeTravel, new GUIContent("Can collide on time travel"));
            EditorGUI.indentLevel--;
            GUILayout.Space(40);
            EditorGUIUtility.labelWidth = 0;
        }

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

        if (changesMaterials)
        {
            EditorGUILayout.PropertyField(propPastMaterials);
            EditorGUILayout.PropertyField(propPresentMaterials);
            EditorGUILayout.PropertyField(propFutureMaterials);
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

        CreateTTObjects(ttoManager);
        ApplySettingsToManager(ttoManager);

        if (createPrefab)
        {
            PrefabUtility.SaveAsPrefabAsset(ttoManager, "Assets/Prefabs/TimeTravelObjects/" + ttoManager.name + ".prefab");
        }

        if (TimezoneChangeEditor.editorIsEnabled)
        {
            TimezoneChangeEditor.ActivateTimezone();
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

    private void ApplySettingsToManager(GameObject ttoManager)
    {
        TimeTravelObjectManager managerComponent = ttoManager.GetComponent<TimeTravelObjectManager>();
        managerComponent.CanBeMovedByPlayer = canBeMovedByPlayer;
        managerComponent.CanCollideOnTimeTravel = canCollideOnTimeTravel;
        managerComponent.ChangesMaterials = changesMaterials;
        managerComponent.ChangesPrefab = changesPrefab;
        // add material arrays with setters
    }

    private void CreateTTObjects(GameObject ttoManager)
    {
        if (changesPrefab)
        {
            guid_01 = System.Guid.NewGuid().ToString();
            string guid_02 = System.Guid.NewGuid().ToString();
            GameObject pastObj = InstantiateTTOPrefab(ttoManager, pastPrefab, TimeTravelPeriod.Past, guid_01, guid_02);
            GameObject presentObj = InstantiateTTOPrefab(ttoManager, defaultPrefab, TimeTravelPeriod.Present, guid_01, guid_02);
            GameObject futureObj = InstantiateTTOPrefab(ttoManager, futurePrefab, TimeTravelPeriod.Future, guid_01, guid_02);
            ApplyUniqueNamesToChildren(pastObj, presentObj, futureObj);
        }
        else
        {
            InstantiateTTOPrefab(ttoManager, defaultPrefab, TimeTravelPeriod.Present, null, null);
        }
    }

    private GameObject InstantiateTTOPrefab(GameObject ttoManager, GameObject go, TimeTravelPeriod timezone, string guid_01, string guid_02)
    {
        if (go != null)
        {
            GameObject tto = (GameObject)PrefabUtility.InstantiatePrefab(go);
            Undo.RegisterCreatedObjectUndo(tto, "Create object");
            tto.transform.parent = ttoManager.transform;
            TimeTravelObject ttoComponent = tto.AddComponent<TimeTravelObject>();
            ttoComponent.timeTravelPeriod = timezone;

            if (guid_01 != null && guid_02 != null)
            {
                ApplyNameToObject(tto, timezone.ToString(), guid_01, guid_02);
            }

            if (TimezoneChangeEditor.editorIsEnabled)
            {
                TimezoneChangeEditor.AddTTOToDictionary(tto, timezone);
            }

            return tto;
            //ttoComponent.GatherRenderers(tto.transform);
            //ttoComponent.OrderedRenderers;
        }
        return null;
    }

    private void ApplyUniqueNamesToChildren(GameObject past, GameObject present, GameObject future)
    {
        for (int i = 0; i < present.transform.childCount; i++)
        { 
            string guid_02 = System.Guid.NewGuid().ToString();

            if (present.transform.GetChild(i) != null && present.transform.GetChild(i).GetComponent<MeshRenderer>())
            {
                ApplyNameToObject(present.transform.GetChild(i).gameObject, "Present", guid_01, guid_02);
            }

            if (past.transform.GetChild(i) != null && past.transform.GetChild(i).GetComponent<MeshRenderer>())
            {
                ApplyNameToObject(past.transform.GetChild(i).gameObject, "Past", guid_01, guid_02);
            }

            if (future.transform.GetChild(i) != null && future.transform.GetChild(i).GetComponent<MeshRenderer>())
            {
                ApplyNameToObject(future.transform.GetChild(i).gameObject, "Future", guid_01, guid_02);
            }

            if (present.transform.GetChild(i).transform.childCount > 0)
            {
                ApplyUniqueNamesToChildren(past.transform.GetChild(i).gameObject, present.transform.GetChild(i).gameObject, future.transform.GetChild(i).gameObject);
            }
        }
    }


    private void ApplyNameToObject(GameObject go, string timePeriod, string guid_01, string guid_02)
    {
        string baseName = objectName == "" || objectName == null ? go.name : objectName;
        if (guid_01 != null && guid_02 != null)
        {
            go.name = "TTO[" + guid_01 + "]_[" + baseName + "]_[" + guid_02 + "]_[" + timePeriod + "]";
        }
    }
}

