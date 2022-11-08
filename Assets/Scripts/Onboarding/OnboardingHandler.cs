using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CallbackSystem;

public static class OnboardingHandler
{
    private static bool climbingDiscovered;
    private static bool cubeDiscovered;
    private static bool timeTravelDiscovered;
    private static bool scientistDiscovered;
    private static bool vacuumCleanerDiscovered;
    private static bool darknessDiscovered;

    public static bool ClimbingDiscovered { get => climbingDiscovered; set { if (!climbingDiscovered) { climbingDiscovered = value
                    ; if (value) { DebugEvent e = new DebugEvent() { DebugText = "climbing" }; e.Invoke(); } } } }  

    public static bool CubeDiscovered { get => cubeDiscovered; set { if (!cubeDiscovered) { cubeDiscovered = value
                    ; if (value) { DebugEvent e = new DebugEvent() { DebugText = "cube" }; e.Invoke(); } } } }

    public static bool TimeTravelDiscovered { get => timeTravelDiscovered; set { if (!timeTravelDiscovered) {
                timeTravelDiscovered = value;
                    if (value) { DebugEvent e = new DebugEvent() { DebugText = "timeTravel" }; e.Invoke(); } } } }

    public static bool ScientistDiscovered { get => scientistDiscovered; set { if (!scientistDiscovered) { scientistDiscovered = value; 
                if (value) { DebugEvent e = new DebugEvent() { DebugText = "scientist" }; e.Invoke(); } } } }

    public static bool VacuumCleanerDiscovered { get => vacuumCleanerDiscovered; set { if (!vacuumCleanerDiscovered) { vacuumCleanerDiscovered = value; 
                if (value) { DebugEvent e = new DebugEvent() { DebugText = "vacuumCleaner" }; e.Invoke(); } } } }

    public static bool DarknessDiscovered { get => darknessDiscovered; set { if (!darknessDiscovered) { darknessDiscovered = value;
                if (value) { DebugEvent e = new DebugEvent() { DebugText = "darkness" }; e.Invoke(); } } } }

}
