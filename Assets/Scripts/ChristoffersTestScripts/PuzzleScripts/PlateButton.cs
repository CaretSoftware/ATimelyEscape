using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CallbackSystem;

public class PlateButton : MonoBehaviour
{
    [Header("Materials")]
    [SerializeField] private Material OnMaterial;
    [SerializeField] private Material OffMaterial;
    [Header("Puzzle Components")]
    [SerializeField] private TextMeshProUGUI instructions;
    [SerializeField] private GameObject puzzleFloor;
    [SerializeField] private GameObject sign;
    [SerializeField] private GameObject puzzle;
    [SerializeField] private GameObject platePuzzle;
    [SerializeField] private GameObject platePuzzle2;
    [SerializeField] private GameObject bigHatch;
    [SerializeField] private GameObject exitHatch;
    [SerializeField] private GameObject candyFeeder;
    [SerializeField] private PlateButton presentPlate;
    [Header("Plate Components")]
    [SerializeField] private GameObject cord;
    [SerializeField] private GameObject cord2;
    [SerializeField] private GameObject plate;
    [SerializeField] private GameObject pastCord;
    [SerializeField] private GameObject pastCord2;
    [SerializeField] private GameObject pastPlate;
    [SerializeField] private bool isPastPlate;
    private MeshRenderer signMeshRenderer;
    private MeshRenderer meshRenderer;
    private MeshRenderer cordMeshRenderer;
    private MeshRenderer cord2MeshRenderer;
    private MeshRenderer plateMeshrenderer;
    private MeshRenderer pastCordMeshRenderer;
    private MeshRenderer pastCord2MeshRenderer;
    private MeshRenderer pastPlateMeshrenderer;
    private Incubator puzzleIncubator;
    private Animator bigHatchAnim;
    private Animator platePuzzleAnim;
    public bool pastON;

    private void Start()
    {
        TimePeriodChanged.AddListener<TimePeriodChanged>(TimeTravel);
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        cordMeshRenderer = cord.GetComponent<MeshRenderer>();
        cord2MeshRenderer = cord2.GetComponent<MeshRenderer>();
        plateMeshrenderer = plate.GetComponent<MeshRenderer>();
        pastCordMeshRenderer = pastCord.GetComponent<MeshRenderer>();
        pastCord2MeshRenderer = pastCord2.GetComponent<MeshRenderer>();
        pastPlateMeshrenderer = pastPlate.GetComponent<MeshRenderer>();
        signMeshRenderer = sign.GetComponent<MeshRenderer>();
        puzzleIncubator = puzzle.GetComponent<Incubator>();
        bigHatchAnim = bigHatch.GetComponent<Animator>();
        platePuzzleAnim = platePuzzle.GetComponent<Animator>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Cube")
        {
            platePuzzle2.SetActive(true);
            ButtonOn();
            puzzleIncubator.puzzleFiveDone = true;
            signMeshRenderer.material = OnMaterial;
            instructions.text = "GOOD";
            bigHatchAnim.SetBool("OpenLast", true);
            platePuzzleAnim.SetBool("Open", false);
            StartCoroutine(puzzleIncubator.Delay());
            Debug.Log("STEP14");
        }
        if (other.gameObject.tag == "CubePast")
        {
            if (isPastPlate)
            {
                ButtonOn();
                pastON = true;
                presentPlate.pastON = true; 
            }
        }
        if (other.gameObject.tag == "CubePresent")
        {
            if (!isPastPlate)
            {
                ButtonOn();
                if (pastON)
                {
                    instructions.text = "GOOD, ALL DONE";
                    signMeshRenderer.material = OnMaterial;
                    exitHatch.GetComponent<Animator>().SetBool("Open", true);
                    candyFeeder.GetComponent<Animator>().SetBool("Open", true);
                    //Invoke("OpenExit", 1);
                }
            }
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "CubePast")
        {
            if (isPastPlate)
            {
                ButtonOff();
                pastON = false;
                presentPlate.pastON = false;
            }
        }

    }

    private void TimeTravel(TimePeriodChanged e)
    {
        if (e.from == TimeTravelPeriod.Past && e.to == TimeTravelPeriod.Present && pastON)
        {
            pastCordMeshRenderer.material = OnMaterial;
            pastCord2MeshRenderer.material = OnMaterial;
            pastPlateMeshrenderer.material = OnMaterial;
            presentPlate.pastON = true;
        }
    }
    private void ButtonOn()
    {
        meshRenderer.material = OnMaterial;
        cordMeshRenderer.material = OnMaterial;
        cord2MeshRenderer.material = OnMaterial;
        plateMeshrenderer.material = OnMaterial;
    }
    private void ButtonOff()
    {
        meshRenderer.material = OffMaterial;
        cordMeshRenderer.material = OffMaterial;
        cord2MeshRenderer.material = OffMaterial;
        plateMeshrenderer.material = OffMaterial;
    }

}
