using System.Collections;
using System.Collections.Generic;
using CallbackSystem;
using UnityEngine;

public static class OnboardingHandler {
    private static bool cubeInteractionsDiscovered;
    private static bool cubeChargeDiscovered;
    private static bool timeTravelDiscovered;
    private static bool timeTravelFutureDiscovered;
    private static bool scientistDiscovered;
    private static bool vacuumCleanerDiscovered;

    public static Vector3 LastSavedPosition { get; set; }

    //CubePush
    public static bool CubeInteractionsDiscovered {
        get => cubeInteractionsDiscovered; set {
            if (!cubeInteractionsDiscovered) {
                cubeInteractionsDiscovered = value
                    ; if (value) { DebugEvent e = new DebugEvent() { DebugText = "interactions" }; e.Invoke(); }
            }
        }
    }

    //incubator
    public static bool CubeChargeDiscovered {
        get => cubeChargeDiscovered; set {
            if (!cubeChargeDiscovered) {
                cubeChargeDiscovered = value
                    ; if (value) { DebugEvent e = new DebugEvent() { DebugText = "charge" }; e.Invoke(); }
            }
        }
    }

    //incubator
    public static bool TimeTravelDiscovered {
        get => timeTravelDiscovered; set {
            if (!timeTravelDiscovered) {
                timeTravelDiscovered = value;
                if (value) { DebugEvent e = new DebugEvent() { DebugText = "timeTravel" }; e.Invoke(); }
            }
        }
    }

    // light detection
    public static bool TimeTravelFutureDiscovered {
        get => timeTravelFutureDiscovered; set {
            if (!timeTravelFutureDiscovered) {
                timeTravelFutureDiscovered = value;
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

}
