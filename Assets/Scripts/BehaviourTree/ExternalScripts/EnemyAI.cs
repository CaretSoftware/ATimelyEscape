using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float chaseRange;
    [SerializeField] private float captureRange;
    [SerializeField] private float idleActivityTimer = 5.0f;
    [SerializeField] private float movingToIdleMagnitude = 0.5f;

    [SerializeField] private GameOverScreen gameOverScreen;
    [SerializeField] private Transform[] activityWaypoints;
    [SerializeField] private Transform agentCenterTransform;

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

    public Transform CurrentActivityPosition { get; set;}

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
        playerTransform = FindObjectOfType<CharacterController>().transform;
        agentCenterTransform = 
            Instantiate<GameObject>(
                new GameObject(), 
                new Vector3(transform.position.x, transform.position.y + agent.height / 2, transform.position.z), 
                Quaternion.identity, 
                gameObject.transform).transform;

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

        smooth = Mathf.Min(1, Time.deltaTime / 0.1f);
        smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);
        velocity = smoothDeltaPosition / Time.deltaTime;

        if(agent.remainingDistance <= agent.stoppingDistance)
            velocity = Vector2.Lerp(Vector2.zero, velocity, agent.remainingDistance / agent.stoppingDistance);

        shouldMove = velocity.magnitude > movingToIdleMagnitude && agent.remainingDistance > agent.radius;

        animator.SetBool("move", shouldMove);
        animator.SetFloat("velx", velocity.x);
        animator.SetFloat("vely", velocity.y);

        deltaMagnitude = worldDeltaPosition.magnitude;
        if (deltaMagnitude > agent.radius / 2f)
            transform.position = Vector3.Lerp(animator.rootPosition, agent.nextPosition, smooth);      
    }

    private void ConstructBehaviourTreePersonnel()
    {
        GoToActivityNode goToActivityNode = new GoToActivityNode(activityWaypoints, agent, animator, idleActivityTimer);
        ChaseNode chaseNode = new ChaseNode(playerTransform, agent, agentCenterTransform);
        RangeNode chasingRangeNode = new RangeNode(chaseRange, playerTransform, agentCenterTransform);
        RangeNode captureRangeNode = new RangeNode(captureRange, playerTransform, agentCenterTransform);
        CaptureNode captureNode = new CaptureNode(agent, playerTransform, captureRange, gameOverScreen, agentCenterTransform);

        Sequence chaseSequence = new Sequence(new List<Node> { chasingRangeNode, chaseNode });
        Sequence captureSequence = new Sequence(new List<Node> { captureRangeNode, captureNode });

        topNode = new Selector(new List<Node> { captureSequence, chaseSequence, goToActivityNode });
    }

    //accounts for small offset between the character- and agent component position that occurs each frame.
    private void OnAnimatorMove()
    {
        Vector3 rootPosition = animator.rootPosition;
        rootPosition.y = agent.nextPosition.y;
        transform.position = rootPosition;
        agent.nextPosition = rootPosition;
    }
}
