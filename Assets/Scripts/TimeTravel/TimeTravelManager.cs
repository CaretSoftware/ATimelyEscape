using System;
using System.Collections.Generic;
using System.Linq;
using CallbackSystem;
using RatCharacterController;
using StateMachines;
using TMPro;
using UnityEngine;

public class TimeTravelManager : MonoBehaviour {
    private StateMachine stateMachine;
    [SerializeField] private TimeTravelPeriod startPeriod;
    [SerializeField] private TextMeshProUGUI timeText;
    public static TimeTravelPeriod currentPeriod;
    public static TimeTravelPeriod desiredPeriod;
    public static HashSet<Rigidbody> MovableObjects = new HashSet<Rigidbody>();
    public static Transform playerTransform;
    public static bool SimulatePhysics { get; set; }

    public static readonly Dictionary<TimeTravelPeriod, Type> PeriodStates = new Dictionary<TimeTravelPeriod, Type>() {
        { TimeTravelPeriod.Past, typeof(PastState) },
        { TimeTravelPeriod.Present, typeof(PresentState) },
        { TimeTravelPeriod.Future, typeof(FutureState) },
    };

    // Start is called before the first frame update
    void Start() {
        MovableObjects.Clear();
        playerTransform = FindObjectOfType<CharacterAnimationController>().transform;
        stateMachine = new StateMachine(this,
            new State[] {
                new PastState() { thisPeriod = TimeTravelPeriod.Past },
                new PresentState() { thisPeriod = TimeTravelPeriod.Present },
                new FutureState() { thisPeriod = TimeTravelPeriod.Future }
            });
        currentPeriod = startPeriod;
        ReloadCurrentTimeTravelPeriod();
        TimePeriodChanged.AddListener<TimePeriodChanged>(OnTimeTravel);
    }

    // Update is called once per frame
    void Update() { stateMachine.Run(); }

    public static bool DesiredTimePeriod(TimeTravelPeriod desired) {
        if (desired == currentPeriod) return false;
        desiredPeriod = desired;
        return true;
    }

    public static void ReloadCurrentTimeTravelPeriod() {
        if (currentPeriod == TimeTravelPeriod.Dummy) return;
        TimePeriodChanged e = new TimePeriodChanged() { from = TimeTravelPeriod.Dummy, to = currentPeriod, IsReload = true };
        Debug.Log($"Reloading current time period: {currentPeriod}");
        e.Invoke();
    }

    public static void SimulateMovableObjectPhysics(int maxIterations) {
        if (!SimulatePhysics) return;
        Physics.autoSimulation = false;

        for (int i = 0; i < maxIterations; i++) {
            Physics.Simulate(Time.fixedDeltaTime);
            if (MovableObjects.All(rb => rb.IsSleeping())) break;
        }

        Physics.autoSimulation = true;
    }

    private void OnTimeTravel(TimePeriodChanged e) {
        var beforeSim = new DebugEvent { DebugText = "BeforeSimulation" };
        beforeSim.Invoke();

        SimulateMovableObjectPhysics(50);
        var simulationComplete = new PhysicsSimulationComplete { from = e.from, to = e.to };
        simulationComplete.Invoke();

        /*switch (e.to) {
            case TimeTravelPeriod.Past:
                timeText.text = "Current Time Period: Past";
                break;
            case TimeTravelPeriod.Present:
                timeText.text = "Current Time Period: Present";
                break;
            case TimeTravelPeriod.Future:
                timeText.text = "Current Time Period: Future";
                break;
        }*/
    }
}

public enum TimeTravelPeriod {
    Past,
    Present,
    Future,
    Dummy
}


namespace StateMachines {
    public class TimePeriodState : State {
        private TimeTravelPeriod travellingTo;
        public TimeTravelPeriod thisPeriod;

        public class Dummy : MonoBehaviour { }
        private Dummy coroutineRunner;
        private GameObject runnerObject = new GameObject("RunnerObject");

        public override void Run() {

            if (TimeTravelManager.currentPeriod != TimeTravelManager.desiredPeriod) {

                LayerMask mask = 0;
                switch (TimeTravelManager.desiredPeriod) {
                    case TimeTravelPeriod.Past: mask = LayerMask.GetMask("PastTimePeriod"); break;
                    case TimeTravelPeriod.Present: mask = LayerMask.GetMask("PresentTimePeriod"); break;
                    case TimeTravelPeriod.Future: mask = LayerMask.GetMask("FutureTimePeriod"); break;
                }

                var cols = Physics.OverlapCapsule(
                    new Vector3(TimeTravelManager.playerTransform.position.x,
                        TimeTravelManager.playerTransform.position.y + 0.2f,
                        TimeTravelManager.playerTransform.position.z),
                    new Vector3(TimeTravelManager.playerTransform.position.x,
                        TimeTravelManager.playerTransform.position.y + 0.1f,
                        TimeTravelManager.playerTransform.position.z), 0.05f, mask);

                if (cols.Length == 0 || cols.All(c => c.isTrigger)) {
                    State nextState = StateMachine.stateDict[
                        TimeTravelManager.PeriodStates[TimeTravelManager.desiredPeriod]];
                    StateMachine.TransitionTo(nextState);
                    travellingTo = TimeTravelManager.desiredPeriod;
                    if (TimeTravelManager.currentPeriod == TimeTravelPeriod.Dummy) Exit();
                } else {
                    TimeTravelManager.desiredPeriod = TimeTravelManager.currentPeriod;
                    Debug.LogError("You tried Time Travelling into another object!");

                    CallHintAnimation callHint = new CallHintAnimation() { animationName = "TravelWarning", waitForTime = 0.5f };
                    callHint.Invoke();
                }
            }
        }

        public override void Exit() {
            if (!coroutineRunner) coroutineRunner = runnerObject.AddComponent<Dummy>();

            var travelEvent = new TimePeriodChanged() { from = thisPeriod, to = travellingTo };
            travelEvent.Invoke();
            Debug.Log("Travelled to: " + travellingTo);
            TimeTravelManager.currentPeriod = travellingTo;
            coroutineRunner.StartCoroutine(DisableTimeTravelDuringEffect());
        }

        private IEnumerator<WaitForSecondsRealtime> DisableTimeTravelDuringEffect() {
            TimeTravelManager.FindObjectOfType<CharacterInput>().CanTimeTravel = false;
            yield return new WaitForSecondsRealtime(0.4f);
            TimeTravelManager.FindObjectOfType<CharacterInput>().CanTimeTravel = true;
        }
    }

    // would rather do without these, but state machine needs a rewrite for that to work
    public class PastState : TimePeriodState { }

    public class PresentState : TimePeriodState { }

    public class FutureState : TimePeriodState { }
}