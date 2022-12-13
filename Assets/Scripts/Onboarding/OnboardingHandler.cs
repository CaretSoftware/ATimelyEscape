using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CallbackSystem;

public static class OnboardingHandler {
    private static bool climbingAndCubeDiscovered;
    private static bool timeTravelDiscovered;
    private static bool timeTravelFutureDiscovered;
    private static bool scientistDiscovered;
    private static bool vacuumCleanerDiscovered;
    private static bool darknessDiscovered;

    public static bool ClimbingAndCubeDiscovered {
        get => climbingAndCubeDiscovered; set {
            if (!climbingAndCubeDiscovered) {
                climbingAndCubeDiscovered = value
                    ; if (value) { DebugEvent e = new DebugEvent() { DebugText = "climbing" }; e.Invoke(); }
            }
        }
    }

    public static bool TimeTravelDiscovered {
        get => timeTravelDiscovered; set {
            if (!timeTravelDiscovered) {
                timeTravelDiscovered = value;
                if (value) { DebugEvent e = new DebugEvent() { DebugText = "timeTravel" }; e.Invoke(); }
            }
        }
    }

    public static bool TimeTravelFutureDiscovered {
        get => timeTravelDiscovered; set {
            if (!timeTravelDiscovered) {
                timeTravelDiscovered = value;
                if (value) { DebugEvent e = new DebugEvent() { DebugText = "timeTravelFuture" }; e.Invoke(); }
            }
        }
    }

    public static bool ScientistDiscovered {
        get => scientistDiscovered; set {
            if (!scientistDiscovered) {
                scientistDiscovered = value;
                if (value) { DebugEvent e = new DebugEvent() { DebugText = "scientist" }; e.Invoke(); }
            }
        }
    }

    public static bool VacuumCleanerDiscovered {
        get => vacuumCleanerDiscovered; set {
            if (!vacuumCleanerDiscovered) {
                vacuumCleanerDiscovered = value;
                if (value) { DebugEvent e = new DebugEvent() { DebugText = "vacuumCleaner" }; e.Invoke(); }
            }
        }
    }

    public static bool DarknessDiscovered {
        get => darknessDiscovered; set {
            if (!darknessDiscovered) {
                darknessDiscovered = value;
                if (value) { DebugEvent e = new DebugEvent() { DebugText = "darkness" }; e.Invoke(); }
            }
        }
    }

}
