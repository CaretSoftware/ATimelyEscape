using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CognitiveAssistanceTriggerHandler : MonoBehaviour
{
    [SerializeField] private BoxCollider collider;
    private List<CognitiveAssistanceTriggerHandler> handlers;

    private void Start()
    {
        fetchAll();
    }

    private void fetchAll()
    {
        GameObject[] handlers = GameObject.FindGameObjectsWithTag("CognitiveAssistanceHandler");
        for (int i = 0; i < handlers.Length; i++)
        {
            CognitiveAssistanceTriggerHandler handler = handlers[i].GetComponent<CognitiveAssistanceTriggerHandler>();
            this.handlers.Add(handler);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        for(int i = 0; i < handlers.Count; i++)
            handlers[i].gameObject.SetActive(false);
        gameObject.SetActive(true);
    }
}
