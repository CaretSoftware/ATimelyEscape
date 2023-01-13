using System.Collections.Generic;
using System.Linq;
using CallbackSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// @author Emil Wessman
/// </summary>
public class RuntimeSceneManager : MonoBehaviour {
    //Sets of each main room in the game and the rooms that should be loaded alongside them
    private static readonly HashSet<int> Room0 = new HashSet<int>() { 1, 2, 3, 5, 10 };    // cage room
    private static readonly HashSet<int> Room1 = new HashSet<int> { 2, 3, 4, 5, 10 };      // Incubator
    private static readonly HashSet<int> Room2 = new HashSet<int> { 2, 3, 4, 5, 10 };      // Office
    private static readonly HashSet<int> Room3 = new HashSet<int> { 3, 4, 5, 10 };      // Corridor
    private static readonly HashSet<int> Room4 = new HashSet<int> { 2, 3, 4, 5, 6, 10 };      // Lab Large
    private static readonly HashSet<int> Room5 = new HashSet<int> { 5, 6, 7 };      // Control Room
    private static readonly HashSet<int> Room6 = new HashSet<int> { 6, 7, 8 };      // Conveyor room
    private static readonly HashSet<int> Room7 = new HashSet<int> { 7, 8, 9 };     // Robot Factory
    private static readonly HashSet<int> Room8 = new HashSet<int> { 8, 9, 10 };    // Garden
    private static readonly HashSet<int> Room9 = new HashSet<int> { 2, 5, 9, 10, };   // Cryo Hall

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

    private const int ADDITIVEMAINSCENE_INDEX = 16;
    private Transform playerTransform;
    private HashSet<int> activeSceneIndexes = new HashSet<int>();
    public int ActiveSceneCount => activeSceneIndexes.Count;
    private int currentSceneIndex = 0;
    private int currentOnboardingSceneIndex = -1;
    public int CurrentSceneIndex => currentSceneIndex;
    public bool OnboardingRoomLoaded => currentOnboardingSceneIndex != -1;
    public int OnboadringRoomIndex => currentOnboardingSceneIndex;

    private void Start() {
        playerTransform = FindObjectOfType<NewRatCharacterController.NewRatCharacterController>().transform;
        SceneManager.sceneLoaded += OnSceneLoaded;
        PlayerEnterRoom.AddListener<PlayerEnterRoom>(OnPlayerEnterRoom);
    }

    private void OnPlayerEnterRoom(PlayerEnterRoom e) { LoadAndUnloadNeighbouringRooms(e.sceneIndex); }

    /// <summary>
    /// Callback function which main purpose is to initialise each new loaded room into the time period the player is currently in. 
    /// In case the scene loaded is an onboarding scene, it is also used to spawn the player at the correct position
    /// </summary>
    /// <param name="scene">Loaded scene</param>
    /// <param name="mode">Scene load mode</param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        TimeTravelManager.ReloadCurrentTimeTravelPeriod();
        GameObject onboardingSpawnMarker = GameObject.FindGameObjectWithTag("OnboardingSpawnMarker");
        if (onboardingSpawnMarker) {
            playerTransform.position = onboardingSpawnMarker.transform.position;
            playerTransform.LookAt(onboardingSpawnMarker.transform, Vector3.back);
            Camera.main.transform.LookAt(onboardingSpawnMarker.transform, Vector3.forward);
        }
    }
    private void OnDestroy() { SceneManager.sceneLoaded -= OnSceneLoaded; }
    public bool SceneIsLoaded(int sceneIndex) { return activeSceneIndexes.Contains(sceneIndex); }

    /// <summary>
    /// Onboarding (Tutorial) rooms are loaded separate from normal rooms because they are not a part of the main game and thus not "physically"
    /// adjacent to any of the other rooms. they also require the "game state" to be saved so the player can be taken back to where they left after finishing
    /// the tutorial. 
    /// </summary>
    /// <param name="onboardingSceneIndex">The index of the onboarding room in project build settings</param>
    public void LoadOnboardingRoom(int onboardingSceneIndex) {
        if (onboardingSceneIndex < 11 || currentOnboardingSceneIndex > -1) return; // room was not an onboarding room or an onboarding room was already loaded
        OnboardingHandler.LastSavedPosition = playerTransform.position;
        OnboardingHandler.LastSavedTimePeriod = TimeTravelManager.currentPeriod;
        SceneManager.LoadSceneAsync(onboardingSceneIndex, LoadSceneMode.Additive);
        currentOnboardingSceneIndex = onboardingSceneIndex;
    }

    /// <summary>
    /// Onboarding (Tutorial) rooms are unloaded separately because they do not have any sets of adjacently loaded rooms, 
    /// and require a restoration of game state
    /// </summary>
    public void UnloadOnboardingRoom() {
        if (currentOnboardingSceneIndex < 0) return; // no onboardingroom was loaded
        // Reload gamestate prior to starting tutorial
        playerTransform.position = OnboardingHandler.LastSavedPosition;
        TimeTravelManager.currentPeriod = OnboardingHandler.LastSavedTimePeriod;
        TimeTravelManager.ReloadCurrentTimeTravelPeriod();
        SceneManager.UnloadSceneAsync(currentOnboardingSceneIndex, UnloadSceneOptions.None);
        currentOnboardingSceneIndex = -1;
        NewRatCharacterController.NewRatCharacterController.Locked = false;
    }

    /// <summary>
    /// Unloads all currently loaded rooms. Useful in the level select system to be able to reload in case of some sort of soft-lock
    /// </summary>
    public void UnloadAllRooms() {
        foreach (int index in activeSceneIndexes) SceneManager.UnloadSceneAsync(index, UnloadSceneOptions.None);
        activeSceneIndexes.Clear();
        currentOnboardingSceneIndex = -1;
        currentSceneIndex = ADDITIVEMAINSCENE_INDEX;
    }

    private HashSet<int> dummySet = new HashSet<int>();
    /// <summary>
    /// Uses set exception between predetermined sets of scenes to be loaded for each scene in build settings to single which of the currently 
    /// loaded ones to unload. If any scenes exist in both sets they are not unloaded. The remaining scenes in the new index's set are then loaded.
    /// </summary>
    /// <param name="newSceneIndex"> The index of the new scene to load as set in project build settings </param>
    private void LoadAndUnloadNeighbouringRooms(int newSceneIndex) {
        if (newSceneIndex == currentSceneIndex) return;
        HashSet<int> exceptionScenes = new HashSet<int>((currentSceneIndex <= Rooms.Count() + 1) ? Rooms[currentSceneIndex] : dummySet);
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

}
