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

    [Tooltip(
        "Whether the object changes materials upon travelling between different time periods. NOTE: Materials specific for each period have to be already applied to the meshes in question")]
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
    [SerializeField]
    private Material[] pastMaterials;

    [ShowIf(nameof(changesMaterials))]
    [SerializeField]
    private Material[] presentMaterials;

    [ShowIf(nameof(changesMaterials))]
    [SerializeField]
    private Material[] futureMaterials;


    [ShowIf(EConditionOperator.Or, nameof(changesMesh), nameof(canBeMovedByPlayer), nameof(changesPosition))]
    [SerializeField]
    private TimeTravelObject past, present, future;

    [HideIf(EConditionOperator.Or, nameof(changesMesh), nameof(canBeMovedByPlayer), nameof(changesPosition))]
    [SerializeField]
    private TimeTravelObject timeTravelObject;

    public TimeTravelObjectState ObjectState { get; private set; }

    public bool CanBeMovedByPlayer => canBeMovedByPlayer;
    public Material[] PastMaterials => pastMaterials;
    public Material[] PresentMaterials => presentMaterials;
    public Material[] FutureMaterials => futureMaterials;


    private void Awake() {
        if (changesMesh || changesPosition || canBeMovedByPlayer) {
            past.SetUpTimeTravelObject(this);
            present.SetUpTimeTravelObject(this, past);
            future.SetUpTimeTravelObject(this, present);
        } else timeTravelObject.SetUpTimeTravelObject(this);


        TimePeriodChanged.AddListener<TimePeriodChanged>(OnTimePeriodChanged);
        CheckForMissingComponents();
        DetermineTimeTravelObjectState();
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
        /*objectState = changesMesh switch {
            true when !canBeMovedByPlayer && !changesPosition => TimeTravelObjectState.MeshChanging,
            true when !canBeMovedByPlayer && changesPosition => TimeTravelObjectState.MeshChangingMoving,
            true when canBeMovedByPlayer && !changesPosition => TimeTravelObjectState.MeshChangingPlayerMove,
            _ => changesMaterials switch {
                true when !isDecal => TimeTravelObjectState.MeshSwitchingMaterial,
                true when isDecal => TimeTravelObjectState.DecalSwitchingMaterial,
                _ => isDecal switch {
                    true when !changesMaterials && !changesPosition => TimeTravelObjectState.Decal,
                    true when changesPosition && !changesMaterials => TimeTravelObjectState.DecalMoving,
                    _ => objectState
                }
            }
        };*/

        //this is ugly af, gonna try to do it better later
        if (changesMesh && !canBeMovedByPlayer && !changesPosition) ObjectState = TimeTravelObjectState.MeshChanging;
        if (changesMesh && !canBeMovedByPlayer && changesPosition)
            ObjectState = TimeTravelObjectState.MeshChangingMoving;
        if (changesMesh && canBeMovedByPlayer && !changesPosition)
            ObjectState = TimeTravelObjectState.MeshChangingPlayerMove;
        if (isDecal && !changesMaterials && !changesPosition) ObjectState = TimeTravelObjectState.Decal;
        if (isDecal && !changesMaterials && changesPosition) ObjectState = TimeTravelObjectState.DecalMoving;
        if (isDecal && changesMaterials && !changesPosition) ObjectState = TimeTravelObjectState.DecalSwitchingMaterial;
        if (!isDecal && changesMaterials) ObjectState = TimeTravelObjectState.MeshSwitchingMaterial;
    }

    // callbacks
    private void OnTimePeriodChanged(TimePeriodChanged e) {
        switch (ObjectState) {
            case TimeTravelObjectState.Decal: break;
            case TimeTravelObjectState.DecalMoving:
            case TimeTravelObjectState.MeshChanging:
            case TimeTravelObjectState.MeshChangingMoving:
            case TimeTravelObjectState.MeshChangingPlayerMove:
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