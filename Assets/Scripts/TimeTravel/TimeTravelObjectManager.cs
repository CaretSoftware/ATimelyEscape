using System;
using System.Collections.Generic;
using CallbackSystem;
using NaughtyAttributes;
using UnityEngine;

public class TimeTravelObjectManager : MonoBehaviour {
    [Tooltip(
        "Whether the time travelling object uses different prefabs in different time periods, this goes for meshes AND decals")]
    [OnValueChanged(nameof(OnCheckChangesMesh))]
    [SerializeField]
    private bool changesPrefab;

    private bool IsNotChangingPrefab => !changesPrefab;

    [Tooltip(
        "Objects that can be moved by player must have different meshes/instances for each time period it exists in. Any time period it does not exist in can be left as null")]
    [DisableIf(nameof(IsNotChangingPrefab))]
    [OnValueChanged(nameof(OnCheckPlayerCanMove))]
    [SerializeField]
    private bool canBeMovedByPlayer;

    [Tooltip("Whether or not to show a preview box outline where the other objects are in different time periods")]
    [ShowIf(nameof(canBeMovedByPlayer))]
    [SerializeField]
    private bool showPreviewBox;

    [Tooltip("How large the preview box should be")]
    [ShowIf(nameof(showPreviewBox))]
    [SerializeField]
    private float previewBoxScale = 1f;

    [Tooltip(
        "The minimum distance objects need to be from each other in different time periods for the preview box to show")]
    [ShowIf(nameof(showPreviewBox))]
    [SerializeField]
    private float previewBoxMinShowDistance = 1f;

    [OnValueChanged(nameof(OnCheckCanCollide))]
    [SerializeField]
    private bool canCollideOnTimeTravel;

    [SerializeField] private bool displaceOnTimeTravel = true;

    [ShowIf(EConditionOperator.Or, nameof(changesPrefab), nameof(canBeMovedByPlayer))]
    [SerializeField]
    private TimeTravelObject past, present, future;

    public TimeTravelObjectState ObjectState { get; private set; }

    private Vector3 activePosition;

    public TimeTravelObject Past { get => past; set => past = value; }
    public TimeTravelObject Present { get => present; set => present = value; }
    public TimeTravelObject Future { get => future; set => future = value; }

    public bool CanBeMovedByPlayer { get => canBeMovedByPlayer; set => canBeMovedByPlayer = value; }
    public bool ChangesPrefab { get => changesPrefab; set => changesPrefab = value; }
    public bool CanCollideOnTimeTravel { get => canCollideOnTimeTravel; set => canCollideOnTimeTravel = value; }

    private bool TObjectOrWBoxNull =>
        (past == null || present == null || future == null || past.wireBox == null || present.wireBox == null ||
         future.wireBox == null);

    private TimeTravelPeriod traveledFrom, traveledTo;

    private Dictionary<string, DisplacmentInfo[]> DisplacementsAndRenderers = new Dictionary<string, DisplacmentInfo[]>();
    private Material pastDisplacement, presentDisplacement, futureDisplacement;

    public void Awake() {
        CheckForMissingComponents();
        DetermineTimeTravelObjectState();
        pastDisplacement = Resources.Load("TimeTravelDisplacement0") as Material;
        presentDisplacement = Resources.Load("TimeTravelDisplacement1") as Material;
        futureDisplacement = Resources.Load("TimeTravelDisplacement2") as Material;


        past?.SetUpTimeTravelObject(this);
        present?.SetUpTimeTravelObject(this, past);
        future?.SetUpTimeTravelObject(this, present);

        if (Application.isPlaying) {
            TimePeriodChanged.AddListener<TimePeriodChanged>(OnTimePeriodChanged);
            PhysicsSimulationComplete.AddListener<PhysicsSimulationComplete>(OnPhysicsSimulationComplete);
        }

        if (!gameObject.activeSelf) gameObject.SetActive(true);

        if (past) { CategorizeRenderersForDisplacement(past.transform, past); past.timeTravelPeriod = TimeTravelPeriod.Past; past.gameObject.SetActive(true); }
        if (present) { CategorizeRenderersForDisplacement(present.transform, present); present.timeTravelPeriod = TimeTravelPeriod.Present; present.gameObject.SetActive(true); }
        if (future) { CategorizeRenderersForDisplacement(future.transform, future); future.timeTravelPeriod = TimeTravelPeriod.Future; future.gameObject.SetActive(true); }

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
        switch (changesPrefab) {
            case true when !canBeMovedByPlayer: ObjectState = TimeTravelObjectState.PrefabChanging; break;
            case true when canBeMovedByPlayer: ObjectState = TimeTravelObjectState.PrefabChangingPlayerMove; break;
        }
    }

    public class DisplacmentInfo {
        public Renderer renderer;
        public string rendererID;
        public TimeTravelDisplacement displacement;
        public Material[] originalMaterials;
    }

    private void CategorizeRenderersForDisplacement(Transform currentTransform, TimeTravelObject currentTTO) {
        Renderer subRenderer = currentTransform.GetComponent<Renderer>();

        if (subRenderer && currentTransform.name.Split('[').Length >= 3) { // TTO was not made with correct ID in name
            DisplacmentInfo info = new DisplacmentInfo();
            TimeTravelDisplacement displacement = currentTransform.gameObject.GetOrAddComponent<TimeTravelDisplacement>();
            info.displacement = displacement;
            info.renderer = subRenderer;
            info.originalMaterials = subRenderer.sharedMaterials;

            string[] splitName = currentTransform.name.Split('[');

            string rendererID = splitName[2].Substring(0, splitName[2].Length - 1);
            info.rendererID = rendererID;
            currentTTO.RendererIDs.Add(rendererID);

            if (!DisplacementsAndRenderers.ContainsKey(rendererID)) DisplacementsAndRenderers.Add(rendererID, new DisplacmentInfo[3]);

            switch (splitName[3].Substring(0, splitName[3].Length - 1).ToLower()) {
                case "past": DisplacementsAndRenderers[rendererID][0] = info; break;
                case "present": DisplacementsAndRenderers[rendererID][1] = info; break;
                case "future": DisplacementsAndRenderers[rendererID][2] = info; break;
            }
        }

        for (int i = 0; i < currentTransform.childCount; i++) CategorizeRenderersForDisplacement(currentTransform.GetChild(i), currentTTO);
    }

    public void RemoveRendererInfo(string ID) { if (DisplacementsAndRenderers.ContainsKey(ID)) DisplacementsAndRenderers.Remove(ID); }

    private void Update() {
        if (TObjectOrWBoxNull) return;
        UpdateActivePosition();
        UpdateWireBox(past, Color.white);
        UpdateWireBox(present, Color.grey);
        UpdateWireBox(future, Color.blue);
    }

    private void UpdateActivePosition() {
        if (past.IsActive) activePosition = past.transform.position;
        else if (present.IsActive) activePosition = present.transform.position;
        else activePosition = future.transform.position;
    }

    private void SetUpWireBox(TimeTravelObject travelObject, Color color) {
        GameObject wireObj = new GameObject(travelObject.name + " preview");
        wireObj.transform.parent = transform;
        travelObject.previewBoxObject = wireObj;
        travelObject.wireBox = travelObject.previewBoxObject.AddComponent<WireBox>();
        travelObject.wireBox.Color = color;
    }

    private void UpdateWireBox(TimeTravelObject timeTravelObject, Color color) {
        if (showPreviewBox && !timeTravelObject.IsActive &&
            Vector3.Distance(timeTravelObject.transform.position, activePosition) > previewBoxMinShowDistance &&
            ObjectState == TimeTravelObjectState.PrefabChangingPlayerMove) {
            timeTravelObject.wireBox.SetLinePositions(previewBoxScale);
            timeTravelObject.previewBoxObject.SetActive(true);
            timeTravelObject.previewBoxObject.transform.position = timeTravelObject.transform.position;
            timeTravelObject.wireBox.Color = color;
        } else timeTravelObject.previewBoxObject?.SetActive(false);
    }

    private void HandleDisplacement(TimePeriodChanged e) {
        traveledFrom = e.from;
        traveledTo = e.to;

        Material displacement = null;
        switch (e.to) {
            case TimeTravelPeriod.Past: displacement = pastDisplacement; break;
            case TimeTravelPeriod.Present: displacement = presentDisplacement; break;
            case TimeTravelPeriod.Future: displacement = futureDisplacement; break;
        }

        if ((int)traveledFrom < 3 && (int)traveledTo < 3 && !e.IsReload) {
            Material[] displacementMat = new Material[] { displacement };

            foreach (var info in DisplacementsAndRenderers.Values) {
                if (!displaceOnTimeTravel || info[(int)traveledFrom] == null || info[(int)traveledTo] == null) continue;
          info[(int)traveledFrom].renderer.sharedMaterials = displacementMat;
                    info[(int)traveledTo].renderer.sharedMaterials = displacementMat;
                if (info[(int)traveledFrom].renderer.gameObject.activeInHierarchy) {
          
                    info[(int)traveledFrom].displacement.Displace(info[(int)traveledTo].renderer.transform);
                }
            }
        }

        if (gameObject.activeInHierarchy) StartCoroutine(DisplacementComplete());
        else DiscplacementCompleteHelper();
    }

    private IEnumerator<WaitForSecondsRealtime> DisplacementComplete() {
        yield return new WaitForSecondsRealtime(!displaceOnTimeTravel ? 0 : 0.4f);

        /*        if ((int)traveledFrom < 3 || (int)traveledTo < 3) {
                   foreach (var info in DisplacementsAndRenderers.Values) {
                       for (int i = 0; i < 3; i++) {
                           if (info[i] == null) continue;
                           info[i].renderer.sharedMaterials = info[i].originalMaterials;
                           info[i].renderer.enabled = i == (int)traveledTo ? true : false;
                       }
                   }
               } */
        DiscplacementCompleteHelper();
    }

    private void DiscplacementCompleteHelper() {
        if ((int)traveledFrom < 3 || (int)traveledTo < 3) {
            foreach (var info in DisplacementsAndRenderers.Values) {
                for (int i = 0; i < 3; i++) {
                    if (info[i] == null) continue;
                    info[i].renderer.sharedMaterials = info[i].originalMaterials;
                    info[i].renderer.enabled = i == (int)traveledTo ? true : false;
                }
            }
        }
    }

    // callbacks
    private void OnTimePeriodChanged(TimePeriodChanged e) {
        switch (ObjectState) {
            case TimeTravelObjectState.PrefabChanging:
            case TimeTravelObjectState.PrefabChangingPlayerMove:
                if (past != null && past.wireBox == null && ObjectState == TimeTravelObjectState.PrefabChangingPlayerMove)
                    SetUpWireBox(past, Color.white);
                if (present != null && present.wireBox == null && ObjectState == TimeTravelObjectState.PrefabChangingPlayerMove)
                    SetUpWireBox(present, Color.gray);
                if (future != null && future.wireBox == null && ObjectState == TimeTravelObjectState.PrefabChangingPlayerMove)
                    SetUpWireBox(future, Color.blue);

                if (past) past.SetActive(e.to == TimeTravelPeriod.Past ? true : false);
                if (present) present.SetActive(e.to == TimeTravelPeriod.Present ? true : false);
                if (future) future.SetActive(e.to == TimeTravelPeriod.Future ? true : false);

                HandleDisplacement(e);

                break;
            case TimeTravelObjectState.Dummy: break;
        }
    }

    private void OnPhysicsSimulationComplete(PhysicsSimulationComplete e) {
        if (ObjectState == TimeTravelObjectState.PrefabChangingPlayerMove) {
            if (past?.Rigidbody != null) past.Rigidbody.isKinematic = e.to is not TimeTravelPeriod.Past;
            if (present?.Rigidbody != null) present.Rigidbody.isKinematic = e.from is TimeTravelPeriod.Future && e.to is TimeTravelPeriod.Present;
        }
    }

    private void OnCheckPlayerCanMove() {
        if (!canBeMovedByPlayer) return;
        if (canBeMovedByPlayer) canCollideOnTimeTravel = true;
    }

    private void OnCheckCanCollide() { if (canBeMovedByPlayer && !canCollideOnTimeTravel) canBeMovedByPlayer = false; }

    private void OnCheckChangesMesh() {
        if (changesPrefab) return;
        canBeMovedByPlayer = false;
    }

    private void OnDestroy() {
        if (EventSystem.Current != null) {
            TimePeriodChanged.RemoveListener<TimePeriodChanged>(OnTimePeriodChanged);
            PhysicsSimulationComplete.RemoveListener<PhysicsSimulationComplete>(OnPhysicsSimulationComplete);
        }
    }
}

public enum TimeTravelObjectState {
    PrefabChanging,
    PrefabChangingPlayerMove,
    Dummy
}