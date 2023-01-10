using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideArrow : MonoBehaviour
{
    private static GuideArrow _instance;
    public static GuideArrow Instance { get { return _instance; } }

    private Transform target;
    private Transform rotator;
    private Transform scaler;
    
    public Transform Target { get { return target; } }
    public Transform Rotator { get { return rotator; } }

    [HideInInspector] public float speed;

    public void SetTarget(Transform target = null)
    {
        this.target = target;
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }

    private void Start()
    {
        gameObject.SetActive(false);
        scaler = transform.GetChild(0).GetChild(0).transform;
        rotator = scaler.GetChild(0).transform;
        ToggleGuideArrow(false);
    }

    private void Update()
    {
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
