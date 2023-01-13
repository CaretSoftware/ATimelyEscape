using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CallbackSystem;
using StateMachines;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// @author Emil Wessman
/// </summary>
public class TimeTravelObject : MonoBehaviour {
    [HideInInspector] public TimeTravelObject pastSelf;
    private Transform destiny;
    private TimeTravelObjectManager manager;
    public Rigidbody Rigidbody { get; private set; }
    private StateMachine stateMachine;
    public GameObject previewBoxObject { get; set; }
    public WireBox wireBox { get; set; }
    public bool IsActive { get; private set; }
    public TimeTravelPeriod timeTravelPeriod { get; set; }
    public List<string> RendererIDs { get; private set; }
    private List<Component> allComponents;

    /// <summary>
    /// To be used instead of unity's start. Every TimeTravelObjectManager is responsible for initiating each child TimeTravelObject
    /// Will run differently depending on settings passde to the manager. This is to require no setup of individual time travel objects, rather leaving it to
    /// just the manager.
    /// </summary>
    /// <param name="manager">The parent TimeTravelObjectManager</param>
    /// <param name="pastSelf">The TimeTravelObject representing the past version of this object, can be null</param>
    public void SetUpTimeTravelObject(TimeTravelObjectManager manager, TimeTravelObject pastSelf = null) {
        this.manager = manager;
        allComponents = GetComponents<Component>().ToList();
        allComponents.AddRange(GetComponentsInChildren<Component>());
        RendererIDs = new List<string>();

        switch (manager.ObjectState) {
            case TimeTravelObjectState.PrefabChanging: break;
            case TimeTravelObjectState.PrefabChangingPlayerMove:
                this.pastSelf = pastSelf;
                Rigidbody = GetComponent<Rigidbody>();
                stateMachine = new StateMachine(this, new State[] { new TimeTravelIdleState(), new TimeTravelMovingState() });

                if (pastSelf != null) {
                    var destinyObject = new GameObject(name + "Destiny") {
                        transform = { position = transform.position, parent = transform.parent }
                    };
                    destiny = destinyObject.transform;
                } else destiny = transform;

                if (Rigidbody == null) throw new MissingComponentException($"Movable {nameof(TimeTravelObject)}s require a {nameof(Rigidbody)} component!");

                TimeTravelManager.MovableObjects.Add(Rigidbody);
                if (Application.isPlaying) DestinyChanged.AddListener<DestinyChanged>(OnDestinyChanged);
                break;

            case TimeTravelObjectState.Dummy: break;
            default: throw new ArgumentOutOfRangeException();
        }
    }

    private void Update() { if (stateMachine != null) stateMachine.Run(); }

    /// <summary>
    /// Method to be used by TimeTravelObjectManagers instead of gameObject.SetActive(). This is because many systems in the game rely
    /// on other components on the gameObject being active, such as the colliders or NavMeshAgents
    /// </summary>
    /// <param name="active">Whether the TimeTravelObject should be active or not</param>
    public void SetActive(bool active) {
        if (manager.CanCollideOnTimeTravel) {
            if (!gameObject.activeSelf) gameObject.SetActive(true);
            foreach (var component in allComponents) {
                if (component != null) {
                    if (!component.gameObject.activeSelf) component.gameObject.SetActive(true);
                    Type type = component.GetType();
                    if (type.IsSubclassOf(typeof(Behaviour)) && type != typeof(TimeTravelObject) && type != typeof(NavMeshAgent) && type != typeof(NavMeshAgentHandler))
                        ((Behaviour)component).enabled = active;
                }
            }

            UpdateColliderLayers(transform, active);
        } else gameObject.SetActive(active);

        IsActive = active;
    }

    /// <summary>
    /// Recursive method to update the physics layer of the TimeTravelObject itself and all of (if any) its children. This is to
    /// Make collision work between different time periods. TimeTravelObjects not currently active are set to the layer representing the period they belong in
    /// </summary>
    /// <param name="transformToUpdate">The transform of the currently considered child of the TTO, should only be invoked with the transform on the actual TTO, not children</param>
    /// <param name="active">whether the TTO is active or not</param>
    private void UpdateColliderLayers(Transform transformToUpdate, bool active) {
        string timePeriodLayerName = "";

        switch (timeTravelPeriod) {
            case TimeTravelPeriod.Past: timePeriodLayerName = "PastTimePeriod"; break;
            case TimeTravelPeriod.Present: timePeriodLayerName = "PresentTimePeriod"; break;
            case TimeTravelPeriod.Future: timePeriodLayerName = "FutureTimePeriod"; break;
        }

        transformToUpdate.gameObject.layer = LayerMask.NameToLayer(active
            ? (manager.ObjectState == TimeTravelObjectState.PrefabChangingPlayerMove ? "Cube" : "Default")
            : timePeriodLayerName);
        for (int i = 0; i < transformToUpdate.childCount; i++) {
            transformToUpdate.GetChild(i).gameObject.layer = LayerMask.NameToLayer(active
                ? (manager.ObjectState == TimeTravelObjectState.PrefabChangingPlayerMove ? "Cube" : "Default")
                : timePeriodLayerName);
            UpdateColliderLayers(transformToUpdate.GetChild(i), active);
        }
    }

    /// <summary>
    /// Callback function to handle when a movable TimeTravelObject's destiny has been changed. This happens when past versions
    /// of the object are moved. The object is then moved to the same position to avoid time travel paradoxes
    /// </summary>
    /// <param name="e"></param>
    private void OnDestinyChanged(DestinyChanged e) {
        if (e.changedObject == pastSelf ||
            (pastSelf != null && pastSelf.pastSelf != null && e.changedObject == pastSelf.pastSelf)) {
            // Destiny has been changed

            destiny.position = e.changedObject.transform.position;
            destiny.rotation = e.changedObject.transform.rotation;

            transform.position = destiny.position;
            transform.rotation = destiny.rotation;
        }
    }

    private void OnDestroy() {
        if (manager != null && this != null && RendererIDs != null) foreach (var ID in RendererIDs) manager.RemoveRendererInfo(ID);
        if (EventSystem.Current != null) DestinyChanged.RemoveListener<DestinyChanged>(OnDestinyChanged);
    }
}

namespace StateMachines {
    /// <summary>
    /// Exists so that the destiny of a TimeTravelObject is only updated when the object is stopped or close to stopping. 
    /// </summary>
    public class TimeTravelMovingState : State {
        private TimeTravelObject TravelObject => (TimeTravelObject)Owner;

        public override void Run() {
            if (TravelObject.Rigidbody != null && TravelObject.Rigidbody.velocity.magnitude < 0.1f) StateMachine.TransitionTo<TimeTravelIdleState>();
        }

        public override void Exit() {
            var destinyChangedEvent = new DestinyChanged() { changedObject = TravelObject, gameObject = TravelObject.gameObject };
            destinyChangedEvent.Invoke();
        }
    }

    public class TimeTravelIdleState : State {
        private TimeTravelObject TravelObject => (TimeTravelObject)Owner;

        public override void Run() {
            if (TravelObject.Rigidbody != null && TravelObject.Rigidbody.velocity.magnitude > 0.1f) StateMachine.TransitionTo<TimeTravelMovingState>();
        }
    }
}