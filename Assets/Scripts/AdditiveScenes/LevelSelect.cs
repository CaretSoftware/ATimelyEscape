using System.Collections;
using System.Collections.Generic;
using CallbackSystem;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// @author Emil Wessman
/// </summary>
public class LevelSelect : MonoBehaviour {
    [SerializeField] private Canvas canvas;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button loadButton;
    [SerializeField] private Toggle loadToggle;
    [SerializeField] private StartRoom startRoom = StartRoom.room1;

    public static LevelSelect Instance;
    private bool reloadIfLoaded;
    private TimeTravelPeriod periodToLoad;
    private Transform playerTransform;
    private NewRatCharacterController.NewCharacterInput input;
    private NewRatCharacterController.NewRatCharacterController controller;
    private RuntimeSceneManager sceneManager;
    private int latestRoomIndex;
    private int activeSceneCounter;
    private bool loadedFromLevelSelect;
    private const int ROOM_ONE_INDEX = 1, ROOM_TEN_INDEX = 10;

    // for the sake of easier testing and minimum confusion
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
    private static readonly Dictionary<int, RoomLoadInfo> loadInfo = new Dictionary<int, RoomLoadInfo>(){
        {1, new RoomLoadInfo(TimeTravelPeriod.Present, false, false, false, false, new Vector3(-0.292491376f, -0.254999995f,4.37516403f))}, //new Vector3(-0.291999996f, -0.254999995f, 4.42600012f))},
        {2, new RoomLoadInfo(TimeTravelPeriod.Present, false, false, false, false, new Vector3(-2.09899998f, 0.239999995f, 0.850000024f))},
        {3, new RoomLoadInfo(TimeTravelPeriod.Present, true, false, true, true, new Vector3(-4.37599993f, 0.175999999f, 1.95500004f))},
        {4, new RoomLoadInfo(TimeTravelPeriod.Present, true, false, true, true, new Vector3(-5.81074095f, -0.68900001f, 2.9472518f))},
        {5, new RoomLoadInfo(TimeTravelPeriod.Present, true, false, true, true, new Vector3(-7.31883526f, 0.504999995f, -0.995308697f))},
        {6, new RoomLoadInfo(TimeTravelPeriod.Future, false, false, false, false, new Vector3(2.7809999f, -0.460999995f, -1.41499996f))},
        {7, new RoomLoadInfo(TimeTravelPeriod.Future, true, true, true, true, new Vector3(3.86899996f, 2.86100006f, -7.13800001f))},
        {8, new RoomLoadInfo(TimeTravelPeriod.Past, true, true, true, true, new Vector3(-0.56099999f, 3.30599999f, -14.4390001f))},
        {9, new RoomLoadInfo(TimeTravelPeriod.Past, true, true, true, true, new Vector3(-3.39680004f, 2.81800008f, -21.3059998f))},
        {10, new RoomLoadInfo(TimeTravelPeriod.Future, true, true, true, true, new Vector3(-4.28200006f, 2.25869989f, -12.9075003f))}
    };

    // Start is called before the first frame update
    private void Start() {
        Instance = this;
        input = FindObjectOfType<NewRatCharacterController.NewCharacterInput>();
        FindObjectOfType<TimeTravelManager>().startPeriod = loadInfo[(int)startRoom].StartPeriod;
        controller = FindObjectOfType<NewRatCharacterController.NewRatCharacterController>();
        playerTransform = input.transform;
        sceneManager = FindObjectOfType<RuntimeSceneManager>();
        loadButton.onClick.AddListener(delegate { TriggerRoomLoad(); });
        loadToggle.onValueChanged.AddListener(delegate { reloadIfLoaded = loadToggle.isOn; });
        loadToggle.isOn = true; 
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        TriggerRoomLoad((int)startRoom);
    }

    /// <summary>
    /// Overload of the TriggerRoomLoad method intended for use with unity's input system. 
    /// Will try to read and load a room based on an index value from the in-game level select menu input field
    /// </summary>
    public void TriggerRoomLoad() {
        if (canvas.gameObject.activeSelf && System.Int32.TryParse(inputField.text, out int startRoom)) {
            controller.paused = false;
            TriggerRoomLoad(startRoom);
            inputField.text = "";
            canvas.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    /// <summary>
    /// Method to manually trigger the same thing a LoadMarker would do when the player enters it. Takes an index and clamps it between the available scenes in build settings.
    /// Forces the player into relevant game state. If the desired room is already loaded the player is simply teleported there.
    /// </summary>
    /// <param name="roomIndex">The index of the room to load in project build settings</param>
    public void TriggerRoomLoad(int roomIndex) {
        roomIndex = Mathf.Clamp(roomIndex, ROOM_ONE_INDEX, ROOM_TEN_INDEX);
        latestRoomIndex = roomIndex;
        if (reloadIfLoaded) sceneManager.UnloadAllRooms();
        TimeTravelManager.currentPeriod = loadInfo[roomIndex].StartPeriod;
        TimeTravelManager.desiredPeriod = loadInfo[roomIndex].StartPeriod;
        input.CanTimeTravel = loadInfo[roomIndex].CanTimeTravel;
        input.CanTimeTravelFuture = loadInfo[roomIndex].FutureAllowed;
        input.CanTimeTravelPresent = loadInfo[roomIndex].PresentAllowed;
        input.CanTimeTravelPast = loadInfo[roomIndex].PastAllowed;
        if (sceneManager.SceneIsLoaded(roomIndex)) playerTransform.position = loadInfo[latestRoomIndex].StartPosition;
        PlayerEnterRoom e = new PlayerEnterRoom() { sceneIndex = roomIndex };
        loadedFromLevelSelect = true;
        e.Invoke();
    }

    /// <summary>
    /// OnSceneLoaded and unloaded are used here to move the player to the correct spawn position in the room *after the scene has loaded* 
    /// It is to avoid the asynchrounous load from causing the player too "fall through the world" before the scene has loaded. This awy it is avoided
    /// </summary>
    /// <param name="scene">The loaded scene</param>
    /// <param name="mode">The scene load mode</param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        activeSceneCounter++;
        if (!sceneManager.OnboardingRoomLoaded && activeSceneCounter == sceneManager.ActiveSceneCount && loadedFromLevelSelect) {
            playerTransform.position = loadInfo[latestRoomIndex].StartPosition;
            loadedFromLevelSelect = false;
        }
    }

    private void OnSceneUnloaded(Scene scene) { activeSceneCounter--; }

    /// <summary>
    /// Alternative way to handle allowing or disallowing time travel in certain rooms. Has the drawback that it's only when entering a room and not
    /// sometime after having entered. Which is why in the final version a trigger system is used instead. 
    /// </summary>
    /// <param name="e">The enter room event</param>
    private void OnPlayerEnterRoom(PlayerEnterRoom e) {
        input.CanTimeTravel = loadInfo[e.sceneIndex].CanTimeTravel;
        input.CanTimeTravelFuture = loadInfo[e.sceneIndex].FutureAllowed;
        input.CanTimeTravelPresent = loadInfo[e.sceneIndex].PresentAllowed;
        input.CanTimeTravelPast = loadInfo[e.sceneIndex].PastAllowed;
    }

    private void OnDestroy() {
        Instance = null;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }
    public void EnableMenu() {
        canvas.gameObject.SetActive(!canvas.gameObject.activeSelf);
        input.CanTimeTravel = !canvas.gameObject.activeSelf;
        if (canvas.gameObject.activeSelf) inputField.Select();
        Cursor.lockState = canvas.gameObject.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
    }

    /// <summary>
    /// Data class to hold all relevant init info for each main room in the game
    /// </summary>
    public class RoomLoadInfo {
        public RoomLoadInfo(TimeTravelPeriod startPeriod, bool canTimeTravel, bool futureAllowed, bool presentAllowed, bool pastAllowed, Vector3 startPosition) {
            StartPeriod = startPeriod;
            CanTimeTravel = canTimeTravel;
            FutureAllowed = futureAllowed;
            PresentAllowed = presentAllowed;
            PastAllowed = pastAllowed;
            StartPosition = startPosition;
        }
        public TimeTravelPeriod StartPeriod { get; private set; }
        public bool CanTimeTravel { get; private set; }
        public bool FutureAllowed { get; private set; }
        public bool PresentAllowed { get; private set; }
        public bool PastAllowed { get; private set; }
        public Vector3 StartPosition { get; private set; }
    }
}
