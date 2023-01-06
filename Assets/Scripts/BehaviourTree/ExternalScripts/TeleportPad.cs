using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class TeleportPad : MonoBehaviour
{
    [SerializeField] private float coolDownTime = 5f; 
    [SerializeField] private TravelPathScript cable;
    [SerializeField] private TeleportPad linkedPad;

    [SerializeField] private TeleportPad firstPad;
    //private TeleportPad[] pads;
    [SerializeField] private MeshRenderer lightIndicator;
    private NavMeshHit hit;
    [SerializeField ]private bool active;

    [HideInInspector] public Material indicatorMaterial;
    [HideInInspector] public bool OnCooldown;

    private bool instantiate;
    
    //TODO Dubbelkolla material-lösningen med Wessman.
    private void Start()
    {
        //pads = transform.parent.GetComponentsInChildren<TeleportPad>();
        //cable = transform.parent.GetComponentInChildren<TravelPathScript>();
        //linkedPad = pads[0] != this ? pads[0] : pads[1];
        //lightIndicator.material = Resources.Load("TeleportMatGreen") as Material; //= new Material(Shader.Find("Unlit/Color"));
    }

    private void Update()
    {
        if (!instantiate)
        {
            UpdateMaterial();
            instantiate = true;
        }
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
        cable.TravelPath(linkedPad != firstPad, target.gameObject, linkedPad.transform.position);
        StartCoroutine(CoolDownTimer());
    }

    public IEnumerator CoolDownTimer()
    {
        lightIndicator.material = Resources.Load("TeleportMatCyan") as Material;
        synchronizeLinkedPad();
        yield return new WaitForSeconds(coolDownTime);
        OnCooldown = false;
        UpdateMaterial();
    }

    private void UpdateMaterial()
    {
        lightIndicator.material = active ? 
            Resources.Load("TeleportMatGreen") as Material : 
            Resources.Load("TeleportMatRed") as Material;
        synchronizeLinkedPad();
    }

    private void synchronizeLinkedPad()
    {
        linkedPad.lightIndicator.material = lightIndicator.material;
        linkedPad.OnCooldown = OnCooldown;
    }
    public void TeleportOn()
    {
        active = true;
        UpdateMaterial();
    }
    public void TeleportOff()
    {
        active = false;
        UpdateMaterial();
    }
}
