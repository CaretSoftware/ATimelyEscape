using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CallbackSystem;
using UnityEngine;
using StateMachines;
using UnityEngine.AI;

public class TimeTravelObject : MonoBehaviour {
    [HideInInspector] public TimeTravelObject pastSelf;
    private Transform destiny;
    private TimeTravelObjectManager manager;
    public Rigidbody Rigidbody { get; private set; }
    private StateMachine stateMachine;
    private List<Renderer> renderers;
    private List<Renderer> renderers2 = new List<Renderer>();

    private int numColliders = 0;
    public GameObject previewBoxObject { get; set; }
    public WireBox wireBox { get; set; }
    public bool IsActive { get; private set; }

    private List<Component> allComponents;

    private List<GameObject> nonMovableColliderObjects = new List<GameObject>();
    private GameObject nonMovableCollidersParent;


    public void SetUpTimeTravelObject(TimeTravelObjectManager manager, TimeTravelObject pastSelf = null) {
        this.manager = manager;
        allComponents = GetComponents<Component>().ToList();
        allComponents.AddRange(GetComponentsInChildren<Component>());
        /*GatherRenderers(transform);*/

        switch (manager.ObjectState) {
            case TimeTravelObjectState.PrefabChanging:
                break;
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
                DestinyChanged.AddListener<DestinyChanged>(OnDestinyChanged);
                break;

            case TimeTravelObjectState.SwitchingMaterial:
                renderers = manager.Renderers.ToList();
                CheckRenderersAndMaterialsMatch();
                break;

            case TimeTravelObjectState.Dummy: break;
            default: throw new ArgumentOutOfRangeException();
        }
    }

    public void GatherRenderers(Transform currentTransform) {
        Renderer temp;
        for (int i = 0; i < currentTransform.childCount; i++) {
            temp = GetComponent<Renderer>();
            if (temp != null) {
                renderers2.Add(temp);
                GatherRenderers(temp.transform);
            }
        }
    }

    private void CheckRenderersAndMaterialsMatch() {
        string errorMessage = "";
        if (manager.PastMaterials.Length != renderers.Count) errorMessage += $"[{nameof(manager.PastMaterials)}";
        if (manager.PresentMaterials.Length != renderers.Count) {
            if (!errorMessage.Contains(nameof(manager.PastMaterials)))
                errorMessage += $"[{nameof(manager.PresentMaterials)}";
            else errorMessage += $", {nameof(manager.PresentMaterials)}";
        }

        if (manager.FutureMaterials.Length != renderers.Count) {
            if (!errorMessage.Contains(nameof(manager.PastMaterials)) &&
                !errorMessage.Contains(nameof(manager.PresentMaterials)))
                errorMessage = $"[{nameof(manager.FutureMaterials)}";
            else errorMessage += $", {nameof(manager.FutureMaterials)}";
        }

        if (errorMessage.Length > 1) {
            errorMessage +=
                $"] material array(s) do not match the length of the renderer array on {nameof(TimeTravelObjectManager)}: {manager.name}";
            /*Debug.LogWarning("CHRISTOFFER THE ERROR IS HERE!", manager.gameObject);*/
            throw new ArgumentException(errorMessage);
        }
    }

    public void UpdateMaterials(TimeTravelPeriod period) {
        switch (period) {
            case TimeTravelPeriod.Past:
                for (int i = 0; i < renderers.Count; i++) renderers[i].materials = manager.PastMaterials[i].materials;
                break;
            case TimeTravelPeriod.Present:
                for (int i = 0; i < renderers.Count; i++)
                    renderers[i].materials = manager.PresentMaterials[i].materials;
                break;
            case TimeTravelPeriod.Future:
                for (int i = 0; i < renderers.Count; i++)
                    renderers[i].materials = manager.FutureMaterials[i].materials;
                break;
        }
    }

    public void SetActive(bool active) {
        if (manager.CanCollideOnTimeTravel) {
            if (!gameObject.activeSelf) gameObject.SetActive(true);
            foreach (var c in allComponents) {
                if (c != null) {
                    if (!c.gameObject.activeSelf) c.gameObject.SetActive(true);
                    Type t = c.GetType();
                    if (t.IsSubclassOf(typeof(Behaviour)) && t != typeof(TimeTravelObject) &&
                        t != typeof(NavMeshAgent) && t != typeof(NavMeshAgentHandler))
                        ((Behaviour)c).enabled = active;
                    else if (t.IsSubclassOf(typeof(Renderer))) ((Renderer)c).enabled = active;
                }
            }

            UpdateColliderLayers(transform, active);
        } else gameObject.SetActive(active);

        IsActive = active;
    }

    private void UpdateColliderLayers(Transform transformToUpdate, bool active) {
        transformToUpdate.gameObject.layer = LayerMask.NameToLayer(active
            ? (manager.ObjectState == TimeTravelObjectState.PrefabChangingPlayerMove ? "Cube" : "Default")
            : "OtherTimePeriod");
        for (int i = 0; i < transformToUpdate.childCount; i++) {
            transformToUpdate.GetChild(i).gameObject.layer = LayerMask.NameToLayer(active
                ? (manager.ObjectState == TimeTravelObjectState.PrefabChangingPlayerMove ? "Cube" : "Default")
                : "OtherTimePeriod");
            UpdateColliderLayers(transformToUpdate.GetChild(i), active);
        }
    }

    private void Update() {
        if (manager.ObjectState == TimeTravelObjectState.PrefabChangingPlayerMove && IsActive) {
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