using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CognitiveAssistanceTriggerHandler : MonoBehaviour
{
    private List<CognitiveAssistanceTriggerHandler> handlers = new List<CognitiveAssistanceTriggerHandler>();

    [SerializeField] private BoxCollider collider;
    [SerializeField] private ObjectiveHolder objectiveHolder;
    [SerializeField] private GameObject[] children;
    
    [HideInInspector] public bool active;
    
    private void Start()
    {
        collider.enabled = true;
        collider.isTrigger = true;
        setChildrenActive(false);
        fetchAll();
    }

    private void fetchAll()
    {
        GameObject[] handlers = GameObject.FindGameObjectsWithTag("CognitiveAssistanceHandler");
        for (int i = 0; i < handlers.Length; i++)
        {
            this.handlers.Add(handlers[i].GetComponent<CognitiveAssistanceTriggerHandler>());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(active || other.tag != "Player") 
            return;

        for (int i = 0; i < handlers.Count; i++)
        {
            if (handlers[i] != this)
            {
                handlers[i].active = false;
                handlers[i].setChildrenActive(false);
            }
        }
        setChildrenActive(true);
        objectiveHolder.ClearList();
        objectiveHolder.Triggered();
        collider.enabled = false;
        active = true;
    }

    public void setChildrenActive(bool arg)
    {
        for(int i = 0; i < children.Length; i++)
            children[i].SetActive(arg);
    }
}
