using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideArrow : MonoBehaviour
{
    private Transform target;
    private Transform rotator;
    private Transform scaler;
    private Transform player;
    private Vector3 arrowOffset = new Vector3(0, 0.2f, 0);
    
    public Transform Target { get { return target; } }
    public Transform Rotator { get { return rotator; } }

    [HideInInspector] public float speed;

    public void SetTarget(Transform target = null)
    {
        this.target = target;
    }

    private void Start()
    {
        ToggleGuideArrow(false);
        scaler = transform.GetChild(0).GetChild(0).transform;
        rotator = scaler.GetChild(0).transform;
        player = FindObjectOfType<NewRatCharacterController.NewRatCharacterController>().transform;

    }
    
    private void Update()
    {
        transform.position = player.position + arrowOffset;
        if(Target)
            transform.LookAt(target);
        if(Rotator)
            Rotator.transform.Rotate(0,0, speed * Time.deltaTime);
    }

    public void ToggleGuideArrow(bool arg)
    {
        gameObject.SetActive(arg);
    }
}
