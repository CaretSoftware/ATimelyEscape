using System;
using System.Collections;
using System.Collections.Generic;
using CallbackSystem;
using UnityEngine;
using StateMachines;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using State = StateMachines.State;
using StateMachine = StateMachines.StateMachine;

public class TimeTravelManager : MonoBehaviour {
    private StateMachine stateMachine;
    [SerializeField] private TimeTravelPeriod startPeriod;
    public static TimeTravelPeriod currentPeriod;

    // Start is called before the first frame update
    void Start() {
        stateMachine = new StateMachine(this, new State[] { new PastState(), new PresentState(), new FutureState() });
        var periodChange = new TimePeriodChanged() { gameObject = this.gameObject, to = startPeriod };
        periodChange.Invoke();
    }

    // Update is called once per frame
    void Update() { stateMachine.Run(); }

    private void OnDrawGizmos() {
        Handles.Label(new Vector3(0f, 5f, 0f), "Current time period: " + currentPeriod.ToString());
    }
}

public enum TimeTravelPeriod {
    Past = 0,
    Present = 1,
    Future = 2
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