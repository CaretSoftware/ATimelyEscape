using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CallbackSystem;
using UnityEngine;
using StateMachines;

public class TimeTravelObject : MonoBehaviour {
    private TimeTravelObject pastSelf;
    private Transform destiny;
    private TimeTravelObjectManager manager;
    public Rigidbody Rigidbody { get; private set; }
    private StateMachine stateMachine;
    public State CurrentState => stateMachine.CurrentState;
    private Renderer mRenderer;
    private Renderer[] renderers;
    public GameObject previewBoxObject { get; set; }
    public WireBox wireBox { get; set; }


    public void SetUpTimeTravelObject(TimeTravelObjectManager manager, TimeTravelObject pastSelf = null) {
        this.manager = manager;

        switch (manager.ObjectState) {
            case TimeTravelObjectState.MeshChanging: break;
            case TimeTravelObjectState.MeshChangingMoving: break;
            case TimeTravelObjectState.DecalMoving: break;

            case TimeTravelObjectState.MeshChangingPlayerMove:
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
                DestinyChanged.AddListener<DestinyChanged>(OnDestinyChanged);
                break;

            case TimeTravelObjectState.Decal:
            case TimeTravelObjectState.DecalSwitchingMaterial:
            case TimeTravelObjectState.MeshSwitchingMaterial:
                renderers = manager.Renderers;
                CheckRenderersAndMaterialsMatch();
                break;

            case TimeTravelObjectState.Dummy: break;
            default: throw new ArgumentOutOfRangeException();
        }
    }

    private void CheckRenderersAndMaterialsMatch() {
        string errorMessage = "";
        if (manager.PastMaterials.Length != renderers.Length) errorMessage += $"[{nameof(manager.PastMaterials)}";
        if (manager.PresentMaterials.Length != renderers.Length) {
            if (!errorMessage.Contains(nameof(manager.PastMaterials)))
                errorMessage += $"[{nameof(manager.PresentMaterials)}";
            else errorMessage += $", {nameof(manager.PresentMaterials)}";
        }

        if (manager.FutureMaterials.Length != renderers.Length) {
            if (!errorMessage.Contains(nameof(manager.PastMaterials)) &&
                !errorMessage.Contains(nameof(manager.PresentMaterials)))
                errorMessage = $"[{nameof(manager.FutureMaterials)}";
            else errorMessage += $", {nameof(manager.FutureMaterials)}";
        }

        if (errorMessage.Length > 1) {
            errorMessage +=
                $"] material array(s) do not match the length of the renderer array on {nameof(TimeTravelObjectManager)}: {manager.name}";
            Debug.LogWarning("CHRISTOFFER THE ERROR IS HERE!", manager.gameObject);
            throw new ArgumentException(errorMessage);
        }
    }

    public void UpdateMaterials(TimeTravelPeriod period) {
        switch (period) {
            case TimeTravelPeriod.Past:
                for (int i = 0; i < renderers.Length; i++) renderers[i].materials = manager.PastMaterials[i].materials;
                break;
            case TimeTravelPeriod.Present:
                for (int i = 0; i < renderers.Length; i++)
                    renderers[i].materials = manager.PresentMaterials[i].materials;
                break;
            case TimeTravelPeriod.Future:
                for (int i = 0; i < renderers.Length; i++)
                    renderers[i].materials = manager.FutureMaterials[i].materials;
                break;
        }
    }

    private void Update() {
        if (manager.ObjectState == TimeTravelObjectState.MeshChangingPlayerMove) {
            stateMachine.Run();
            if (Input.GetKey(KeyCode.A)) Rigidbody.AddForce(Vector3.left * 10f, ForceMode.Force);
            if (Input.GetKey(KeyCode.D)) Rigidbody.AddForce(Vector3.right * 10f, ForceMode.Force);
            if (Input.GetKey(KeyCode.W)) Rigidbody.AddForce(Vector3.forward * 10f, ForceMode.Force);
            if (Input.GetKey(KeyCode.S)) Rigidbody.AddForce(Vector3.back * 10f, ForceMode.Force);
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

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        if (pastSelf != null) {
            Gizmos.DrawWireCube(destiny.position, Vector3.one);
        }
    }
#endif
}

namespace StateMachines {
    public class TimeTravelMovingState : State {
        private TimeTravelObject TravelObject => (TimeTravelObject)Owner;
        private bool listenerAdded;

        public override void Run() {
            if (!listenerAdded) {
                //DebugEvent.AddListener<DebugEvent>(OnTimeTravel);
                listenerAdded = true;
            }

            if (TravelObject.Rigidbody != null && TravelObject.Rigidbody.velocity.magnitude < 0.1f)
                StateMachine.TransitionTo<TimeTravelIdleState>();
        }

        public override void Exit() {
            var destinyChangedEvent =
                new DestinyChanged() { changedObject = TravelObject, gameObject = TravelObject.gameObject };
            destinyChangedEvent.Invoke();
        }

        private void OnTimeTravel(DebugEvent e) {
            if (e.DebugText == null || !e.DebugText.Contains("Simulation")) return;
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