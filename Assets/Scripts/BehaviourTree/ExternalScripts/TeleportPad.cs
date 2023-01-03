using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TeleportPad : MonoBehaviour
{
    [SerializeField] private float coolDownTime = 5f; 
    private TravelPathScript cable;
    private TeleportPad linkedPad;
    private TeleportPad[] pads;
    private MeshRenderer mr;
    private NavMeshHit hit;
    [SerializeField ]private bool active;

    [HideInInspector] public Material indicatorMaterial;
    [HideInInspector] public bool OnCooldown;
    private void Start()
    {
        pads = transform.parent.GetComponentsInChildren<TeleportPad>();
        cable = transform.parent.GetComponentInChildren<TravelPathScript>();
        linkedPad = pads[0] != this ? pads[0] : pads[1];
        indicatorMaterial = Resources.Load("TeleportMatGreen") as Material; //= new Material(Shader.Find("Unlit/Color"));
        mr = transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>();
        mr.material = indicatorMaterial;
        UpdateIndicatorColor();
    }

    private void OnTriggerEnter(Collider target)
    {
        if (active && !OnCooldown)
            if (target.transform.tag.Equals("Player") || target.transform.tag.Equals("Cube"))
                Teleport(target.transform);
    }
    
    private void Teleport(Transform target)
    {
        OnCooldown = true;
        synchronizeLinkedPad();
        //target.gameObject.SetActive(false);
        //send value indicating on which pad the object to be teleported interacted with.
        //if linkedPad is equal to the second object in the hierachy then teleportation occurred 
        //from the first pad, else from second pad.
        //true = first pad, false = second pad.
        cable.TravelPath(linkedPad == pads[1], target.gameObject, linkedPad.transform.position);
        StartCoroutine(CoolDownTimer());
    }

    public IEnumerator CoolDownTimer()
    {
        mr.material = Resources.Load("TeleportMatCyan") as Material;
        synchronizeLinkedPad();
        yield return new WaitForSeconds(coolDownTime);
        UpdateIndicatorColor();
        OnCooldown = false;
        synchronizeLinkedPad();
    }

    private void UpdateIndicatorColor()
    {
        mr.material = active ? 
            Resources.Load("TeleportMatGreen") as Material : 
            Resources.Load("TeleportMatRed") as Material;
        synchronizeLinkedPad();
    }

    private void synchronizeLinkedPad()
    {
        //if (linkedPad.mr.material == null)
            linkedPad.mr.material = mr.material;
        //linkedPad.indicatorMaterial.color = indicatorMaterial.color;
        linkedPad.OnCooldown = OnCooldown;
    }
    public void TeleportOn()
    {
        active = true;
        UpdateIndicatorColor();
    }
    public void TeleportOff()
    {
        active = false;
        UpdateIndicatorColor();
    }
}
