using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CallbackSystem
{
    public class Checkpoint : MonoBehaviour
    {

        [SerializeField] private LayerMask activationLayers;
        [SerializeField] private Transform respawnPoint;
        private CheckpointEvent checkpointEvent;

        private void Start()
        {
            checkpointEvent = new(respawnPoint);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (activationLayers == (activationLayers | 1 << other.gameObject.layer))
            {
                checkpointEvent.Invoke();
            }
        }
    }
}