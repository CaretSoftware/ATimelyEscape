using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CallbackSystem;

public class TimeTravelObject : MonoBehaviour {
    public Vector3 Destiny { get; set; }
    public TimeTravelObject PastSelf { get; set; }
    public TimeTravelObject FutureSelf { get; set; }

    [SerializeField] private TimeTravelObject past, future;

    private Vector3 prevDestiny;

    // Start is called before the first frame update
    void Start() {
        PastSelf = past;
        FutureSelf = future;
        Destiny = prevDestiny = transform.position;
        DestinyChanged.AddListener<DestinyChanged>(OnDestinyChanged);
    }

    // Update is called once per frame
    void Update() {
        UpdateDestiny();
        if (Destiny != prevDestiny) {
            var changedEvent = new DestinyChanged { changedObject = this };
            changedEvent.Invoke();
            transform.position = Destiny;
        }

        prevDestiny = Destiny;
    }

    private void UpdateDestiny() { Destiny = PastSelf != null ? PastSelf.transform.position : transform.position; }

    private void OnDestinyChanged(DestinyChanged e) {
        if (e.changedObject != PastSelf) return;
        print($"Destiny of {name} has been changed!");
        Destiny = e.changedObject.Destiny;
    }
}