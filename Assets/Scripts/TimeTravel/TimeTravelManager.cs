using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CallbackSystem;
using UnityEngine;
using StateMachines;
using TMPro;
using UnityEditor;

public class TimeTravelManager : MonoBehaviour {
    private StateMachine stateMachine;
    [SerializeField] private TimeTravelPeriod startPeriod;
    [SerializeField] private TextMeshProUGUI timeText;
    public static TimeTravelPeriod currentPeriod;
    public static List<Rigidbody> MovableObjects = new List<Rigidbody>();

    // Start is called before the first frame update
    void Awake() {
        stateMachine = new StateMachine(this, new State[] { new PastState(), new PresentState(), new FutureState() });
        TimePeriodChanged.AddListener<TimePeriodChanged>(OnTimeTravel);
    }

    // Update is called once per frame
    void Update() { stateMachine.Run(); }

    public static void SimulateMovableObjectPhysics(int maxIterations) {
        Physics.autoSimulation = false;

        for (int i = 0; i < maxIterations; i++) {
            Physics.Simulate(Time.fixedDeltaTime);
            if (MovableObjects.All(rb => rb.IsSleeping())) break;
        }

        Physics.autoSimulation = true;
    }

    private void OnTimeTravel(TimePeriodChanged e) {
        var beforeSim = new DebugEvent();
        beforeSim.DebugText = "BeforeSimulation";
        beforeSim.Invoke();

        SimulateMovableObjectPhysics(1000);
        var simulationComplete = new PhysicsSimulationComplete();
        simulationComplete.from = e.from;
        simulationComplete.to = e.to;
        simulationComplete.Invoke();

        switch (e.to) {
            case TimeTravelPeriod.Past:
                timeText.text = "Current Time Period: Past";
                break;
            case TimeTravelPeriod.Present:
                timeText.text = "Current Time Period: Present";
                break;
            case TimeTravelPeriod.Future:
                timeText.text = "Current Time Period: Future";
                break;
        }
    }
}

public enum TimeTravelPeriod {
    Past = 0,
    Present = 1,
    Future = 2,
    Dummy = 3
}

namespace StateMachines {
    public class PastState : State {
        private TimeTravelPeriod travellingTo;

        public override void Run() {
            if (Input.GetKey(KeyCode.Alpha2)) {
                StateMachine.TransitionTo<PresentState>();
                travellingTo = TimeTravelPeriod.Present;
            }

            if (Input.GetKey(KeyCode.Alpha3)) {
                StateMachine.TransitionTo<FutureState>();
                travellingTo = TimeTravelPeriod.Future;
            }
        }

        public override void Exit() {
            var travelEvent = new TimePeriodChanged() { from = TimeTravelPeriod.Past, to = travellingTo };
            travelEvent.Invoke();
            Debug.Log("Travelled to: " + travellingTo.ToString());
            TimeTravelManager.currentPeriod = travellingTo;
        }
    }

    public class PresentState : State {
        private TimeTravelPeriod travellingTo;

        public override void Run() {
            if (Input.GetKey(KeyCode.Alpha1)) {
                StateMachine.TransitionTo<PastState>();
                travellingTo = TimeTravelPeriod.Past;
            }

            if (Input.GetKey(KeyCode.Alpha3)) {
                StateMachine.TransitionTo<FutureState>();
                travellingTo = TimeTravelPeriod.Future;
            }
        }

        public override void Exit() {
            var travelEvent = new TimePeriodChanged() { from = TimeTravelPeriod.Present, to = travellingTo };
            travelEvent.Invoke();
            Debug.Log("Travelled to: " + travellingTo.ToString());
            TimeTravelManager.currentPeriod = travellingTo;
        }
    }

    public class FutureState : State {
        private TimeTravelPeriod travellingTo;

        public override void Run() {
            if (Input.GetKey(KeyCode.Alpha1)) {
                StateMachine.TransitionTo<PastState>();
                travellingTo = TimeTravelPeriod.Past;
            }

            if (Input.GetKey(KeyCode.Alpha2)) {
                StateMachine.TransitionTo<PresentState>();
                travellingTo = TimeTravelPeriod.Present;
            }
        }

        public override void Exit() {
            var travelEvent = new TimePeriodChanged() { from = TimeTravelPeriod.Future, to = travellingTo };
            travelEvent.Invoke();
            Debug.Log("Travelled to: " + travellingTo.ToString());
            TimeTravelManager.currentPeriod = travellingTo;
        }
    }
}