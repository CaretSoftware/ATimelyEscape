using System;
using System.Collections.Generic;
using System.Linq;
using CallbackSystem;
using StateMachines;
using UnityEngine;

public class TimeTravelManager : MonoBehaviour {
    private StateMachine stateMachine;
    public TimeTravelPeriod startPeriod = TimeTravelPeriod.Present;
    public static TimeTravelPeriod currentPeriod;
    public static TimeTravelPeriod desiredPeriod;
    public static HashSet<Rigidbody> MovableObjects = new HashSet<Rigidbody>();
    public static Transform playerTransform;
    public static bool SimulatePhysics { get; set; }
    public StateMachine StateMachine => stateMachine;

    public static readonly Dictionary<TimeTravelPeriod, Type> PeriodStates = new Dictionary<TimeTravelPeriod, Type>() {
        { TimeTravelPeriod.Past, typeof(PastState) },
        { TimeTravelPeriod.Present, typeof(PresentState) },
        { TimeTravelPeriod.Future, typeof(FutureState) },
    };

    // Start is called before the first frame update
    void Start() {
        MovableObjects.Clear();
        playerTransform = FindObjectOfType<NewRatCharacterController.NewRatAnimationController>().transform;
        stateMachine = new StateMachine(this,
            new State[] {
                new PastState() { thisPeriod = TimeTravelPeriod.Past },
                new PresentState() { thisPeriod = TimeTravelPeriod.Present },
                new FutureState() { thisPeriod = TimeTravelPeriod.Future }
            });
        currentPeriod = startPeriod;
        desiredPeriod = startPeriod;
        TimePeriodChanged.AddListener<TimePeriodChanged>(OnTimeTravel);
        ReloadCurrentTimeTravelPeriod();
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
        TimeTravelManager manager = FindObjectOfType<TimeTravelManager>();
        TimePeriodChanged e = new TimePeriodChanged() { from = TimeTravelPeriod.Dummy, to = currentPeriod, IsReload = true };
        TimeTravelManager.desiredPeriod = currentPeriod;

        State nextState = manager.stateMachine.stateDict[TimeTravelManager.PeriodStates[currentPeriod]];
        manager.stateMachine.CurrentState = nextState;
        manager.stateMachine.QueuedState = nextState;
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
        private NewRatCharacterController.NewCharacterInput input;

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
                    Exit();
                } else {
                    // Player tried time travelling into another object
                    TimeTravelManager.desiredPeriod = TimeTravelManager.currentPeriod;
                    CallHintAnimation callHint = new CallHintAnimation() { animationName = "TravelWarning" };
                    callHint.Invoke();
                }
            }
        }

        public override void Exit() {
            if (!coroutineRunner) coroutineRunner = runnerObject.AddComponent<Dummy>();
            if (!input) input = TimeTravelManager.FindObjectOfType<NewRatCharacterController.NewCharacterInput>();

            var travelEvent = new TimePeriodChanged() { from = thisPeriod, to = travellingTo };
            travelEvent.Invoke();
            TimeTravelManager.currentPeriod = travellingTo;
            coroutineRunner.StartCoroutine(DisableTimeTravelDuringEffect(input.CanTimeTravel));
        }

        private IEnumerator<WaitForSecondsRealtime> DisableTimeTravelDuringEffect(bool timeTravelEneabled) {
            if (timeTravelEneabled) input.CanTimeTravel = false;
            yield return new WaitForSecondsRealtime(0.4f);
            if (timeTravelEneabled) input.CanTimeTravel = true;
        }
    }

    // would rather do without these, but state machine needs a rewrite for that to work
    public class PastState : TimePeriodState { }

    public class PresentState : TimePeriodState { }

    public class FutureState : TimePeriodState { }
}