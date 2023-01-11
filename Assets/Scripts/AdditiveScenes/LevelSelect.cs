using System.Collections;
using System.Collections.Generic;
using CallbackSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour {
    [SerializeField] private Canvas canvas;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private Button loadButton;
    [SerializeField] private Toggle loadToggle;
    public static LevelSelect Instance;
    private bool reloadIfLoaded;
    private TimeTravelPeriod periodToLoad;
    private Transform playerTransform;
    private NewRatCharacterController.NewCharacterInput input;
    private NewRatCharacterController.NewRatCharacterController controller;
    private RuntimeSceneManager sceneManager;

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

    // Start is called before the first frame update
    private void Start() {
        Instance = this;
        input = FindObjectOfType<NewRatCharacterController.NewCharacterInput>();
        controller = FindObjectOfType<NewRatCharacterController.NewRatCharacterController>();
        playerTransform = input.transform;
        sceneManager = FindObjectOfType<RuntimeSceneManager>();
        loadButton.onClick.AddListener(delegate { TriggerRoomLoad(); });
        loadToggle.onValueChanged.AddListener(delegate { reloadIfLoaded = loadToggle.isOn; });
        dropdown.onValueChanged.AddListener(delegate {
            switch (dropdown.value) {
                case 0: periodToLoad = TimeTravelPeriod.Past; break;
                case 1: periodToLoad = TimeTravelPeriod.Present; break;
                case 2: periodToLoad = TimeTravelPeriod.Future; break;
            }
        });
    }

    public void TriggerRoomLoad() {
        if (System.Int32.TryParse(inputField.text, out int startRoom)) {
            TriggerRoomLoad(startRoom);
            canvas.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void TriggerRoomLoad(int roomIndex) {
        if (roomIndex < 1) roomIndex = 1;
        else if (roomIndex > 10) roomIndex = 10;
        if (reloadIfLoaded) sceneManager.UnloadAllRooms();
        playerTransform.transform.position = startPositions[roomIndex - 1];
        PlayerEnterRoom e = new PlayerEnterRoom() { sceneIndex = roomIndex };
        if (roomIndex == 1) input.CanTimeTravel = false;
        e.Invoke();
    }

    private void OnDestroy() { Instance = null; }
    public void EnableMenu() {
        canvas.gameObject.SetActive(!canvas.gameObject.activeSelf);
        input.CanTimeTravel = !canvas.gameObject.activeSelf;
        if (!canvas.gameObject.activeSelf) inputField.text = "";
        if (canvas.gameObject.activeSelf) inputField.Select();
        Cursor.lockState = canvas.gameObject.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
    }

}
