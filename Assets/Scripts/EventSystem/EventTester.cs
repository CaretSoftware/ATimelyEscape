using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CallbackSystem;

// Example of how to use the EventSystem
public class EventTester : MonoBehaviour {
    public bool useDToDelete;
    private DieEvent dieEvent = new DieEvent();

    // Start is called before the first frame update
    void Start() {
        dieEvent.AddListener<DieEvent>(Dummy);
        dieEvent.timeToDestroy = 0.2f;
    }

    // Update is called once per frame
    void Update() {
        if (!useDToDelete && Input.GetKey(KeyCode.Space)) {
            dieEvent.RemoveListener<DieEvent>(Dummy);
            /*print("Unregistering!");*/
        }

        if (useDToDelete && Input.GetKey(KeyCode.D))
            dieEvent.RemoveListener<DieEvent>(Dummy);

        if (Input.GetKey(KeyCode.W)) {
            /*print("Invoking!");*/
            dieEvent.Invoke();
        }

        if (Input.GetKey(KeyCode.S)) {
            /*print("Registering!");*/
            dieEvent.AddListener<DieEvent>(Dummy);
        }
    }

    private void Dummy(DieEvent e) { print("Time to destroy: " + e.timeToDestroy); }
}