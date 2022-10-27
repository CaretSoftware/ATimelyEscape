using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VacumController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private float startWaitTime = 4f;
    [SerializeField] private float timeToRotate = 2f;
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float runSpeed = 6f;


    [SerializeField] private float viewRadius = 15f;
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask obsticleMask;
    /*  [SerializeField] private float meshResolution = 1f;
      [SerializeField] private int edgeIterations = 4;
      [SerializeField] private float edgeDistance = 0.5f;*/

    [SerializeField] private GameObject eyeBrows;
    [SerializeField] private Transform[] wayPoints;
    private int currentWeaponIndex;

    private Vector3 playersLastPosition = Vector3.zero;
    private Vector3 playerPosition;
    private float waitTime;
    private float mtimeToRotate;
    private bool playerInRange;
    private bool playerNear;
    private bool isPatrol;
    private bool caughtPlayer;


    // Start is called before the first frame update
    void Start()
    {
        if (navMeshAgent != null)
        {

            playerPosition = Vector3.zero;
            isPatrol = true;
            caughtPlayer = false;
            playerInRange = false;
            waitTime = startWaitTime;
            mtimeToRotate = timeToRotate;
            eyeBrows.SetActive(false);

            currentWeaponIndex = 0;
            navMeshAgent = GetComponent<NavMeshAgent>();

            navMeshAgent.isStopped = false;
            navMeshAgent.speed = walkSpeed;
            navMeshAgent.SetDestination(wayPoints[currentWeaponIndex].position);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
        EnviromentView();
        if (!isPatrol)
        {
            Chasing();
        }
        else
        {
            Patrolling();
        }
    }
    private void Chasing()
    {
        if (navMeshAgent != null)
        {

            playerNear = false;
            playersLastPosition = Vector3.zero;
            if (!caughtPlayer)
            {
                Move(runSpeed);
                navMeshAgent.SetDestination(playerPosition);
                eyeBrows.SetActive(true);
            }
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (waitTime <= 0 && !caughtPlayer && Vector3.Distance(transform.position,
                    GameObject.FindGameObjectWithTag("Player").transform.position) >= 6f)
                {
                    isPatrol = true;
                    playerNear = false;
                    Move(walkSpeed);
                    mtimeToRotate = timeToRotate;
                    waitTime = startWaitTime;
                    navMeshAgent.SetDestination(wayPoints[currentWeaponIndex].position);
                    eyeBrows.SetActive(false);
                }
                else
                {
                    if (Vector3.Distance(transform.position,
                        GameObject.FindGameObjectWithTag("Player").transform.position) >= 2.5f)
                    {
                        Stop();
                        waitTime -= Time.deltaTime;
                    }
                }
            }
        }
    }

    private void Patrolling()
    {
        if (navMeshAgent != null)
        {

            if (playerNear)
            {
                if (mtimeToRotate <= 0)
                {
                    Move(walkSpeed);
                    LookingPlayer(playersLastPosition);

                }
                else
                {
                    Stop();
                    mtimeToRotate -= Time.deltaTime;
                }
            }
            else
            {
                playerNear = false;
                playersLastPosition = Vector3.zero;
                navMeshAgent.SetDestination(wayPoints[currentWeaponIndex].position);
                if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
                {
                    if (waitTime <= 0)
                    {
                        NextPoint();
                        Move(walkSpeed);
                        waitTime = startWaitTime;
                    }
                    else
                    {
                        Stop();
                        waitTime -= Time.deltaTime;
                    }
                }
            }
        }
    }
    private void Move(float speed)
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speed;
    }
    private void Stop()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.speed = 0;
    }
    public void NextPoint()
    {
        currentWeaponIndex = (currentWeaponIndex + 1) % wayPoints.Length;
        navMeshAgent.SetDestination(wayPoints[currentWeaponIndex].position);
    }

    private void CaughtPlayer()
    {
        caughtPlayer = true;
    }

    private void LookingPlayer(Vector3 player)
    {
        navMeshAgent.SetDestination(player);
        if (Vector3.Distance(transform.position, player) <= 0.3)
        {
            if (waitTime <= 0)
            {
                playerNear = false;
                Move(walkSpeed);
                navMeshAgent.SetDestination(wayPoints[currentWeaponIndex].position);
                waitTime = startWaitTime;
                mtimeToRotate = timeToRotate;
            }
            else
            {
                Stop();
                waitTime -= Time.deltaTime;
            }
        }
    }
    private void EnviromentView()
    {
        if (navMeshAgent != null)
        {

            Collider[] playerIsInRange = Physics.OverlapSphere(transform.position, viewRadius, playerMask);
            for (int i = 0; i < playerIsInRange.Length; i++)
            {
                Transform player = playerIsInRange[i].transform;
                Vector3 dirToPlayer = (player.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2)
                {
                    float dstToPlayer = Vector3.Distance(transform.position, player.position);
                    if (!Physics.Raycast(transform.position, dirToPlayer, dstToPlayer, obsticleMask))
                    {
                        playerInRange = true;
                        isPatrol = false;

                    }
                    else
                    {
                        playerInRange = false;
                    }
                }
                if (Vector3.Distance(transform.position, player.position) > viewRadius)
                {
                    playerInRange = false;
                }
                if (playerInRange)
                {
                    playerPosition = player.transform.position;
                }
            }
        }
    }
}
