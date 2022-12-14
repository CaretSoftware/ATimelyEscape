using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CallbackSystem;
using StateMachines;
using UnityEngine;
using UnityEngine.AI;

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
    private List<Component> allComponents;

    public void SetUpTimeTravelObject(TimeTravelObjectManager manager, TimeTravelObject pastSelf = null) {
        this.manager = manager;
        allComponents = GetComponents<Component>().ToList();
        allComponents.AddRange(GetComponentsInChildren<Component>());

        switch (manager.ObjectState) {
            case TimeTravelObjectState.PrefabChanging: break;
            case TimeTravelObjectState.PrefabChangingPlayerMove:
                this.pastSelf = pastSelf;
                Rigidbody = GetComponent<Rigidbody>();
                stateMachine = new StateMachine(this,
                    new State[] { new TimeTravelIdleState(), new TimeTravelMovingState() });

                if (pastSelf != null) {
                    var destinyObject = new GameObject(name + "Destiny") {
                        transform = { position = transform.position, parent = transform.parent }
                    };
                    destiny = destinyObject.transform;
                } else destiny = transform;

                if (Rigidbody == null) {
                    throw new MissingComponentException(
                        $"Movable {nameof(TimeTravelObject)}s require a {nameof(Rigidbody)} component!");
                }

                TimeTravelManager.MovableObjects.Add(Rigidbody);
                if (Application.isPlaying) DestinyChanged.AddListener<DestinyChanged>(OnDestinyChanged);
                break;

            case TimeTravelObjectState.Dummy: break;
            default: throw new ArgumentOutOfRangeException();
        }
    }

    private void Update() { if (stateMachine != null) stateMachine.Run(); }

    public void SetActive(bool active) {
        if (manager.CanCollideOnTimeTravel) {
            if (!gameObject.activeSelf) gameObject.SetActive(true);
            foreach (var component in allComponents) {
                if (component != null) {
                    if (!component.gameObject.activeSelf) component.gameObject.SetActive(true);
                    Type type = component.GetType();
                    if (type.IsSubclassOf(typeof(Behaviour)) && type != typeof(TimeTravelObject) &&
                        type != typeof(NavMeshAgent) && type != typeof(NavMeshAgentHandler))
                        ((Behaviour)component).enabled = active;
                }
            }

            UpdateColliderLayers(transform, active);
        } else gameObject.SetActive(active);

        IsActive = active;
    }

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

    private void OnDestinyChanged(DestinyChanged e) {
        if (e.changedObject == pastSelf ||
            (pastSelf != null && pastSelf.pastSelf != null && e.changedObject == pastSelf.pastSelf)) {
            print($"Destiny of {name} has been changed!");

            destiny.position = e.changedObject.transform.position;
            destiny.rotation = e.changedObject.transform.rotation;

            transform.position = destiny.position;
            transform.rotation = destiny.rotation;
        }
    }
}

namespace StateMachines {
    public class TimeTravelMovingState : State {
        private TimeTravelObject TravelObject => (TimeTravelObject)Owner;

        public override void Run() {
            if (TravelObject.Rigidbody != null && TravelObject.Rigidbody.velocity.magnitude < 0.1f)
                StateMachine.TransitionTo<TimeTravelIdleState>();
        }

        public override void Exit() {
            var destinyChangedEvent =
                new DestinyChanged() { changedObject = TravelObject, gameObject = TravelObject.gameObject };
            destinyChangedEvent.Invoke();
        }
    }

    public class TimeTravelIdleState : State {
        private TimeTravelObject TravelObject => (TimeTravelObject)Owner;

        public override void Run() {
            if (TravelObject.Rigidbody != null && TravelObject.Rigidbody.velocity.magnitude > 0.1f) {
                StateMachine.TransitionTo<TimeTravelMovingState>();
            }
        }
    }
}