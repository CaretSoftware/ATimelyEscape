using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using CallbackSystem;

public class RuntimeSceneManager : MonoBehaviour {
    private static readonly HashSet<int> Room0 = new HashSet<int>() { 1, 2, 3 };    // cage room
    private static readonly HashSet<int> Room1 = new HashSet<int> { 2, 3, 4 };      // Incubator
    private static readonly HashSet<int> Room2 = new HashSet<int> { 3, 4, 5 };      // Office
    private static readonly HashSet<int> Room3 = new HashSet<int> { 5, 6, 7 };      // Corridor
    private static readonly HashSet<int> Room4 = new HashSet<int> { 6, 7, 8 };      // Lab Large
    private static readonly HashSet<int> Room5 = new HashSet<int> { 7, 8, 9 };      // Control Room
    private static readonly HashSet<int> Room6 = new HashSet<int> { 8, 9, 10 };      // Conveyor room
    private static readonly HashSet<int> Room7 = new HashSet<int> { 9, 10, 11 };     // Robot Factory
    private static readonly HashSet<int> Room8 = new HashSet<int> { 10, 11, 12 };    // Garden
    private static readonly HashSet<int> Room9 = new HashSet<int> { 11, 12, 13 };   // Cryo Hall
    private static readonly HashSet<int> Room10 = new HashSet<int> { 12, 13, 14 };  // Cryo Room

    private static readonly HashSet<int>[] Rooms = new HashSet<int>[] {
        Room0,
        Room1,
        Room2,
        Room3,
        Room4,
        Room5,
        Room6,
        Room7,
        Room8,
        Room9,
        Room10
    };

    private HashSet<int> activeScenes = new HashSet<int>();

    private int currentRoom = 0;

    private void Start() {
        PlayerEnterRoom.AddListener<PlayerEnterRoom>(OnPlayerEnterRoom);
        PlayerEnterRoom e = new PlayerEnterRoom() { roomIndex = 1 };
        e.Invoke();
    }
    public void OnPlayerEnterRoom(PlayerEnterRoom e) { LoadNeighbouringRooms(e.roomIndex); }

    private void LoadNeighbouringRooms(int newRoom) {
        if (newRoom == currentRoom) return;

        HashSet<int> exceptionScenes = new HashSet<int>(Rooms[newRoom]);
        exceptionScenes.ExceptWith(Rooms[currentRoom]);
        exceptionScenes.UnionWith(Rooms[currentRoom]);
        int[] scenesToUnload = exceptionScenes.ToArray();

        // unload rooms
        for (int sceneToUnload = 0; sceneToUnload < scenesToUnload.Length; sceneToUnload++) {
            if (activeScenes.Contains(scenesToUnload[sceneToUnload])) {
                SceneManager.UnloadSceneAsync(scenesToUnload[sceneToUnload], UnloadSceneOptions.None);
                activeScenes.Remove(scenesToUnload[sceneToUnload]);
            }
        }

        currentRoom = newRoom;

        // load rooms
        foreach (var sceneToLoad in Rooms[currentRoom]) {
            if (sceneToLoad == currentRoom) continue;
            SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
            activeScenes.Add(sceneToLoad);
        }
    }
}
