using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CallbackSystem;
using NaughtyAttributes;

public class TimeTravelObjectManager : MonoBehaviour {
    [Tooltip("Whether the time travelling object uses different meshes in different time periods")]
    [DisableIf(EConditionOperator.Or, nameof(isDecal), nameof(changesMaterials))]
    [OnValueChanged(nameof(OnCheckChangesMesh))]
    [SerializeField]
    private bool changesMesh;

    [Tooltip("Whether the object changes materials upon travelling between different time periods. " +
             "ATTENTION: It's very important that the renderers are added in the arrays in the SAME ORDER as the materials otherwise it will not work properly." +
             " NOTE: Materials specific for each period have to be already applied to the meshes in question")]
    [DisableIf(EConditionOperator.Or, nameof(canBeMovedByPlayer), nameof(changesPosition), nameof(changesMesh))]
    [OnValueChanged(nameof(OnCheckMaterialsChange))]
    [SerializeField]
    private bool changesMaterials;

    [Tooltip(
        "If the object changes position between time periods, ATTENTION: not the same as \"can be moved by player.\" " +
        "Objects with this checked cannot be moved by the player, but can have different positions set in the unity editor for the different time periods")]
    [DisableIf(EConditionOperator.Or, nameof(canBeMovedByPlayer), nameof(changesMaterials),
        nameof(IsNotChangingMeshDecal))]
    [OnValueChanged(nameof(OnCheckChangesPosition))]
    [SerializeField]
    private bool changesPosition;

    private bool IsNotChangingMeshDecal => !changesMesh && !isDecal;
    private bool IsNotChangingMesh => !changesMesh;

    [Tooltip(
        "Objects that can be moved by player must have different meshes/instances for each time period it exists in. Any time period it does not exist in can be left as null")]
    [DisableIf(EConditionOperator.Or, nameof(changesPosition), nameof(isDecal), nameof(IsNotChangingMesh))]
    [OnValueChanged(nameof(OnCheckPlayerCanMove))]
    [SerializeField]
    private bool canBeMovedByPlayer;

    [Tooltip("Whether the time travelling object is a decal")]
    [DisableIf(EConditionOperator.Or, nameof(canBeMovedByPlayer), nameof(changesMesh))]
    [OnValueChanged(nameof(OnCheckIsDecal))]
    [SerializeField]
    private bool isDecal;

    [ShowIf(nameof(changesMaterials))]
    [AllowNesting]
    [SerializeField]
    private MaterialInfo[] pastMaterials;


    [ShowIf(nameof(changesMaterials))]
    [AllowNesting]
    [SerializeField]
    private MaterialInfo[] presentMaterials;


    [ShowIf(nameof(changesMaterials))]
    [AllowNesting]
    [SerializeField]
    private MaterialInfo[] futureMaterials;

    [ShowIf(nameof(changesMaterials))]
    [SerializeField]
    private Renderer[] renderers;


    [ShowIf(EConditionOperator.Or, nameof(changesMesh), nameof(canBeMovedByPlayer), nameof(changesPosition))]
    [SerializeField]
    private TimeTravelObject past, present, future;

    [HideIf(EConditionOperator.Or, nameof(changesMesh), nameof(canBeMovedByPlayer), nameof(changesPosition))]
    [SerializeField]
    private TimeTravelObject timeTravelObject;

    public TimeTravelObjectState ObjectState { get; private set; }

    public bool CanBeMovedByPlayer => canBeMovedByPlayer;
    public MaterialInfo[] PastMaterials => pastMaterials;
    public MaterialInfo[] PresentMaterials => presentMaterials;
    public MaterialInfo[] FutureMaterials => futureMaterials;
    public Renderer[] Renderers => renderers;


    private void Awake() {
        CheckForMissingComponents();
        DetermineTimeTravelObjectState();

        if (changesMesh || changesPosition || canBeMovedByPlayer) {
            past.SetUpTimeTravelObject(this);
            present.SetUpTimeTravelObject(this, past);
            future.SetUpTimeTravelObject(this, present);
        } else timeTravelObject.SetUpTimeTravelObject(this);

        TimePeriodChanged.AddListener<TimePeriodChanged>(OnTimePeriodChanged);
        PhysicsSimulationComplete.AddListener<PhysicsSimulationComplete>(OnPhysicsSimulationComplete);
    }

    private void CheckForMissingComponents() {
        string missingComponentNames = "";
        for (int i = 0; i < transform.childCount; i++) {
            if (transform.GetChild(i).GetComponent<TimeTravelObject>() == null &&
                !transform.GetChild(i).name.Contains("Destiny")) {
                if (i == transform.childCount - 1 || transform.childCount == 1)
                    missingComponentNames += transform.GetChild(i).name;
                else missingComponentNames += transform.GetChild(i).name + ", ";
            }
        }

        if (missingComponentNames.Length > 0) {
            throw new MissingComponentException(
                $"Every child of a {nameof(TimeTravelObjectManager)} needs a {nameof(TimeTravelObject)} component! The following GameOject(s) have none: {missingComponentNames}");
        }
    }

    private void DetermineTimeTravelObjectState() {
        /*ObjectState = changesMesh switch {
            true when !canBeMovedByPlayer && !changesPosition => TimeTravelObjectState.MeshChanging,
            true when !canBeMovedByPlayer && changesPosition => TimeTravelObjectState.MeshChangingMoving,
            true when canBeMovedByPlayer && !changesPosition => TimeTravelObjectState.MeshChangingPlayerMove,
            _ => changesMaterials switch {
                true when !isDecal => TimeTravelObjectState.MeshSwitchingMaterial,
                true when isDecal => TimeTravelObjectState.DecalSwitchingMaterial,
                _ => isDecal switch {
                    true when !changesMaterials && !changesPosition => TimeTravelObjectState.Decal,
                    true when changesPosition && !changesMaterials => TimeTravelObjectState.DecalMoving,
                    _ => ObjectState
                }
            }
        };*/

        switch (changesMesh) {
            case true when !canBeMovedByPlayer && !changesPosition:
                ObjectState = TimeTravelObjectState.MeshChanging;
                return;
            case true when canBeMovedByPlayer && !changesPosition:
                ObjectState = TimeTravelObjectState.MeshChangingPlayerMove;
                return;
            case true when !canBeMovedByPlayer && changesPosition:
                ObjectState = TimeTravelObjectState.MeshChangingMoving;
                return;
        }

        switch (isDecal) {
            case false when changesMaterials:
                ObjectState = TimeTravelObjectState.MeshSwitchingMaterial;
                return;
            case true when changesMaterials && !changesPosition:
                ObjectState = TimeTravelObjectState.DecalSwitchingMaterial;
                return;
            case true when !changesMaterials && changesPosition:
                ObjectState = TimeTravelObjectState.DecalMoving;
                return;
            case true when !changesMaterials && !changesPosition:
                ObjectState = TimeTravelObjectState.Decal;
                break;
        }
    }

    private void SetUpPreviewBox(TimeTravelObject travelObject, Color color) {
        GameObject wireObj = new GameObject(travelObject.name + " preview");
        wireObj.transform.parent = transform;
        travelObject.previewBoxObject = wireObj;
        travelObject.wireBox = travelObject.previewBoxObject.AddComponent<WireBox>();
        travelObject.wireBox.Color = color;
    }

    private void Update() { UpdateWireBoxes(); }

    private void UpdateWireBoxes() {
        if (past == null || present == null || future == null || past.wireBox == null || present.wireBox == null ||
            future.wireBox == null) return;

        var activePos = Vector3.zero;
        if (past.gameObject.activeSelf) activePos = past.transform.position;
        else if (present.gameObject.activeSelf) activePos = present.transform.position;
        else activePos = future.transform.position;

        if (!past.gameObject.activeSelf && Vector3.Distance(past.transform.position, activePos) > 1f &&
            ObjectState == TimeTravelObjectState.MeshChangingPlayerMove) {
            past.wireBox.SetLinePositions();
            past.previewBoxObject.SetActive(true);
            past.previewBoxObject.transform.position = past.transform.position;
            past.wireBox.Color = Color.white;
        } else past.previewBoxObject?.SetActive(false);

        if (!present.gameObject.activeSelf && Vector3.Distance(present.transform.position, activePos) > 1f &&
            ObjectState == TimeTravelObjectState.MeshChangingPlayerMove) {
            present.wireBox?.SetLinePositions();
            present.previewBoxObject.SetActive(true);
            present.previewBoxObject.transform.position = present.transform.position;
            present.wireBox.Color = Color.gray;
        } else present.previewBoxObject?.SetActive(false);

        if (!future.gameObject.activeSelf && Vector3.Distance(future.transform.position, activePos) > 1f &&
            ObjectState == TimeTravelObjectState.MeshChangingPlayerMove) {
            future.wireBox.SetLinePositions();
            future.previewBoxObject.SetActive(true);
            future.previewBoxObject.transform.position = future.transform.position;
            future.wireBox.Color = Color.blue;
        } else future.previewBoxObject?.SetActive(false);
    }

    // callbacks
    private void OnTimePeriodChanged(TimePeriodChanged e) {
        switch (ObjectState) {
            case TimeTravelObjectState.Decal: break;
            case TimeTravelObjectState.DecalMoving:
            case TimeTravelObjectState.MeshChanging:
            case TimeTravelObjectState.MeshChangingMoving:
            case TimeTravelObjectState.MeshChangingPlayerMove:
                if (past.wireBox == null && ObjectState == TimeTravelObjectState.MeshChangingPlayerMove)
                    SetUpPreviewBox(past, Color.white);
                if (present.wireBox == null && ObjectState == TimeTravelObjectState.MeshChangingPlayerMove)
                    SetUpPreviewBox(present, Color.gray);
                if (future.wireBox == null && ObjectState == TimeTravelObjectState.MeshChangingPlayerMove)
                    SetUpPreviewBox(future, Color.blue);

                past.gameObject.SetActive(e.to == TimeTravelPeriod.Past ? true : false);
                present.gameObject.SetActive(e.to == TimeTravelPeriod.Present ? true : false);
                future.gameObject.SetActive(e.to == TimeTravelPeriod.Future ? true : false);


                break;
            case TimeTravelObjectState.DecalSwitchingMaterial:
            case TimeTravelObjectState.MeshSwitchingMaterial:
                timeTravelObject.UpdateMaterials(e.to);
                break;
            case TimeTravelObjectState.Dummy: break;
        }
    }

    private void OnPhysicsSimulationComplete(PhysicsSimulationComplete e) {
        if (ObjectState == TimeTravelObjectState.MeshChangingPlayerMove) {
            if (past.Rigidbody != null) {
                past.Rigidbody.isKinematic = e.to is not TimeTravelPeriod.Past;
            }

            if (present.Rigidbody != null)
                present.Rigidbody.isKinematic = e.from is TimeTravelPeriod.Future && e.to is TimeTravelPeriod.Present;
        }
    }

    private void OnCheckPlayerCanMove() {
        if (!canBeMovedByPlayer) return;
        isDecal = false;
        changesPosition = false;
        changesMaterials = false;
    }

    private void OnCheckChangesPosition() {
        if (changesPosition) canBeMovedByPlayer = false;
    }

    private void OnCheckIsDecal() {
        if (isDecal) canBeMovedByPlayer = false;
    }

    private void OnCheckMaterialsChange() {
        if (changesMaterials) changesMesh = false;
    }

    private void OnCheckChangesMesh() {
        if (changesMesh) return;
        changesPosition = false;
        canBeMovedByPlayer = false;
    }

    [Serializable]
    public class MaterialInfo {
        public Material[] materials;
    }
}

public enum TimeTravelObjectState {
    Decal = 0,
    DecalMoving = 1,
    DecalSwitchingMaterial = 2,

    MeshChanging = 3,
    MeshChangingMoving = 4,
    MeshChangingPlayerMove = 5,

    MeshSwitchingMaterial = 6,
    Dummy = 7
}