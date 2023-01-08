using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelect : MonoBehaviour {
    [SerializeField] private Canvas canvas;
    private NewRatCharacterController.NewRatCharacterController controller;
    public static LevelSelect Instance;

    // Start is called before the first frame update
    private void Start() {
        Instance = this;
        print(canvas == null);
        controller = FindObjectOfType<NewRatCharacterController.NewRatCharacterController>();
    }

    private void OnDestroy() { Instance = null; }
    public void EnableMenu() {
        print("EnableMenuPressed");
        canvas.gameObject.SetActive(!canvas.gameObject.activeSelf);
        controller.paused = canvas.gameObject.activeSelf;
    }

}
