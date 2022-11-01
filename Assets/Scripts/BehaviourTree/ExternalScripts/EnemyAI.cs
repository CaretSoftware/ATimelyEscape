using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private const float MovingToIdleMagnitude = 0.5f;
    private const float NavMeshRadiusOffstep = 5f;

    [Header("AI Detection Range Variables")]
    [SerializeField] [Range(1.0f, 10.0f)] private float chaseRange = 5.5f;
    [SerializeField] [Range(0.5f, 3.5f)] private float captureRange = 2.0f;

    [Header("AI Behaviour Input")]
    [SerializeField] [Range(0.0f, 10.0f)] private float idleActivityTimer = 5.0f;
    [SerializeField] private Transform checkpoint;
    [Tooltip("Assigning the same waypoints to multiple enemies may result in unwanted behaviour.")]
    [SerializeField] private Transform[] activityWaypoints;

    [Header("Scene Tools")]
    [SerializeField] private bool chaseRangeGizmo;
    [SerializeField] private bool catchRangeGizmo;
    [SerializeField] private bool waypointsGizmo;

    private Transform agentCenterTransform;
    private Transform playerTransform;
    private NavMeshAgent agent;
    private Animator animator;
    private Node topNode;

    private Vector3 worldDeltaPosition;
    private Vector2 smoothDeltaPosition;
    private Vector2 deltaPosition;
    private Vector2 velocity;

    private float deltaMagnitude;
    private float smooth;
    private float dx;
    private float dy;
    private bool shouldMove;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        animator.applyRootMotion = true;
        agent.updatePosition = false;
        agent.updateRotation = true;
    }

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        agentCenterTransform = GameObject.Find($"{gameObject.name}/AgentCenterTransform").transform;
        ConstructBehaviourTreePersonnel();
    }

    private void Update()
    {
        SynchronizeAnimatorAndAgent();
        topNode.Evaluate();
        if (topNode.nodeState == NodeState.FAILURE)
            agent.isStopped = true;
    }

    private void SynchronizeAnimatorAndAgent()
    {
        worldDeltaPosition = agent.nextPosition - transform.position;
        worldDeltaPosition.y = 0;

        dx = Vector3.Dot(transform.right, worldDeltaPosition);
        dy = Vector3.Dot(transform.forward, worldDeltaPosition);
        deltaPosition = new Vector2(dx, dy);

        smooth = Mathf.Min(1, Time.deltaTime / 0.5f);
        smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);
        velocity = smoothDeltaPosition / Time.deltaTime;

        if (agent.remainingDistance <= agent.stoppingDistance)
            velocity = Vector2.Lerp(Vector2.zero, velocity, agent.remainingDistance / agent.stoppingDistance);

        shouldMove = velocity.magnitude > MovingToIdleMagnitude && agent.remainingDistance > agent.radius;

        animator.SetBool("move", shouldMove);
        animator.SetFloat("velx", velocity.x);
        animator.SetFloat("vely", velocity.y);

        deltaMagnitude = worldDeltaPosition.magnitude;
        if (deltaMagnitude > agent.radius / NavMeshRadiusOffstep)
            transform.position = Vector3.Lerp(animator.rootPosition, agent.nextPosition, smooth);
    }

    private void ConstructBehaviourTreePersonnel()
    {
        GoToActivityNode goToActivityNode = new GoToActivityNode(activityWaypoints, agent, animator, gameObject, idleActivityTimer);
        ChaseNode chaseNode = new ChaseNode(playerTransform, agent, agentCenterTransform);
        RangeNode chasingRangeNode = new RangeNode(chaseRange, playerTransform, agentCenterTransform);
        RangeNode captureRangeNode = new RangeNode(captureRange, playerTransform, agentCenterTransform);
        CaptureNode captureNode = new CaptureNode(agent, playerTransform, captureRange, checkpoint, agentCenterTransform);

        Sequence chaseSequence = new Sequence(new List<Node> { chasingRangeNode, chaseNode });
        Sequence captureSequence = new Sequence(new List<Node> { captureRangeNode, captureNode });

        topNode = new Selector(new List<Node> { captureSequence, chaseSequence, goToActivityNode });
    }

    //accounts for offset between the character- and agent component position that occurs each frame.
    private void OnAnimatorMove()
    {
        Vector3 rootPosition = animator.rootPosition;
        rootPosition.y = agent.nextPosition.y;
        transform.position = rootPosition;
        agent.nextPosition = rootPosition;
    }

    private float radius;
    [SerializeField][Range(0, 360)] private float angle;
    private float corountineDelay = 0.2f;
    private float distanceToTarget;
    private bool playerDetected;

    private Transform target;
    private Vector3 directionToTarget;
    private Collider[] rangeChecks;

    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstacleMask;

    private IEnumerator FOVRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(corountineDelay);
            FOVCheck();
        }
    }

    private void FOVCheck()
    {
        rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);
        if (rangeChecks.Length != 0)
        {
            target = rangeChecks[0].transform;
            directionToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                distanceToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
                    playerDetected = true;         
            }
            else
                playerDetected = false;
        }
        else if (playerDetected)
            playerDetected = false;
    }

    private void OnDrawGizmos()
    {
        if (agentCenterTransform != null)
        {
            if (catchRangeGizmo)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(agentCenterTransform.position, captureRange);
            }
            if (chaseRangeGizmo)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(agentCenterTransform.position, chaseRange);
            }
            if (waypointsGizmo)
            {
                Gizmos.color = Color.cyan;
                for (int i = 0; i < activityWaypoints.Length; i++)
                {
                    if (i + 1 < activityWaypoints.Length)
                     Gizmos.DrawLine(activityWaypoints[i].position, activityWaypoints[i + 1].position);
                    else Gizmos.DrawLine(activityWaypoints[i].position, activityWaypoints[0].position);
                }
            }
        }
        else return;
    }
}
