using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Plate : MonoBehaviour
{
    [SerializeField] private Material OnMaterial;
    [SerializeField] private Material OffMaterial;

    [SerializeField] private bool isPast;
    [SerializeField] private bool isPresent;
    [SerializeField] private bool isFuture;

    [SerializeField] private GameObject plateHolderGO;
    [SerializeField] private GameObject cord;
    [SerializeField] private GameObject cord2;
    [SerializeField] private GameObject sign;
    /*[SerializeField] private GameObject clock;
    [SerializeField] private GameObject pastClock;
    [SerializeField] private GameObject presentClock;*/

    public bool pastOn;
    public bool presentOn;
    public bool futureOn;

    private MeshRenderer meshRenderer;
    private MeshRenderer cordMeshRenderer;
    private MeshRenderer cord2MeshRenderer;
    private MeshRenderer signMeshRenderer;
/*    private DigitalClock digitalClock;
    private DigitalClock pastDigitalClock;
    private DigitalClock presentDigitalClock;*/
    /*private TextMeshProUGUI pastText;
    private TextMeshProUGUI presentText;*/
    private PlateHolder plateHolder;


    private void Awake()
    {
        pastOn = false;
        presentOn = false;
        futureOn = false;


        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        cordMeshRenderer = cord.GetComponent<MeshRenderer>();
        cord2MeshRenderer = cord2.GetComponent<MeshRenderer>();
        signMeshRenderer = sign.GetComponent<MeshRenderer>();
        //digitalClock = clock.GetComponent<DigitalClock>();
        plateHolder = plateHolderGO.GetComponent<PlateHolder>();

       /* if (pastClock != null)
        {
            pastDigitalClock = pastClock.GetComponent<DigitalClock>();
            pastText = pastClock.GetComponentInChildren<TextMeshProUGUI>();
        }
        if (presentClock != null)
        {
            presentDigitalClock = presentClock.GetComponent<DigitalClock>();
            presentText = presentClock.GetComponentInChildren<TextMeshProUGUI>();
        }*/
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isPast)
        {
            if (other.gameObject.tag == "CubePast")
            {
                ButtonOn();
                //pastDigitalClock.enabled = false;
                //pastText.text = "0";
                pastOn = true;
                //digitalClock.takingAway = false;
            }
        }
        if (isPresent)
        {
            if (other.gameObject.tag == "CubePresent")
            {
                ButtonOn();
                //pastDigitalClock.enabled = false;
                //presentDigitalClock.enabled = false;
                //pastText.text = "0";
                //presentText.text = "0";
                presentOn = true;
                //digitalClock.takingAway = false;

            }
        }
        if (isFuture)
        {
            if (other.gameObject.tag == "CubeFuture")
            {
                ButtonOn();
                futureOn = true;
                //digitalClock.takingAway = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (plateHolder.allIsDone == false)
        {

            if (isPast)
            {
                if (other.gameObject.tag == "CubePast")
                {
                    ButtonOff();
                    //pastDigitalClock.enabled = true;
                    pastOn = false;
                }
            }
            if (isPresent)
            {
                if (other.gameObject.tag == "CubePresent")
                {
                    ButtonOff();
                    //pastDigitalClock.enabled = true;
                    //presentDigitalClock.enabled = true;
                    presentOn = false;
                }
            }
            if (isFuture)
            {
                if (!pastOn || !presentOn)
                {
                    if (other.gameObject.tag == "CubeFuture")
                    {
                        ButtonOff();
                        futureOn = false;
                    }
                }
            }
        }
        else
        {
            meshRenderer.material = OnMaterial;
            cordMeshRenderer.material = OnMaterial;
            cord2MeshRenderer.material = OnMaterial;
            signMeshRenderer.material = OnMaterial;
        }
    }
    private void ButtonOn()
    {
        meshRenderer.material = OnMaterial;
        //gameObject.GetComponentInChildren<MeshRenderer>().material = OnMaterial;
        cordMeshRenderer.material = OnMaterial;
        cord2MeshRenderer.material = OnMaterial;
        signMeshRenderer.material = OnMaterial;
        //digitalClock.isOn = true;
    }
    private void ButtonOff()
    {
        meshRenderer.material = OffMaterial;
        cordMeshRenderer.material = OffMaterial;
        cord2MeshRenderer.material = OffMaterial;
        signMeshRenderer.material = OffMaterial;
        //digitalClock.isOn = false;
    }


}
