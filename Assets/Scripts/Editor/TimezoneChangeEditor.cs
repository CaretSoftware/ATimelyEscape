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
    public static bool editorIsEnabled;
    private static Dictionary<GameObject, TimeTravelPeriod> ttosInScene;

    [MenuItem("Tools/EnableTimezoneOption")]
    public static void Enable()
    {
        LoadCurrentTimezone();
        ActivateObjectsByTimezone();
        editorIsEnabled = true;
        SceneView.duringSceneGui += DuringSceneGUI;
    }

    [MenuItem("Tools/DisableTimezoneOption")]
    public static void Disable()
    {
        SaveCurrentTimezone();
        editorIsEnabled = false;
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
            ActivateObjectsByTimezone();
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


    /*private static void InitializeTTOManagers()
    {
        TimeTravelObjectManager[] ttoManagers = FindObjectsOfType<TimeTravelObjectManager>();
        for (var i = 0; i < ttoManagers.Length; ++i)
        {
            ttoManagers[i].Awake();
        }
    }*/

    /*public static void ActivateTimezone()
    {
        ActivateObjectsByTimezone();

        foreach (KeyValuePair<GameObject, TimeTravelPeriod> tto in ttosInScene)
        {
            if (currentTimezone == TimeTravelPeriod.Dummy)
            {
                tto.Key.SetActive(true);
            }
            else
            {
                if (tto.Value == currentTimezone)
                {
                    tto.Key.SetActive(true);
                }
                else
                {
                    tto.Key.SetActive(false);
                }
            }
        }
    }*/


    public static void ActivateObjectsByTimezone()
    {
        TimeTravelObjectManager[] timeTravelManagers = Resources.FindObjectsOfTypeAll<TimeTravelObjectManager>();

        if (timeTravelManagers.Length > 0)

            for (var i = 0; i < timeTravelManagers.Length; ++i)
            {
                ActivateObjects(timeTravelManagers[i]);
            }
    }


    private static void ActivateObjects(TimeTravelObjectManager ttoManager)
    {
        GameObject past = null;
        GameObject present = null;
        GameObject future = null;

        if (ttoManager.Past)
        {
            past = ttoManager.Past.gameObject;
        }

        if (ttoManager.Present)
        {
            present = ttoManager.Present.gameObject;
        }

        if (ttoManager.Future)
        {
            future = ttoManager.Future.gameObject;
        }

        switch (currentTimezone)
        {
            case TimeTravelPeriod.Dummy:
                if (past != null)
                {
                    past.SetActive(true);
                }
                if (present != null)
                {
                    present.SetActive(true);
                }
                if (future != null)
                {
                    future.SetActive(true);
                }
                break;

            case TimeTravelPeriod.Past:
                if (past != null)
                {
                    past.SetActive(true);
                }
                if (present != null)
                {
                    present.SetActive(false);
                }
                if (future != null)
                {
                    future.SetActive(false);
                }
                break;

            case TimeTravelPeriod.Present:
                if (past != null)
                {
                    past.SetActive(false);
                }
                if (present != null)
                {
                    present.SetActive(true);
                }
                if (future != null)
                {
                    future.SetActive(false);
                }
                break;

            case TimeTravelPeriod.Future:
                if (past != null)
                {
                    past.SetActive(false);
                }
                if (present != null)
                {
                    present.SetActive(false);
                }
                if (future != null)
                {
                    future.SetActive(true);
                }
                break;
        }
    }


    public static void AddTTOToDictionary(GameObject go, TimeTravelPeriod timezone)
    {
        // implement
        ttosInScene.Add(go, timezone);
    }
}