using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMTester : MonoBehaviour {
    private StateMachine sm;
    [SerializeField] private State[] states;

    // Start is called before the first frame update
    void Start() { sm = new StateMachine(this, states); }

    // Update is called once per frame
    void Update() { sm.Run(); }
}