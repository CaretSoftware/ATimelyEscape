using System.Collections;
using System.Collections.Generic;
using CallbackSystem;
using UnityEngine;

/// <summary>
/// @author Emil Wessman
/// </summary>
public class SceneLoadMarker : MonoBehaviour {
    [SerializeField] private int sceneIndex;
    [SerializeField] private float sceneLoadCoolDownSeconds = 3;
    private bool readyToLoad = true;

    PlayerEnterRoom enterRoomEvent = new PlayerEnterRoom();

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            enterRoomEvent.sceneIndex = sceneIndex;
            enterRoomEvent.Invoke();
            readyToLoad = false;
            StartCoroutine(LoadCooldown());
        }
    }

    // to disallow constant calls to load a room
    private IEnumerator LoadCooldown() {
        yield return new WaitForSecondsRealtime(sceneLoadCoolDownSeconds);
        readyToLoad = true;
    }

}
