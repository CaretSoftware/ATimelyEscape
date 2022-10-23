using System;
using System.Collections;
using System.Collections.Generic;
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

    public void SetUpTimeTravelObject(TimeTravelObjectManager manager, TimeTravelObject pastSelf = null) {
        this.manager = manager;
        stateMachine = new StateMachine(this, new State[] { new TimeTravelIdleState(), new TimeTravelMovingState() });
        this.pastSelf = pastSelf;
        Rigidbody = GetComponent<Rigidbody>();
        if (pastSelf != null) {
            var destinyObject = new GameObject(name + "Destiny");
            destinyObject.transform.position = transform.position;
            destinyObject.transform.parent = transform.parent;
            destiny = destinyObject.transform;
        } else destiny = transform;

        if (manager.CanBeMoved && Rigidbody == null)
            throw new MissingComponentException("Movable time travel objects require a rigidbody component!");

        if (pastSelf == null) return;
        DestinyChanged.AddListener<DestinyChanged>(OnDestinyChanged);
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