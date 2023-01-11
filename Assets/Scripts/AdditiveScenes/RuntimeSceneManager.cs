using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CallbackSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RuntimeSceneManager : MonoBehaviour {

    [SerializeField] private StartRoom startRoom = StartRoom.room1;
    private Transform playerTransform;

    private static readonly HashSet<int> Room0 = new HashSet<int>() { 1, 2, 3, 5, 10 };    // cage room
    private static readonly HashSet<int> Room1 = new HashSet<int> { 2, 3, 4, 5, 10 };      // Incubator
    private static readonly HashSet<int> Room2 = new HashSet<int> { 2, 3, 4, 5, 10 };      // Office
    private static readonly HashSet<int> Room3 = new HashSet<int> { 3, 4, 5, 10 };      // Corridor
    private static readonly HashSet<int> Room4 = new HashSet<int> { 2, 3, 5, 6, 10 };      // Lab Large
    private static readonly HashSet<int> Room5 = new HashSet<int> { 5, 6, 7 };      // Control Room
    private static readonly HashSet<int> Room6 = new HashSet<int> { 6, 7, 8 };      // Conveyor room
    private static readonly HashSet<int> Room7 = new HashSet<int> { 7, 8, 9 };     // Robot Factory
    private static readonly HashSet<int> Room8 = new HashSet<int> { 8, 9, 10 };    // Garden
    private static readonly HashSet<int> Room9 = new HashSet<int> { 9, 10, };   // Cryo Hall

    public enum StartRoom {
        dummyDontUse,
        room1,
        room2,
        room3,
        room4,
        room5,
        room6,
        room7,
        room8,
        room9,
        room10
    }

    private static readonly Vector3[] startPositions = new Vector3[]{
        new Vector3(-0.291999996f,-0.254999995f,4.42600012f),
        new Vector3(-2.09899998f,0.239999995f,0.850000024f),
        new Vector3(-4.37599993f,0.175999999f,1.95500004f),
        new Vector3(-5.81074095f,-0.68900001f,2.9472518f),
        new Vector3(-7.31883526f,0.504999995f,-0.995308697f),
        new Vector3(2.7809999f,-0.460999995f,-1.41499996f),
        new Vector3(3.86899996f,2.86100006f,-7.13800001f),
        new Vector3(-0.56099999f,3.30599999f,-14.4390001f),
        new Vector3(-3.39680004f,2.81800008f,-21.3059998f),
        new Vector3(-4.28200006f,2.25869989f,-12.9075003f),
    };

    private static readonly TimeTravelPeriod[] startPeriods = new TimeTravelPeriod[]{
        TimeTravelPeriod.Present,
        TimeTravelPeriod.Present,
        TimeTravelPeriod.Present,
        TimeTravelPeriod.Present,
        TimeTravelPeriod.Present,
        TimeTravelPeriod.Future,
        TimeTravelPeriod.Future,
        TimeTravelPeriod.Past,
        TimeTravelPeriod.Past,
        TimeTravelPeriod.Future,
    };

    private static readonly HashSet<int>[] Rooms = new HashSet<int>[] {
        new HashSet<int>(),
        Room0,
        Room1,
        Room2,
        Room3,
        Room4,
        Room5,
        Room6,
        Room7,
        Room8,
        Room9
    };

    private HashSet<int> activeSceneIndexes = new HashSet<int>();

    private int currentSceneIndex = 0;
    private int currentOnboardingSceneIndex = -1;
    private int loadedScenesCounter = 0;
    public bool OnboardingRoomLoaded { get { return currentOnboardingSceneIndex != -1; } }
    public int OnboadringRoomIndex => currentOnboardingSceneIndex;

    private void Start() {
        /*     TimeTravelManager.SimulatePhysics = false;
            Physics.autoSimulation = false; */
        FindObjectOfType<TimeTravelManager>().startPeriod = startPeriods[(int)startRoom - 1];
        playerTransform = FindObjectOfType<NewRatCharacterController.NewRatCharacterController>().transform;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnLoaded;
        PlayerEnterRoom.AddListener<PlayerEnterRoom>(OnPlayerEnterRoom);
        playerTransform.transform.position = startPositions[(int)startRoom - 1];
        PlayerEnterRoom e = new PlayerEnterRoom() { sceneIndex = (int)startRoom };
        if ((int)startRoom > 1) FindObjectOfType<NewRatCharacterController.NewCharacterInput>().CanTimeTravel = true;
        e.Invoke();
    }

    private void OnPlayerEnterRoom(PlayerEnterRoom e) {
        /*      TimeTravelManager.SimulatePhysics = false;
             Physics.autoSimulation = false; */
        LoadAndUnloadNeighbouringRooms(e.sceneIndex);
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        TimeTravelManager.ReloadCurrentTimeTravelPeriod();
        loadedScenesCounter++;
        GameObject onboardingSpawnMarker = GameObject.FindGameObjectWithTag("OnboardingSpawnMarker");
        if (onboardingSpawnMarker) {
            playerTransform.position = onboardingSpawnMarker.transform.position;
            playerTransform.LookAt(onboardingSpawnMarker.transform, Vector3.back); //= onboardingSpawnMarker.transform.rotation;
            Camera.main.transform.LookAt(onboardingSpawnMarker.transform, Vector3.forward);
        }
        /*         if (loadedScenesCounter == activeSceneIndexes.Count) {
                    Physics.autoSimulation = true;
                    TimeTravelManager.SimulatePhysics = true;
                } */
    }
    private void OnSceneUnLoaded(Scene scene) {
        loadedScenesCounter--;
        Mathf.Clamp(loadedScenesCounter, 0, SceneManager.sceneCount);
    }

    public void TriggerRoomLoad(int sceneIndex) {

    }

    private void UnloadRooms(int newSceneIndex) {
        if (newSceneIndex == currentSceneIndex) return;

        HashSet<int> exceptionScenes = new HashSet<int>(Rooms[currentSceneIndex]);
        exceptionScenes.ExceptWith(Rooms[newSceneIndex]);
        int[] scenesToUnload = exceptionScenes.ToArray();

        // unload rooms
        for (int sceneToUnload = 0; sceneToUnload < scenesToUnload.Length; sceneToUnload++) {
            if (activeSceneIndexes.Contains(scenesToUnload[sceneToUnload])) {
                SceneManager.UnloadSceneAsync(scenesToUnload[sceneToUnload], UnloadSceneOptions.None);
                activeSceneIndexes.Remove(scenesToUnload[sceneToUnload]);
            }
        }

        currentSceneIndex = newSceneIndex;
    }

    private void LoadRooms(int newSceneIndex) {
        foreach (var sceneToLoad in Rooms[newSceneIndex]) {
            if ((sceneToLoad == newSceneIndex && activeSceneIndexes.Contains(sceneToLoad)) || activeSceneIndexes.Contains(sceneToLoad)) continue;
            SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
            activeSceneIndexes.Add(sceneToLoad);
        }
    }

    public void LoadOnboardingRoom(int onboardingSceneIndex) {
        if (onboardingSceneIndex < 11 || currentOnboardingSceneIndex > -1) return; // room was not an onboarding room or an onboarding room was already loaded
        OnboardingHandler.LastSavedPosition = playerTransform.position;
        OnboardingHandler.LastSavedTimePeriod = TimeTravelManager.currentPeriod;
        SceneManager.LoadSceneAsync(onboardingSceneIndex, LoadSceneMode.Additive);
        currentOnboardingSceneIndex = onboardingSceneIndex;
    }

    public void UnloadOnboardingRoom() {
        if (currentOnboardingSceneIndex < 0) return; // no onboardingroom was loaded
        // Reload gamestate prior to starting tutorial
        playerTransform.position = OnboardingHandler.LastSavedPosition;
        TimeTravelManager.currentPeriod = OnboardingHandler.LastSavedTimePeriod;
        TimeTravelManager.ReloadCurrentTimeTravelPeriod();
        SceneManager.UnloadSceneAsync(currentOnboardingSceneIndex, UnloadSceneOptions.None);
        currentOnboardingSceneIndex = -1;
    }

    public void UnloadAllRooms() {
        foreach (int index in activeSceneIndexes) {
            SceneManager.UnloadSceneAsync(index, UnloadSceneOptions.None);
        }
        activeSceneIndexes.Clear();
    }

    private void LoadAndUnloadNeighbouringRooms(int newSceneIndex) {
        if (newSceneIndex == currentSceneIndex) return;

        HashSet<int> exceptionScenes = new HashSet<int>(Rooms[currentSceneIndex]);
        exceptionScenes.ExceptWith(Rooms[newSceneIndex]);
        int[] scenesToUnload = exceptionScenes.ToArray();

        // unload rooms
        for (int sceneToUnload = 0; sceneToUnload < scenesToUnload.Length; sceneToUnload++) {
            if (activeSceneIndexes.Contains(scenesToUnload[sceneToUnload])) {
                SceneManager.UnloadSceneAsync(scenesToUnload[sceneToUnload], UnloadSceneOptions.None);
                activeSceneIndexes.Remove(scenesToUnload[sceneToUnload]);
            }
        }

        currentSceneIndex = newSceneIndex;

        // load rooms
        foreach (var sceneToLoad in Rooms[currentSceneIndex]) {
            if ((sceneToLoad == currentSceneIndex && activeSceneIndexes.Contains(sceneToLoad)) || activeSceneIndexes.Contains(sceneToLoad)) continue;
            SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
            activeSceneIndexes.Add(sceneToLoad);
        }
    }

    public int GetCurrentSceneIndex() { return currentSceneIndex; }
}
