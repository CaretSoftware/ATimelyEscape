using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TimeTravelObjectCreator : EditorWindow
{
    public GameObject pastPrefab;
    public GameObject presentPrefab;
    public GameObject futurePrefab;
    public string objectName;
    //public Material[] pastMaterials;
    //public Material[] presentMaterials;
    //public Material[] futureMaterials;
    //public GameObject[] timeTravelObjects;

    private bool showSettings;
    public bool createPrefab;
    public bool canBeMovedByPlayer;
    public bool canCollideOnTimeTravel;
    //public bool changesPrefab;
    //public bool changesMaterials;

    SerializedObject so;

    SerializedProperty propPastPrefab;
    SerializedProperty propPresentPrefab;
    SerializedProperty propFuturePrefab;
    SerializedProperty propObjectName;
    SerializedProperty propMakePrefab;
    SerializedProperty propCanBeMovedByPlayer;
    SerializedProperty propCanCollideOnTimeTravel;
    //SerializedProperty propPastMaterials;
    //SerializedProperty propPresentMaterials;
    //SerializedProperty propFutureMaterials;
    //SerializedProperty propChangesPrefab;
    //SerializedProperty propChangesMaterials;
    //SerializedProperty propTimeTravelObjects;


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

        propPastPrefab = so.FindProperty("pastPrefab");
        propPresentPrefab = so.FindProperty("presentPrefab");
        propFuturePrefab = so.FindProperty("futurePrefab");
        propObjectName = so.FindProperty("objectName");
        propMakePrefab = so.FindProperty("createPrefab");
        propCanBeMovedByPlayer = so.FindProperty("canBeMovedByPlayer");
        propCanCollideOnTimeTravel = so.FindProperty("canCollideOnTimeTravel");
        //propTimeTravelObjects = so.FindProperty("timeTravelObjects");
        //propChangesPrefab = so.FindProperty("changesPrefab");
        //propChangesMaterials = so.FindProperty("changesMaterials");
        //propPastMaterials = so.FindProperty("pastMaterials");
        //propPresentMaterials = so.FindProperty("presentMaterials");
        //propFutureMaterials = so.FindProperty("futureMaterials");
    }

    private void LoadPreviousSettings()
    {
        objectName = EditorPrefs.GetString("TTO_CREATOR_objectName", "");
        createPrefab = EditorPrefs.GetBool("TTO_CREATOR_createPrefab", false);
        canBeMovedByPlayer = EditorPrefs.GetBool("TTO_CREATOR_canBeMovedByPlayer", false);
        canCollideOnTimeTravel = EditorPrefs.GetBool("TTO_CREATOR_canCollideOnTimeTravel", true);
        showSettings = EditorPrefs.GetBool("TTO_CREATOR_showSettings", false);
        //changesPrefab = EditorPrefs.GetBool("TTO_CREATOR_changesPrefab", false);

        pastPrefab = LoadByPath("TTO_pastPrefab");
        presentPrefab = LoadByPath("TTO_presentPrefab");
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
        EditorPrefs.SetBool("TTO_CREATOR_canBeMovedByPlayer", canBeMovedByPlayer);
        EditorPrefs.SetBool("TTO_CREATOR_canCollideOnTimeTravel", canCollideOnTimeTravel);
        EditorPrefs.SetBool("TTO_CREATOR_showSettings", showSettings);
        EditorPrefs.SetString("TTO_pastPrefab", AssetDatabase.GetAssetPath(pastPrefab));
        EditorPrefs.SetString("TTO_presentPrefab", AssetDatabase.GetAssetPath(presentPrefab));
        EditorPrefs.SetString("TTO_futurePrefab", AssetDatabase.GetAssetPath(futurePrefab));
        //EditorPrefs.SetBool("TTO_CREATOR_changesPrefab", changesPrefab);


        /*if (pastMaterials != null)
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
        }*/
    }

    /*private void SaveMaterialArray(string key, Material[] materials)
    {
        string materialsValue = "";
        for (var i = 0; i < materials.Length; ++i)
        {
            materialsValue += AssetDatabase.GetAssetPath(materials[i]) + ",";
        }
        EditorPrefs.SetString(key, materialsValue);
    }*/

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
            //EditorGUILayout.PropertyField(propChangesPrefab, new GUIContent("Changes prefab"));

            EditorGUILayout.PropertyField(propCanBeMovedByPlayer, new GUIContent("Can be moved by player"));
            EditorGUILayout.PropertyField(propCanCollideOnTimeTravel, new GUIContent("Can collide on time travel"));
            EditorGUI.indentLevel--;
            GUILayout.Space(40);
            EditorGUIUtility.labelWidth = 0;
        }


        EditorGUILayout.PropertyField(propPastPrefab, new GUIContent("Past"));
        EditorGUILayout.PropertyField(propPresentPrefab, new GUIContent("Present"));
        EditorGUILayout.PropertyField(propFuturePrefab, new GUIContent("Future"));


        GUILayout.Space(20);

        /*if (changesMaterials)
        {
            EditorGUILayout.PropertyField(propPastMaterials);
            EditorGUILayout.PropertyField(propPresentMaterials);
            EditorGUILayout.PropertyField(propFutureMaterials);
        }*/

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
            TimezoneChangeEditor.ActivateObjectsByTimezone();
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
        managerComponent.ChangesMaterials = false;
        managerComponent.ChangesPrefab = true;
        // add material arrays with setters
    }

    private void CreateTTObjects(GameObject ttoManager)
    {
            string id = System.Guid.NewGuid().ToString();

            GameObject pastObj = null;
            GameObject presentObj = null;
            GameObject futureObj = null;

            if (pastPrefab != null)
            {
                pastObj = InstantiateTTOPrefab(ttoManager, pastPrefab, TimeTravelPeriod.Past, id);
                ttoManager.GetComponent<TimeTravelObjectManager>().Past = pastObj.GetComponent<TimeTravelObject>();
            }

            if (presentPrefab != null)
            {
                presentObj = InstantiateTTOPrefab(ttoManager, presentPrefab, TimeTravelPeriod.Present, id);
                ttoManager.GetComponent<TimeTravelObjectManager>().Present = presentObj.GetComponent<TimeTravelObject>();
            }

            if (futurePrefab != null)
            {
                futureObj = InstantiateTTOPrefab(ttoManager, futurePrefab, TimeTravelPeriod.Future, id);
                ttoManager.GetComponent<TimeTravelObjectManager>().Future = futureObj.GetComponent<TimeTravelObject>();
            }
       
            ApplyUniqueNamesToChildren(pastObj, presentObj, futureObj);
    }

    private GameObject InstantiateTTOPrefab(GameObject ttoManager, GameObject go, TimeTravelPeriod timezone, string id)
    {
        if (go != null)
        {
            GameObject tto = (GameObject)PrefabUtility.InstantiatePrefab(go);
            Undo.RegisterCreatedObjectUndo(tto, "Create object");
            tto.transform.parent = ttoManager.transform;
            TimeTravelObject ttoComponent = tto.AddComponent<TimeTravelObject>();
            ttoComponent.timeTravelPeriod = timezone;

            if (id != null && tto.GetComponent<MeshRenderer>() != null)
            {
                ApplyNameToObject(tto, timezone.ToString(), id);
            }

            /*if (TimezoneChangeEditor.editorIsEnabled)
            {
                TimezoneChangeEditor.AddTTOToDictionary(tto, timezone);
            }*/

            return tto;
        }
        return null;
    }

    private void ApplyUniqueNamesToChildren(GameObject past, GameObject present, GameObject future)
    {
        int count = SetHighestChildCount(past, present, future);

        for (int i = 0; i <= count; i++)
        {
            string id = System.Guid.NewGuid().ToString();
            bool hasPast = past != null && past.transform.childCount > i;
            bool hasPresent = present != null && present.transform.childCount > i;
            bool hasFuture = future != null && future.transform.childCount > i;

            if (hasPast && past.transform.GetChild(i).GetComponent<MeshRenderer>())
            {
                ApplyNameToObject(past.transform.GetChild(i).gameObject, "Past", id);
            }

            if (hasPresent && present.transform.GetChild(i).GetComponent<MeshRenderer>())
            {
                ApplyNameToObject(present.transform.GetChild(i).gameObject, "Present", id);
            }

            if (hasFuture && future.transform.GetChild(i).GetComponent<MeshRenderer>())
            {
                ApplyNameToObject(future.transform.GetChild(i).gameObject, "Future", id);
            }

            if (hasPast && past.transform.GetChild(i).childCount > 0 ||
                hasPresent && present.transform.GetChild(i).childCount > 0 ||
                hasFuture && future.transform.GetChild(i).childCount > 0)
            {
                GameObject nextPastObj = hasPast ? past.transform.GetChild(i).gameObject : null;
                GameObject nextPresentObj = hasPresent ? present.transform.GetChild(i).gameObject : null;
                GameObject nextFutureObj = hasFuture ? future.transform.GetChild(i).gameObject : null;

                ApplyUniqueNamesToChildren(nextPastObj, nextPresentObj, nextFutureObj);
            }
        }
    }

    private int SetHighestChildCount(GameObject past, GameObject present, GameObject future)
    {
        int pastCount = past != null ? (past.transform.childCount) : -1;
        int presentCount = present != null ? (present.transform.childCount) : -1;
        int futureCount = future != null ? (future.transform.childCount) : -1;
        return Mathf.Max(pastCount, Mathf.Max(presentCount, futureCount));
    }

    private void ApplyNameToObject(GameObject go, string timePeriod, string id)
    {
        string baseName = objectName == "" || objectName == null ? go.name : objectName;
        if (id != null)
        {
            go.name = "TTO[" + baseName + "]_[" + id + "]_[" + timePeriod + "]";
            //go.name = "TTO[" + guid_01 + "]_[" + baseName + "]_[" + guid_02 + "]_[" + timePeriod + "]";
        }
    }
}

