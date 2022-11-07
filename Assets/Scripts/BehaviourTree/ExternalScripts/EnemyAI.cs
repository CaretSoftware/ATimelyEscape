using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private const float MovingToIdleMagnitude = 0.5f;
    private const float NavMeshRadiusOffstep = 10f;

    [Header("AI Behaviour Input")]
    [SerializeField] [Range(0.0f, 10.0f)] private float idleActivityTimer = 5.0f;
    [SerializeField] private Transform checkpoint;
    [Tooltip("Assigning the same waypoints to multiple enemies may result in unwanted behaviour.")]
    [SerializeField] private Transform[] activityWaypoints;

    private NavMeshAgent agent;
    private Animator animator;
    private Node topNode;

    private EnemyFOV enemyFOV;
    private Transform playerTransform;
    private Transform agentCenterTransform;
    private Vector3 worldDeltaPosition;
    private Vector2 smoothDeltaPosition;
    private Vector2 deltaPosition;
    private Vector2 velocity;

    private float deltaMagnitude;
    private float chaseRange;
    private float captureRange;
    private float smooth;
    private float dx;
    private float dy;
    private bool shouldMove;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        enemyFOV = GetComponent<EnemyFOV>();
        animator.applyRootMotion = true;
        agent.updatePosition = false;
        agent.updateRotation = true;
    }

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        agentCenterTransform = GameObject.Find($"{gameObject.name}/AgentCenterTransform").transform;
        chaseRange = enemyFOV.ChaseRadius;
        captureRange = enemyFOV.CatchRadius;
        ConstructBehaviourTreePersonnel();
    }

    private void Update()
    {
        SynchronizeAnimatorAndAgent();
        topNode.Evaluate();
        if (topNode.nodeState == NodeState.FAILURE)
            agent.isStopped = true;
        CheckOutOfBounds();
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
        ChaseNode chaseNode = new ChaseNode(playerTransform, agent, agentCenterTransform, captureRange);
        RangeNode chasingRangeNode = new RangeNode(chaseRange, playerTransform, agentCenterTransform, enemyFOV);
        RangeNode captureRangeNode = new RangeNode(captureRange, playerTransform, agentCenterTransform, enemyFOV);
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

    public Transform AgentCenterTransform { get { return agentCenterTransform; } private set { agentCenterTransform = value; } }

    float onMeshThreshold = 3;
    NavMeshHit hit;

    //if false, agent is out of bounds of the NavMesh.
    private void CheckOutOfBounds()
    {
        if (NavMesh.SamplePosition(agent.transform.position, out hit, onMeshThreshold, NavMesh.AllAreas))
        {
            if (Mathf.Approximately(agent.transform.position.x, hit.position.x) &&
                Mathf.Approximately(agent.transform.position.z, hit.position.z))
                return;
            else
                agent.transform.position = hit.position;
        }
    }
    private void OnDrawGizmos()
    {
        if (agentCenterTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(agentCenterTransform.position, captureRange);
        }
    }
}
