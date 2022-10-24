using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private const float FrameTimeDuration = 1e-5f;
    private const float SpeedMultiplier = 1.0f;
    private const float SmallNumber = 0.025f;

    [SerializeField] private float startingHealth;
    [SerializeField] private float lowHealthThreshold;
    [SerializeField] private float healthRestorationRate;
    [SerializeField] private float chasingRange;
    [SerializeField] private float shootingRange;
    [SerializeField] private float captureRange;
    [SerializeField] private float activityIdleTime = 5.0f;

    [SerializeField] private Transform playerTransform;
    [SerializeField] private Cover[] availableCovers;
    [SerializeField] private Transform[] activityWaypoints;

    public Transform BestCoverSpot { get; set; }
    public float CurrentHealth { get { return _currentHealth; } private set { _currentHealth = Mathf.Clamp(value, 0, startingHealth); } }
    private float _currentHealth;
    public Color Color { get { return material.color; } set { material.color = value; } }

    private NavMeshAgent agent;
    private Material material;
    private Animator animator;
    private Node topNode;

    private Vector3 worldDeltaPosition;
    private Vector3 groundDeltaPosition;
    private Vector2 velocity;
    private bool shouldMove;

    public Transform CurrentActivityPosition { get; set;}

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        material = GetComponent<MeshRenderer>().material;
        animator = GetComponent<Animator>();
        agent.updatePosition = false;
    }
    private void Start()
    {
        CurrentHealth = startingHealth;
        ConstructBehaviourTreePersonnel();
    }


    private void ConstructBehaviourTreePersonnel()
    {
        GoToActivityNode goToActivityNode = new GoToActivityNode(activityWaypoints, agent, animator, activityIdleTime);
        //PerformActivityNode performActivityNode = new PerformActivityNode(agent);
        ChaseNode chaseNode = new ChaseNode(playerTransform, agent, this);
        RangeNode chasingRangeNode = new RangeNode(chasingRange, playerTransform, transform);
        RangeNode captureRangeNode = new RangeNode(captureRange, playerTransform, transform);
        CaptureNode captureNode = new CaptureNode(agent, playerTransform);

        Sequence chaseSequence = new Sequence(new List<Node> {chasingRangeNode, chaseNode});
        Sequence captureSequence = new Sequence(new List<Node> {captureRangeNode, captureNode});

        topNode = new Selector(new List<Node> {captureSequence, chaseSequence, goToActivityNode});
    }


    private void Update()
    {
        ConvertMovementToAnim();
        topNode.Evaluate();
        if (topNode.nodeState == NodeState.FAILURE)
        {
            Color = Color.red;
            agent.isStopped = true;
        }
    }

    //accounts for small offset between the character- and agent component position that occurs each frame.
    private void OnAnimatorMove()
    {
        transform.position = agent.nextPosition;
    }

    private void OnMouseDown()
    {
        CurrentHealth -= 10f;
    }

    private void ConvertMovementToAnim()
    {
        //compare next destination to current position.
        worldDeltaPosition = agent.nextPosition - transform.position;
        //get components in forward/sideways directions.
        groundDeltaPosition.x = Vector3.Dot(transform.right, worldDeltaPosition);
        groundDeltaPosition.y = Vector3.Dot(transform.forward, worldDeltaPosition);

        //divide my duration of frame to get the velocity of which the character should move.
        velocity = (Time.deltaTime > FrameTimeDuration) ? groundDeltaPosition / Time.deltaTime : velocity = Vector2.zero;
        //check if velocity is greater than a small number and if we have not yet arrived at our destination.
        shouldMove = velocity.magnitude > SmallNumber && agent.remainingDistance > agent.radius;

        velocity.x *= SpeedMultiplier;
        velocity.y *= SpeedMultiplier;
        //set anim parameters accoringly.
        animator.SetBool("move", shouldMove);
        animator.SetFloat("velx", velocity.x);
        animator.SetFloat("vely", velocity.y);
    }

    /*
    private void ConstructBehaviourTree()
    {
        IsCoverAvailableNode coverAvailableNode = new IsCoverAvailableNode(availableCovers, playerTransform, this);
        GoToCoverNode goToCoverNode = new GoToCoverNode(agent, this);
        HealthNode healthNode = new HealthNode(this, lowHealthThreshold);
        IsCoveredNode isCoveredNode = new IsCoveredNode(playerTransform, transform);
        ChaseNode chaseNode = new ChaseNode(playerTransform, agent, this);
        RangeNode chasingRangeNode = new RangeNode(chasingRange, playerTransform, transform);
        RangeNode shootingRangeNode = new RangeNode(shootingRange, playerTransform, transform);
        ShootNode shootNode = new ShootNode(agent, this);

        Sequence chaseSequence = new Sequence(new List<Node> {chasingRangeNode, chaseNode});
        Sequence shootSequence = new Sequence(new List<Node> {shootingRangeNode, shootNode});
        
        Sequence goToCoverSequence = new Sequence(new List<Node> {coverAvailableNode, goToCoverNode});
        Selector findCoverSelector = new Selector(new List<Node> {goToCoverSequence, chaseSequence});
        Selector tryToTakeCoverSelector = new Selector(new List<Node> {isCoveredNode, findCoverSelector});
        Sequence mainCoverSequence = new Sequence(new List<Node> {healthNode, tryToTakeCoverSelector});

        topNode = new Selector(new List<Node> {mainCoverSequence, shootSequence, chaseSequence});
    }
    */
}
