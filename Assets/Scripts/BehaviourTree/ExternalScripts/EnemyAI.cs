using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;

public class EnemyAI : MonoBehaviour
{
    private const float MovingToIdleMagnitude = 0.5f;
    private const float NavMeshRadiusOffstep = 20f;
    private const float AnimationPreviewBasedFeetPos = 0.285f;
    private const float AnimationPreviewBasedHipPos = 0.5f;

    [HideInInspector] public static int IDCounter;
    [HideInInspector] public int ID;

    [Header("AI Behaviour Input")]
    [SerializeField] [Range(0.0f, 10.0f)] private float idleActivityTimer = 5.0f;
    [SerializeField] private Transform checkpoint;
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private Collider[] collidersToIgnorePlayer;

    [Tooltip("Assigning the same waypoints to multiple enemies may result in unwanted behaviour.")]
    [SerializeField] private Transform[] activityWaypoints;

    [Header("Rig Setup")]
    [SerializeField] private Transform handIKTarget;
    [SerializeField] private Transform feetPos;
    [SerializeField] private Transform hipPos;
    [SerializeField] private Transform agentCenterTransform;
    [SerializeField] private GameObject fullBodyRig;
    [SerializeField] private Transform losPos;

    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public Animator animator;
    [HideInInspector] public bool activeAI;
    
    private ChainIKConstraint chainIKConstraint;
    private Collider playerCollider;
    private EnemyFOV enemyFOV;
    private Node topNode;
    
    private Transform defaultIKTarget;
    private Transform playerTransform;

    private NavMeshHit hit;
    private Vector3 worldDeltaPosition;
    private Vector3 rootPosition;
    private Vector2 smoothDeltaPosition;
    private Vector2 deltaPosition;
    private Vector2 velocity;
    private Vector3 defaultFeetPos;
    private Vector3 defaultHipPos;
    private Vector3 feetPosBent;
    private Vector3 hipPosBent;

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
        playerCollider = playerTransform.GetComponent<Collider>();
        AssignCollidersIgnore();

        chainIKConstraint.weight = 0;
        defaultIKTarget = handIKTarget;
        chaseRange = enemyFOV.ChaseRadius;
        captureRange = enemyFOV.CatchRadius;
        ConstructBehaviourTreePersonnel();

        defaultFeetPos = feetPos.localPosition;
        defaultHipPos = hipPos.localPosition;
        hipPosBent = new Vector3(defaultHipPos.x, AnimationPreviewBasedHipPos ,defaultHipPos.z);
        feetPosBent = new Vector3(defaultFeetPos.x, AnimationPreviewBasedFeetPos ,defaultFeetPos.z);
    }
    
    private void AssignCollidersIgnore()
    {
        for (int i = 0; i < collidersToIgnorePlayer.Length; i++)
            Physics.IgnoreCollision(playerCollider, collidersToIgnorePlayer[i]);
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
        RangeNode chaseRangeNode = new RangeNode(chaseRange, agentCenterTransform, playerTransform, enemyFOV, animator);
        ChaseNode chaseNode = new ChaseNode(playerTransform, agent, agentCenterTransform, captureRange, chaseRange);
        RangeNode captureRangeNode = new RangeNode(captureRange, agentCenterTransform, playerTransform, enemyFOV, animator);
        CaptureNode captureAttemptNode = new CaptureNode(playerTransform, this, losPos.position, playerLayerMask);
        CaptureAnimationNode captureAnimationNode = new CaptureAnimationNode(playerTransform, agentCenterTransform, animator, captureRange);
        InvertedRangeNode invertedRangeNode = new InvertedRangeNode(captureRange, agentCenterTransform, playerTransform, animator, this);

        Sequence captureSequence = new Sequence(new List<Node> { captureRangeNode, captureAttemptNode, captureAnimationNode });
        Sequence chaseSequence = new Sequence(new List<Node> { chaseRangeNode, chaseNode });
        Selector enemyActivitySelector = new Selector(new List<Node> { chaseSequence, goToActivityNode });
        Sequence enemyActivityMainSequence = new Sequence(new List<Node> { invertedRangeNode, enemyActivitySelector });

        topNode = new Selector(new List<Node> { enemyActivityMainSequence, captureSequence });
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
        else
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, captureRange);
        }

        if (withinReach)
        {
            Gizmos.color = drawLOS ? Color.blue : Color.red;
            Gizmos.DrawLine(losPos.position, playerTransform.position);
        }
    }

    private bool drawLOS;
    private bool withinReach;
    public void DrawLOS(bool arg)
    {
        withinReach = true;
        print($"losPos: {drawLOS}");
        drawLOS = arg;
    }
    //Animation event methods.
    public void ResetAfterAnimations()
    {
        handIKTarget.position = defaultIKTarget.position;
        RestoreAfterKneeling();

        animator.SetBool("GrabActionBool", false);
        isCapturing = false;
    }

    public void SetPlayerTransformToCheckpoint()
    {
        animator.SetBool("DeathAnimationPlaying", true);
        FailStateScript.Instance.PlayDeathVisualization(checkpoint, transform);
    }
    
    public void StartReaching()
    {
        isCapturing = true;
    }
    
    public void StopReaching()
    {
        animator.SetBool("DeathAnimationPlaying", false);
    }
    
    public bool IsCapturing
    {
        get { return isCapturing; }
        set { isCapturing = value; }
    }

    

    public void BendTheKnee()
    {
        if (playerTransform.position.y < 0.5f)
        {
            feetPos.localPosition = feetPosBent;
            hipPos.localPosition = hipPosBent;
        }
    }

    private void RestoreAfterKneeling()
    {
        feetPos.localPosition = defaultFeetPos;
        hipPos.localPosition = defaultHipPos;

    }
}
