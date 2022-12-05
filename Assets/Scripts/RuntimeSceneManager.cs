using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RuntimeSceneManager : MonoBehaviour {
    private static readonly HashSet<int> Room0 = new HashSet<int>() { 0, 1, 2 };    // cage room
    private static readonly HashSet<int> Room1 = new HashSet<int> { 1, 2, 3 };      // Incubator
    private static readonly HashSet<int> Room2 = new HashSet<int> { 2, 3, 4 };      // Office
    private static readonly HashSet<int> Room3 = new HashSet<int> { 4, 5, 6 };      // Corridor
    private static readonly HashSet<int> Room4 = new HashSet<int> { 5, 6, 7 };      // Lab Large
    private static readonly HashSet<int> Room5 = new HashSet<int> { 6, 7, 8 };      // Control Room
    private static readonly HashSet<int> Room6 = new HashSet<int> { 7, 8, 9 };      // Conveyor room
    private static readonly HashSet<int> Room7 = new HashSet<int> { 8, 9, 10 };     // Robot Factory
    private static readonly HashSet<int> Room8 = new HashSet<int> { 9, 10, 11 };    // Garden
    private static readonly HashSet<int> Room9 = new HashSet<int> { 10, 11, 12 };   // Cryo Hall
    private static readonly HashSet<int> Room10 = new HashSet<int> { 11, 12, 13 };  // Cryo Room

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
    
    private int _currentRoom = 0;
    
    private void Awake() {
        
    }

    public void RatEnteredRoom(int room) {
        LoadNeighbouringRooms(room);
    }

    private void LoadNeighbouringRooms(int room) {
        if (room == _currentRoom) return;

        HashSet<int> exceptionScenes = new HashSet<int>(Rooms[room]);
        exceptionScenes.ExceptWith(Rooms[_currentRoom]);
        exceptionScenes.UnionWith(Rooms[_currentRoom]);
        int[] scenesToUnload = exceptionScenes.ToArray();
        
        // unload rooms
        for (int sceneToUnload = 0; sceneToUnload < scenesToUnload.Length; sceneToUnload++) {
            // for (int previousRooms = 0; previousRooms < UPPER; previousRooms++)
            // {
                
            // }
            SceneManager.UnloadSceneAsync(0, UnloadSceneOptions.None);
        }
        
        _currentRoom = room;
        
        // load rooms
        for (int neighbourRooms = 0; neighbourRooms < Rooms[room].Count; neighbourRooms++) {
           // int sceneToLoad = Rooms[room][neighbourRooms];
           // if (sceneToLoad == _currentRoom) continue;
            
           // SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        }
    }
}
