using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class TimezoneChangeEditor : EditorWindow
{

    public enum Timezone
    {
        past,
        present,
        future
    }
    public static Timezone timezone;


    [MenuItem("Tools/EnableTimezoneOption")]
    public static void Enable()
    {
        LoadCurrentTimezone();
        SceneView.duringSceneGui += DuringSceneGUI;
    }

    [MenuItem("Tools/DisableTimezoneOption")]
    public static void Disable()
    {
        SaveCurrentTimezone();
        SceneView.duringSceneGui -= DuringSceneGUI;
    }

    private static void LoadCurrentTimezone()
    {
        int currentTimezone = EditorPrefs.GetInt("TimezoneChangeEditor_currentTimezone", 1);

        switch (currentTimezone)
        {
            case 0:
                timezone = Timezone.past;
                break;
            case 2:
                timezone = Timezone.future;
                break;
            default:
                timezone = Timezone.present;
                break;
        }
    }

    private static void SaveCurrentTimezone()
    {
        int currentTimezone;

        switch (timezone)
        {
            case Timezone.past:
                currentTimezone = 0;
                break;
            case Timezone.future:
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
        timezone = (Timezone)EditorGUI.EnumPopup(new Rect(5, 10, 67, 15), timezone);
        if (EditorGUI.EndChangeCheck())
        {
            ChangeTimezone();
        }

        Handles.EndGUI();
    }

    private static void ChangeTimezone()
    {
        switch (timezone)
        {
            case Timezone.past:
                Debug.Log("Changed to past");
                break;
            case Timezone.present:
                Debug.Log("Changed to present");
                break;
            case Timezone.future:
                Debug.Log("Changed to future");
                break;
        }
    }
}