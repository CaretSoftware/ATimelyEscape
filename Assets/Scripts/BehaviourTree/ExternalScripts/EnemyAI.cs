using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class EnemyAI : MonoBehaviour
{
    private const float MovingToIdleMagnitude = 0.5f;
    private const float NavMeshRadiusOffstep = 20f;

    [HideInInspector] public static int IDCounter;
    [HideInInspector] public int ID;

    [Header("AI Behaviour Input")]
    [SerializeField] [Range(0.0f, 10.0f)] private float idleActivityTimer = 5.0f;
    [SerializeField] private Transform checkpoint;
    [Tooltip("Assigning the same waypoints to multiple enemies may result in unwanted behaviour.")]
    [SerializeField] private Transform[] activityWaypoints;

    [Header("Rig Setup")]
    [SerializeField] private Transform handIKTarget;

    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public Animator animator;
    [HideInInspector] public bool activeAI;

    private MultiAimConstraint multiAimConstraint;
    private ChainIKConstraint chainIKConstraint;
    private GameObject fullBodyRig;
    private EnemyFOV enemyFOV;
    private NavMeshHit hit;
    private Node topNode;

    private Transform playerTransform;
    private Transform agentCenterTransform;
    private Vector3 worldDeltaPosition;
    private Vector3 rootPosition;
    private Vector2 smoothDeltaPosition;
    private Vector2 deltaPosition;
    private Vector2 velocity;

    private float deltaMagnitude;
    private float chaseRange;
    private float captureRange;
    private float onMeshThreshold = 3;
    private float smooth;
    private float dx;
    private float dy;

    private bool shouldMove;

    public Transform AgentCenterTransform { get { return agentCenterTransform; } private set { agentCenterTransform = value; } }

    private void Awake()
    {
         // is always false if(ID == null) ID = IDCounter++;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        enemyFOV = GetComponent<EnemyFOV>();
        fullBodyRig = GameObject.Find("FullBodyRig").transform.GetChild(0).gameObject;
        chainIKConstraint = fullBodyRig.GetComponent<ChainIKConstraint>();
        multiAimConstraint = fullBodyRig.GetComponent<MultiAimConstraint>();
        animator.applyRootMotion = true;
        agent.updatePosition = false;
        agent.updateRotation = true;
    }

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        agentCenterTransform = GameObject.Find($"{gameObject.name}/AgentCenterTransform").transform;
        chainIKConstraint.weight = 0;
        multiAimConstraint.weight = 0;
        chaseRange = enemyFOV.ChaseRadius;
        captureRange = enemyFOV.CatchRadius;
        ConstructBehaviourTreePersonnel();
    }

    private void Update()
    {
        if (activeAI)
        {
            SynchronizeAnimatorAndAgent();
            topNode.Evaluate();
            if (topNode.nodeState == NodeState.FAILURE)
                agent.isStopped = true;
            CheckOutOfBounds();
        }
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
        ChaseNode chaseNode = new ChaseNode(playerTransform, agent, agentCenterTransform, captureRange, multiAimConstraint);
        RangeNode chasingRangeNode = new RangeNode(chaseRange, playerTransform, agentCenterTransform, enemyFOV);
        RangeNode captureRangeNode = new RangeNode(captureRange, playerTransform, agentCenterTransform, enemyFOV);
        CaptureNode captureNode = new CaptureNode(agent, playerTransform, captureRange, checkpoint, agentCenterTransform, handIKTarget, animator, chainIKConstraint);

        Sequence chaseSequence = new Sequence(new List<Node> { chasingRangeNode, chaseNode });
        Sequence captureSequence = new Sequence(new List<Node> { captureRangeNode, captureNode });

        topNode = new Selector(new List<Node> { captureSequence, chaseSequence, goToActivityNode });
    }

    //accounts for offset between the character- and agent component position that occurs each frame.
    private void OnAnimatorMove()
    {
        rootPosition = animator.rootPosition;
        rootPosition.y = agent.nextPosition.y;
        transform.position = rootPosition;
        agent.nextPosition = rootPosition;
    }

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

    public void SetPlayerTransformToCheckpoint()
    {
        playerTransform.position = checkpoint.position;
        animator.SetTrigger("ReturnHandAction");

    }

    private void OnDrawGizmos()
    {
        if (agentCenterTransform != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(agentCenterTransform.position, captureRange);
        }
    }
}
