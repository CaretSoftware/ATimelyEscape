using System;
using System.Collections.Generic;
using CallbackSystem;
using NaughtyAttributes;
using UnityEngine;

public class TimeTravelObjectManager : MonoBehaviour {
    [Tooltip(
        "Whether the time travelling object uses different prefabs in different time periods, this goes for meshes AND decals")]
    [DisableIf(nameof(changesMaterials))]
    [OnValueChanged(nameof(OnCheckChangesMesh))]
    [SerializeField]
    private bool changesPrefab;

    [Tooltip("Whether the object changes materials upon travelling between different time periods. " +
             "ATTENTION: It's very important that the renderers are added in the arrays in the SAME ORDER as the materials otherwise it will not work properly." +
             " NOTE: Materials specific for each period have to be already applied to the meshes in question")]
    [DisableIf(EConditionOperator.Or, nameof(canBeMovedByPlayer), nameof(changesPrefab))]
    [OnValueChanged(nameof(OnCheckMaterialsChange))]
    [SerializeField]
    private bool changesMaterials;

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


    [ShowIf(EConditionOperator.Or, nameof(changesPrefab), nameof(canBeMovedByPlayer))]
    [SerializeField]
    private TimeTravelObject past, present, future;

    [HideIf(EConditionOperator.Or, nameof(changesPrefab), nameof(canBeMovedByPlayer))]
    [SerializeField]
    private TimeTravelObject timeTravelObject;

    public TimeTravelObjectState ObjectState { get; private set; }

    public MaterialInfo[] PastMaterials => pastMaterials;
    public MaterialInfo[] PresentMaterials => presentMaterials;
    public MaterialInfo[] FutureMaterials => futureMaterials;
    public Renderer[] Renderers => renderers;

    private Vector3 activePosition;

    public TimeTravelObject Past { get => past; set => past = value; }
    public TimeTravelObject Present { get => present; set => present = value; }
    public TimeTravelObject Future { get => future; set => future = value; }

    public bool CanBeMovedByPlayer { get => canBeMovedByPlayer; set => canBeMovedByPlayer = value; }
    public bool ChangesMaterials { get => changesMaterials; set => changesMaterials = value; }
    public bool ChangesPrefab { get => changesPrefab; set => changesPrefab = value; }
    public bool ShowPreviewBox { get => showPreviewBox; set => showPreviewBox = value; }
    public float PreviewBoxScale { get => previewBoxScale; set => previewBoxScale = value; }
    public float PreviewBoxMinDistance { get => previewBoxMinShowDistance; set => previewBoxMinShowDistance = value; }
    public bool CanCollideOnTimeTravel { get => canCollideOnTimeTravel; set => canCollideOnTimeTravel = value; }

    private bool TObjectOrWBoxNull =>
        (past == null || present == null || future == null || past.wireBox == null || present.wireBox == null ||
         future.wireBox == null);

    private int traveledFromIndex, traveledToIndex;

    private Dictionary<string, DisplacmentInfo[]> DisplacementsAndRenderers = new Dictionary<string, DisplacmentInfo[]>();

    public void Awake() {
        CheckForMissingComponents();
        DetermineTimeTravelObjectState();

        if (changesPrefab || canBeMovedByPlayer) {
            past?.SetUpTimeTravelObject(this);
            present?.SetUpTimeTravelObject(this, past);
            future?.SetUpTimeTravelObject(this, present);
        } else timeTravelObject.SetUpTimeTravelObject(this);


        if (Application.isPlaying) {
            TimePeriodChanged.AddListener<TimePeriodChanged>(OnTimePeriodChanged);
            PhysicsSimulationComplete.AddListener<PhysicsSimulationComplete>(OnPhysicsSimulationComplete);
        }

        if (past) { CategorizeRenderersForDisplacement(past.transform); past.timeTravelPeriod = TimeTravelPeriod.Past; }
        if (present) { CategorizeRenderersForDisplacement(present.transform); present.timeTravelPeriod = TimeTravelPeriod.Present; }
        if (future) { CategorizeRenderersForDisplacement(future.transform); future.timeTravelPeriod = TimeTravelPeriod.Future; }

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
            default: { if (changesMaterials && !canBeMovedByPlayer && !changesPrefab) ObjectState = TimeTravelObjectState.SwitchingMaterial; break; }
        }
    }

    public class DisplacmentInfo {
        public Renderer renderer;
        public string rendererID;
        public TimeTravelDisplacement displacement;
        public Material[] originalMaterials;
    }

    private void CategorizeRenderersForDisplacement(Transform currentTransform) {
        Renderer subRenderer = currentTransform.GetComponent<Renderer>();
        if (subRenderer) {
            DisplacmentInfo info = new DisplacmentInfo();
            TimeTravelDisplacement displacement = currentTransform.gameObject.GetOrAddComponent<TimeTravelDisplacement>();
            info.displacement = displacement;
            info.renderer = subRenderer;
            info.originalMaterials = subRenderer.materials;

            string[] splitName = currentTransform.name.Split('_');

            string rendererID = splitName[1];
            info.rendererID = rendererID;

            if (!DisplacementsAndRenderers.ContainsKey(rendererID)) DisplacementsAndRenderers.Add(rendererID, new DisplacmentInfo[3]);

            switch (splitName[2].Substring(1, splitName[2].Length - 2).ToLower()) {
                case "past": DisplacementsAndRenderers[rendererID][0] = info; break;
                case "present": DisplacementsAndRenderers[rendererID][1] = info; break;
                case "future": DisplacementsAndRenderers[rendererID][2] = info; break;
            }
        }

        for (int i = 0; i < currentTransform.childCount; i++) CategorizeRenderersForDisplacement(currentTransform.GetChild(i));
    }

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

    private void DetermineTimeTravelIndex(bool from, TimeTravelPeriod period) {
        int temp = -1;
        switch (period) {
            case TimeTravelPeriod.Past:
                if (!past) temp = -1;
                else temp = 0;
                break;
            case TimeTravelPeriod.Present:
                if (!present) temp = -1;
                else temp = 1;
                break;
            case TimeTravelPeriod.Future:
                if (!future) temp = -1;
                else temp = 2;
                break;
        }
        if (from) traveledFromIndex = temp;
        else traveledToIndex = temp;
    }

    private void HandleDisplacement(TimePeriodChanged e) {
        DetermineTimeTravelIndex(true, e.from);
        DetermineTimeTravelIndex(false, e.to);

        if (traveledToIndex != -1 && traveledFromIndex != -1) {
            Material[] displacementMat = new Material[] { Resources.Load("TimeTravelDisplacement1") as Material };
            foreach (var info in DisplacementsAndRenderers.Values) {
                if (info[traveledFromIndex] == null || info[traveledToIndex] == null) continue;
                info[traveledFromIndex].renderer.materials = displacementMat;
                info[traveledToIndex].renderer.materials = displacementMat;
                info[traveledFromIndex].displacement.Displace(info[traveledToIndex].renderer.transform);
            }
        }


        StartCoroutine(DisplacementComplete());
    }

    private IEnumerator<WaitForSecondsRealtime> DisplacementComplete() {
        yield return new WaitForSecondsRealtime(0.2f);

        if (traveledFromIndex > -1 || traveledToIndex > -1) {
            foreach (var info in DisplacementsAndRenderers.Values) {
                for (int i = 0; i < 3; i++) {
                    if (info[i] == null) continue;
                    info[i].renderer.materials = info[i].originalMaterials;
                    info[i].renderer.enabled = i == traveledToIndex ? true : false;
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

                past?.SetActive(e.to == TimeTravelPeriod.Past ? true : false);
                present?.SetActive(e.to == TimeTravelPeriod.Present ? true : false);
                future?.SetActive(e.to == TimeTravelPeriod.Future ? true : false);

                HandleDisplacement(e);

                break;
            case TimeTravelObjectState.SwitchingMaterial: timeTravelObject.UpdateMaterials(e.to); break;
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
        changesMaterials = false;
    }

    private void OnCheckCanCollide() { if (canBeMovedByPlayer && !canCollideOnTimeTravel) canBeMovedByPlayer = false; }
    private void OnCheckMaterialsChange() { if (changesMaterials) changesPrefab = false; }

    private void OnCheckChangesMesh() {
        if (changesPrefab) return;
        canBeMovedByPlayer = false;
    }

    [Serializable]
    public class MaterialInfo { public Material[] materials; }
}

public enum TimeTravelObjectState {
    PrefabChanging = 0,
    PrefabChangingPlayerMove = 1,
    SwitchingMaterial = 2,
    Dummy = 3
}