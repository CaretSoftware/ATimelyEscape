using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class TimezoneChangeEditor : EditorWindow
{
    /*public enum Timezone
    {
        past,
        present,
        future
    }*/

    public static TimeTravelPeriod currentTimezone;
    private static Dictionary<GameObject, TimeTravelPeriod> ttosInScene;

    [MenuItem("Tools/EnableTimezoneOption")]
    public static void Enable()
    {
        LoadCurrentTimezone();
        InitializeTTOManagers();
        InitializeObjectDictionary();
        SceneView.duringSceneGui += DuringSceneGUI;
    }

    [MenuItem("Tools/DisableTimezoneOption")]
    public static void Disable()
    {
        SaveCurrentTimezone();
        ttosInScene.Clear();
        SceneView.duringSceneGui -= DuringSceneGUI;
    }

    private static void LoadCurrentTimezone()
    {
        int currentTimezone = EditorPrefs.GetInt("TimezoneChangeEditor_currentTimezone", 1);

        switch (currentTimezone)
        {
            case 0:
                TimezoneChangeEditor.currentTimezone = TimeTravelPeriod.Past;
                break;
            case 2:
                TimezoneChangeEditor.currentTimezone = TimeTravelPeriod.Future;
                break;
            default:
                TimezoneChangeEditor.currentTimezone = TimeTravelPeriod.Present;
                break;
        }
    }

    private static void SaveCurrentTimezone()
    {
        int currentTimezone;

        switch (TimezoneChangeEditor.currentTimezone)
        {
            case TimeTravelPeriod.Past:
                currentTimezone = 0;
                break;
            case TimeTravelPeriod.Future:
                currentTimezone = 2;
                break;
            default:
                currentTimezone = 1;
                break;
        }

        EditorPrefs.SetInt("TimezoneChangeEditor_currentTimezone", currentTimezone);
    }

    private static void DuringSceneGUI(SceneView sceneview)
    {
        Handles.BeginGUI();

        EditorGUI.BeginChangeCheck();
        currentTimezone = (TimeTravelPeriod)EditorGUI.EnumPopup(new Rect(5, 10, 67, 15), currentTimezone);
        if (EditorGUI.EndChangeCheck())
        {
            ActivateTimezone();
            //PrintObjects();
            //ChangeTimezone();
        }

        Handles.EndGUI();
    }

    /*private static void ChangeTimezone()
    {
        switch (currentTimezone)
        {
            case TimeTravelPeriod.Past:
                Debug.Log("Changed to past");
                break;
            case TimeTravelPeriod.Present:
                Debug.Log("Changed to present");
                break;
            case TimeTravelPeriod.Future:
                Debug.Log("Changed to future");
                break;
        }
    }*/

    private static void InitializeTTOManagers()
    {
        TimeTravelObjectManager[] ttoManagers = FindObjectsOfType<TimeTravelObjectManager>();
        for (var i = 0; i < ttoManagers.Length; ++i)
        {
            ttoManagers[i].Awake();
        }
    }

    private static void InitializeObjectDictionary()
    {
        ttosInScene = new Dictionary<GameObject, TimeTravelPeriod>();

        TimeTravelObject[] timeTravelObjects = Resources.FindObjectsOfTypeAll<TimeTravelObject>();
        if (timeTravelObjects.Length > 0)

        for (var i = 0; i < timeTravelObjects.Length; ++i)
        {
            TimeTravelPeriod timezone;

                if (timeTravelObjects[i].timeTravelPeriod == TimeTravelPeriod.Past)//(timeTravelObjects[i].pastSelf != null && timeTravelObjects[i].pastSelf.pastSelf != null)
                {
                    timezone = TimeTravelPeriod.Past;
                }
                else if (timeTravelObjects[i].timeTravelPeriod == TimeTravelPeriod.Present)//(timeTravelObjects[i].pastSelf != null)
                {
                    timezone = TimeTravelPeriod.Present;
                }
                else if (timeTravelObjects[i].timeTravelPeriod == TimeTravelPeriod.Future)
                {
                    timezone = TimeTravelPeriod.Future;
                }
                else
                {
                    timezone = TimeTravelPeriod.Dummy;
                }
            ttosInScene.Add(timeTravelObjects[i].gameObject, timezone);
            //Debug.Log(timeTravelObjects[i].name + ": " + timezone);
        }
    }

    private static void ActivateTimezone()
    {
        foreach(KeyValuePair<GameObject, TimeTravelPeriod> tto in ttosInScene)
        {
            if(tto.Value == currentTimezone)
            {
                tto.Key.SetActive(true);
            }
            else
            {
                tto.Key.SetActive(false);
            }
        }
    }

    private static void PrintObjects()
    {
        foreach (KeyValuePair<GameObject, TimeTravelPeriod> tto in ttosInScene)
        {
            Debug.Log(tto.Key.name + ": " + tto.Value);
        }
    }

    private static void AddTTOToDictionary(GameObject go, TimeTravelPeriod timezone)
    {
        // implement
        ttosInScene.Add(go, timezone);
    }
}