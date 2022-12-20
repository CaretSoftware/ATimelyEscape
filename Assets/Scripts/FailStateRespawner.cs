using CallbackSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailStateRespawner : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform respawnPoint;
    // Start is called before the first frame update
    void Start()
    {
        CheckpointEvent.AddListener<CheckpointEvent>(NewCheckpoint);
        FailStateEvent.AddListener<FailStateEvent>(Respawn);
    }


    private void Respawn(FailStateEvent fail)
    {
        player.position = respawnPoint.position;
    }

    private void NewCheckpoint(CheckpointEvent checkpointEvent)
    {
        respawnPoint = checkpointEvent.respawnPoint;
    }
}
