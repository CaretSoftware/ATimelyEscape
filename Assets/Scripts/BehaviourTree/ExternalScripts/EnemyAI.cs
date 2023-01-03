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

    private ChainIKConstraint chainIKConstraint;
    [SerializeField] private GameObject fullBodyRig;
    private EnemyFOV enemyFOV;
    private NavMeshHit hit;
    private Node topNode;

    private Transform playerTransform;
    [SerializeField] private Transform agentCenterTransform;
    private Transform defaultIKTarget;
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
    public static bool isCapturing;
    private bool isAnimationRunning;
    
    public float ChaseRange
    {
        get { return chaseRange;}
        set { chaseRange = value; }
    }
    
    public float CaptureRange
    {
        get { return captureRange; }
        set { captureRange = value; }
    }
    
    public float IdleActivityTime
    {
        get { return idleActivityTimer; }
        set { idleActivityTimer = value; }
    }

    public Transform AgentCenterTransform
    {
        get { return agentCenterTransform; }
    }

    private void Awake()
    {
         // is always false if(ID == null) ID = IDCounter++;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        enemyFOV = GetComponent<EnemyFOV>();
        chainIKConstraint = fullBodyRig.GetComponent<ChainIKConstraint>();
        animator.applyRootMotion = true;
        agent.updatePosition = false;
        agent.updateRotation = true;
    }

    private void Start()
    {
        playerTransform = FindObjectOfType<NewRatCharacterController.NewRatCharacterController>().transform;
        chainIKConstraint.weight = 0;
        defaultIKTarget = handIKTarget;
        chaseRange = enemyFOV.ChaseRadius;
        captureRange = enemyFOV.CatchRadius;
        ConstructBehaviourTreePersonnel();
    }

    private void Update()
    {
        if (activeAI)
        {
            handIKTarget.position = isCapturing ? playerTransform.position : defaultIKTarget.position;
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
        ChaseNode chaseNode = new ChaseNode(playerTransform, agent, agentCenterTransform, captureRange);
        RangeNode chasingRangeNode = new RangeNode(chaseRange, playerTransform, agentCenterTransform, enemyFOV);
        RangeNode captureRangeNode = new RangeNode(captureRange, playerTransform, agentCenterTransform, enemyFOV);
        CaptureNode captureNode = new CaptureNode(agent, playerTransform, captureRange, agentCenterTransform, animator, this);

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
    
    private void OnDrawGizmos()
    {
        if (agentCenterTransform != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(agentCenterTransform.position, captureRange);
        }
    }

    //Animation event methods.
    public void ResetAfterAnimations()
    {
        handIKTarget.position = defaultIKTarget.position;
        isCapturing = false;
    }
    public void SetPlayerTransformToCheckpoint() { FailStateScript.Instance.PlayDeathVisualization(checkpoint, transform); }
    public void ReturnHand() { animator.SetTrigger("ReturnHandAction"); }
    public void StartReaching()
    {
        isCapturing = true;
    } 
    public bool IsCapturing
    {
        get { return isCapturing; }
        set { isCapturing = value; }
    }
}
