using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(BoxCollider))]
public class CognitiveAssistanceTriggerHandler : MonoBehaviour
{
    private List<CognitiveAssistanceTriggerHandler> handlers = new List<CognitiveAssistanceTriggerHandler>();

    [SerializeField] private BoxCollider collider;
    [SerializeField] private ObjectiveHolder objectiveHolder;
    [SerializeField] private GameObject[] children;
    
    [HideInInspector] public bool isCurrentlyActive;
    [SerializeField] public bool Active; 

    
    private void Start()
    {
        collider.enabled = true;
        collider.isTrigger = true;
        setChildrenActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {

        if(!Active || other.tag != "Player" || isCurrentlyActive) 
            return;

        fetchAll();
        for (int i = 0; i < handlers.Count; i++)
        {
            if (handlers[i] == this)
            {
                setChildrenActive(true);
                objectiveHolder.Triggered();
                collider.enabled = false;
                isCurrentlyActive = true;
                return;
            }
            handlers[i].isCurrentlyActive = false;
            handlers[i].setChildrenActive(false);
        }
    }

    private void fetchAll()
    {
        this.handlers.Clear();
        GameObject[] handlers = GameObject.FindGameObjectsWithTag("CognitiveAssistanceHandler");
        
        for (int i = 0; i < handlers.Length; i++)
            this.handlers.Add(handlers[i].GetComponent<CognitiveAssistanceTriggerHandler>());
    }

    public void setChildrenActive(bool arg)
    {
        for(int i = 0; i < children.Length; i++)
            children[i].SetActive(arg);
    }

    private void OnDestroy()
    {
        Destroy(gameObject);
    }
}
