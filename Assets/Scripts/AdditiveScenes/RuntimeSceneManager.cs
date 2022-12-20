using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CallbackSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RuntimeSceneManager : MonoBehaviour {
    private static readonly HashSet<int> Room0 = new HashSet<int>() { 1, 2, 3, 5, 10 };    // cage room
    private static readonly HashSet<int> Room1 = new HashSet<int> { 2, 3, 4, 5, 10 };      // Incubator
    private static readonly HashSet<int> Room2 = new HashSet<int> { 2, 3, 4, 5, 10 };      // Office
    private static readonly HashSet<int> Room3 = new HashSet<int> { 3, 4, 5, 10 };      // Corridor
    private static readonly HashSet<int> Room4 = new HashSet<int> { 2, 3, 5, 6, 10 };      // Lab Large
    private static readonly HashSet<int> Room5 = new HashSet<int> { 5, 6, 7 };      // Control Room
    private static readonly HashSet<int> Room6 = new HashSet<int> { 6, 7, 8 };      // Conveyor room
    private static readonly HashSet<int> Room7 = new HashSet<int> { 7, 8, 9 };     // Robot Factory
    private static readonly HashSet<int> Room8 = new HashSet<int> { 8, 9, 10 };    // Garden
    private static readonly HashSet<int> Room9 = new HashSet<int> { 2, 5, 9, 10, };   // Cryo Hall

    /*
1 Ladda in Kuvös(r2), Kontoret(R3), Korridoren(R4), Stora Labbet(R5) och sista rummet(R10
2. Ladda ur R1
3 Ladda in ÖvervakningsrummetR6 och ladda ur Kuvös(r2), Kontoret(R3), Korridoren(R4), Stora Labbet(R5) och sista rummet(R10
4. Ladda in R7 ladda ur R6
5 Ladda in R8 
6 Ladda in R9 och  Ladda ut R7
7 Ladda in R10, R2Kuvös och R5, Ladda ur R8
8.Ladda ur R9
9. Ladda ur R2Kuvös och R5

    */

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
    private int loadedScenesCounter = 0;

    private void Start() {
        /*     TimeTravelManager.SimulatePhysics = false;
            Physics.autoSimulation = false; */
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnLoaded;
        PlayerEnterRoom.AddListener<PlayerEnterRoom>(OnPlayerEnterRoom);
        PlayerEnterRoom e = new PlayerEnterRoom() { sceneIndex = 1 };
        e.Invoke();
    }

    public void OnPlayerEnterRoom(PlayerEnterRoom e) {
        /*      TimeTravelManager.SimulatePhysics = false;
             Physics.autoSimulation = false; */
        LoadAndUnloadNeighbouringRooms(e.sceneIndex);
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        TimeTravelManager.ReloadCurrentTimeTravelPeriod();
        loadedScenesCounter++;
        /*         if (loadedScenesCounter == activeSceneIndexes.Count) {
                    Physics.autoSimulation = true;
                    TimeTravelManager.SimulatePhysics = true;
                } */
    }
    void OnSceneUnLoaded(Scene scene) {
        loadedScenesCounter--;
        Mathf.Clamp(loadedScenesCounter, 0, SceneManager.sceneCount);
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
