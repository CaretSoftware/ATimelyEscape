using System;
using System.Collections.Generic;
using System.Linq;
using CallbackSystem;
using StateMachines;
using UnityEngine;

/// <summary>
/// @author Emil Wessman 
/// </summary>
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

    /// <summary>
    /// Exists to allow for rooms to be "initiated" into the currently active time period, as rooms outside of runtime do not exist in any particular time perido
    /// </summary>
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

    /// <summary>
    /// Makes sure that time travel objects with rigidbodies (effectively only the cubes) avoid paradoxes. 
    /// A cube suspended in the air in the present should not continue to be in the future
    /// </summary>
    /// <param name="maxIterations">How many iterations to simulate the unity physics for</param>
    public static void SimulateMovableObjectPhysics(int maxIterations) {
        if (!SimulatePhysics) return;
        Physics.autoSimulation = false;

        for (int i = 0; i < maxIterations; i++) {
            Physics.Simulate(Time.fixedDeltaTime);
            if (MovableObjects.All(rb => rb.IsSleeping())) break;
        }
        Physics.autoSimulation = true;
    }

    /// <summary>
    /// Callback function to respond to the player time travelling. Used to simulate physics with SimulateMovableObjectPhysics()
    /// </summary>
    /// <param name="e">The Time travel event</param>
    private void OnTimeTravel(TimePeriodChanged e) {
        var beforeSim = new DebugEvent { DebugText = "BeforeSimulation" };
        beforeSim.Invoke();

        SimulateMovableObjectPhysics(50);
        var simulationComplete = new PhysicsSimulationComplete { from = e.from, to = e.to };
        simulationComplete.Invoke();
    }
}

/// <summary>
/// Universal names and values for each time period, used throughout the entire project
/// </summary>
public enum TimeTravelPeriod {
    Past,
    Present,
    Future,
    Dummy
}


namespace StateMachines {
    /// <summary>
    /// Main state for each time period used in TimeTravelManger statemachine
    /// </summary>
    public class TimePeriodState : State {
        private TimeTravelPeriod travellingTo;
        public TimeTravelPeriod thisPeriod;

        public class Dummy : MonoBehaviour { } // to start coroutine from outside monobehaviour
        private Dummy coroutineRunner;
        private GameObject runnerObject = new GameObject("RunnerObject");
        private NewRatCharacterController.NewCharacterInput input;
        private const float RAT_HEIGHT = 0.2f;

        /// <summary>
        /// Checks whether the desired time period (if the player has initiated time travelled) has changed and performs an 
        /// overlap collision check at the player's position with objects on the physics layer corresponding to the desired period. 
        /// 4D collision, if you will
        /// </summary>
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
                        TimeTravelManager.playerTransform.position.y + RAT_HEIGHT,
                        TimeTravelManager.playerTransform.position.z),
                    new Vector3(TimeTravelManager.playerTransform.position.x,
                        TimeTravelManager.playerTransform.position.y + RAT_HEIGHT / 2,
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

        /// <summary>
        /// performs the actual time travel if the collision check in Run() has passed, disables further time travel while the vfx
        /// for it is active.
        /// </summary>
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