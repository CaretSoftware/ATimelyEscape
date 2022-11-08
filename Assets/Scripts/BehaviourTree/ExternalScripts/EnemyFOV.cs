using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFOV : MonoBehaviour
{
    [Tooltip("Player layerMask")]
    [SerializeField] private LayerMask playerMask;
    [Tooltip("LayerMask of gameobjects which the player can hide behind")] 
    [SerializeField] private LayerMask obstacleMask;
    [Tooltip("How frequently the enemy will run the search method")]
    [SerializeField] private float searchDelay = 0.2f;

    private Vector3 directionToTarget;
    private Collider[] rangeChecks;

    private float distanceToTarget;
    [Header("Detection range variables")]
    [Range(0, 360)] 
    public float Angle;
    public float FacingViewRange;
    public float ChaseRadius;
    public float CatchRadius;

    public Transform Center { get; private set; }

    [HideInInspector] public Transform player;
    [HideInInspector] public bool playerDetected { get; private set; }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        Center = GetComponent<EnemyAI>().AgentCenterTransform;
    }

    private void Update()
    {
        FOVCheck();
    }

    private void FOVCheck()
    {
        rangeChecks = Physics.OverlapSphere(transform.position, FacingViewRange, playerMask);
        if (rangeChecks.Length != 0)
        {
            player = rangeChecks[0].transform;
            directionToTarget = (player.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToTarget) < Angle / 2)
            {
                distanceToTarget = Vector3.Distance(transform.position, player.position);
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
                    playerDetected = true;
                else
                    playerDetected = false;
            }
            else
                playerDetected = false;
        }
        else if (playerDetected)
            playerDetected = false;
    }
}
