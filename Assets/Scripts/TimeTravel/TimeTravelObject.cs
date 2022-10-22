using System;
using System.Collections;
using System.Collections.Generic;
using CallbackSystem;
using UnityEngine;
using StateMachines;
using UnityEngine.Rendering.Universal;

public class TimeTravelObject : MonoBehaviour {
    private TimeTravelObject pastSelf;
    private Transform destiny;
    private TimeTravelObjectManager manager;
    public Rigidbody Rigidbody { get; private set; }
    private StateMachine stateMachine;
    public State CurrentState => stateMachine.CurrentState;
    private TimeTravelObjectState timeObjectState;
    private Renderer mRenderer;
    private DecalProjector decalProjector;


    public void SetUpTimeTravelObject(TimeTravelObjectManager manager, TimeTravelObject pastSelf = null) {
        this.manager = manager;
        stateMachine = new StateMachine(this, new State[] { new TimeTravelIdleState(), new TimeTravelMovingState() });

        switch (manager.ObjectState) {
            case TimeTravelObjectState.Decal:
            case TimeTravelObjectState.DecalMoving:
            case TimeTravelObjectState.DecalSwitchingMaterial:
                decalProjector = GetComponent<DecalProjector>();
                if (!decalProjector)
                    throw new MissingComponentException(
                        $"Decal {nameof(TimeTravelObject)}s require a {nameof(DecalProjector)} component!");
                break;
            case TimeTravelObjectState.MeshChanging: break;
            case TimeTravelObjectState.MeshChangingMoving: break;

            case TimeTravelObjectState.MeshChangingPlayerMove:
                this.pastSelf = pastSelf;
                Rigidbody = GetComponent<Rigidbody>();

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

                DestinyChanged.AddListener<DestinyChanged>(OnDestinyChanged);
                break;

            case TimeTravelObjectState.MeshSwitchingMaterial:
                mRenderer = GetComponent<Renderer>();
                if (!mRenderer)
                    throw new MissingComponentException(
                        $"{nameof(TimeTravelObject)} switching materials require a {nameof(Renderer)} component!");
                break;

            case TimeTravelObjectState.Dummy: break;
            default: throw new ArgumentOutOfRangeException();
        }
    }

    public void UpdateMaterials(TimeTravelPeriod period) {
        switch (manager.ObjectState) {
            case TimeTravelObjectState.DecalSwitchingMaterial:
                decalProjector.material = period switch {
                    TimeTravelPeriod.Past => manager.PastMaterials[0],
                    TimeTravelPeriod.Present => manager.PresentMaterials[0],
                    TimeTravelPeriod.Future => manager.FutureMaterials[0],
                    _ => decalProjector.material
                };
                break;
            case TimeTravelObjectState.MeshSwitchingMaterial:
                mRenderer.materials = period switch {
                    TimeTravelPeriod.Past => manager.PastMaterials,
                    TimeTravelPeriod.Present => manager.PresentMaterials,
                    TimeTravelPeriod.Future => manager.FutureMaterials,
                    _ => mRenderer.materials
                };
                break;
        }
    }

    private void Update() {
        stateMachine.Run();
        if (Input.GetKey(KeyCode.A)) Rigidbody.AddForce(Vector3.left * 10f, ForceMode.Force);
        if (Input.GetKey(KeyCode.D)) Rigidbody.AddForce(Vector3.right * 10f, ForceMode.Force);
        if (Input.GetKey(KeyCode.W)) Rigidbody.AddForce(Vector3.forward * 10f, ForceMode.Force);
        if (Input.GetKey(KeyCode.S)) Rigidbody.AddForce(Vector3.back * 10f, ForceMode.Force);
    }

    private void OnDestinyChanged(DestinyChanged e) {
        if (e.changedObject != pastSelf && e.changedObject != pastSelf.pastSelf) return;
        print($"Destiny of {name} has been changed!");

        destiny.position = e.changedObject.transform.position;
        destiny.rotation = e.changedObject.transform.rotation;

        transform.position = destiny.position;
        transform.rotation = destiny.rotation;
    }

    private void OnDrawGizmos() {
        if (pastSelf != null) {
            Gizmos.DrawWireCube(destiny.position, Vector3.one);
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