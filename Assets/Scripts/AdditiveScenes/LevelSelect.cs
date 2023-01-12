using System.Collections;
using System.Collections.Generic;
using CallbackSystem;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        {1, new RoomLoadInfo(TimeTravelPeriod.Present, false, false, false, false, new Vector3(-0.291999996f, -0.254999995f, 4.42600012f))},
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
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        TriggerRoomLoad((int)startRoom);
    }

    public void TriggerRoomLoad() {
        if (canvas.gameObject.activeSelf && System.Int32.TryParse(inputField.text, out int startRoom)) {
            print(startRoom + " startroom");
            TriggerRoomLoad(startRoom);
            inputField.text = "";
            loadToggle.isOn = false;
            canvas.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void TriggerRoomLoad(int roomIndex) {
        if (roomIndex < 1) roomIndex = 1;
        else if (roomIndex > 10) roomIndex = 10;
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
        if (roomIndex == 1) input.CanTimeTravel = false;
        e.Invoke();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        activeSceneCounter++;
        if (!sceneManager.OnboardingRoomLoaded && activeSceneCounter == sceneManager.ActiveSceneCount && loadedFromLevelSelect) {
            playerTransform.position = loadInfo[latestRoomIndex].StartPosition;
            loadedFromLevelSelect = false;
        }

    }

    private void OnSceneUnloaded(Scene scene) {
        activeSceneCounter--;
    }

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
