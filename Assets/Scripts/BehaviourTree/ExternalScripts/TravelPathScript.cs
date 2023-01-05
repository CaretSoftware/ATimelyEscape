using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class TravelPathScript : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private GameObject objectToMove;
    private LineRenderer line;
    private GameObject teleportingObject;
    private NavMeshHit hit;

    private Vector3[] positions;
    private Vector3[] pos;
    private Vector3 location;
    private int index;
    private int iterations = 0;
    private NewRatCharacterController.NewRatCharacterController playerParent;

    private bool pathFromFirstIndexPad;

    private void Start()
    {
        line = GetComponent<LineRenderer>();
        positions = new Vector3[line.positionCount];
        pos = GetLinePointsInWorldSpace();
        objectToMove.SetActive(false);
        playerParent = FindObjectOfType<NewRatCharacterController.NewRatCharacterController>();

    }

    private void Update()
    {
        if (iterations < positions.Length && objectToMove.activeInHierarchy)
            Move();
    }

    public void TravelPath(bool pathReg, GameObject teleportingObject, Vector3 arrivalLocation)
    {
        this.teleportingObject = teleportingObject;
        pathFromFirstIndexPad = pathReg;
        location = arrivalLocation;
        index = pathReg ? 0 : pos.Length - 1;
        iterations = 0;

        if(this.teleportingObject.tag.Equals("Player"))
            this.teleportingObject.transform.Find("Rat").gameObject.SetActive(false);
        else if (this.teleportingObject.tag.Equals("Cube"))
        {
            this.teleportingObject.gameObject.SetActive(false);
            playerParent.transform.Find("Rat").gameObject.SetActive(false);
            playerParent.LetGoOfCube = true;

        }
            
        NewRatCharacterController.NewRatCharacterController.caughtEvent?.Invoke(true);
        objectToMove.SetActive(true);
        objectToMove.transform.position = pos[index];
    }

    private Vector3[] GetLinePointsInWorldSpace()
    {
        line.GetPositions(positions);
        return positions;
    }
    
    private void Move()
    {
        objectToMove.transform.position = Vector3.MoveTowards(objectToMove.transform.position, pos[index], speed * Time.deltaTime);
        if (objectToMove.transform.position == pos[index])
        {
            playerParent.transform.position = objectToMove.transform.position;
            index = pathFromFirstIndexPad ? ++index : --index; 
            iterations++;
        }
        if (iterations == positions.Length)
        {
            objectToMove.SetActive(false);
            NewRatCharacterController.NewRatCharacterController.caughtEvent?.Invoke(false);
            teleportingObject.transform.position = location;
            if(teleportingObject.tag.Equals("Player"))
                teleportingObject.transform.Find("Rat").gameObject.SetActive(true);
            else if (teleportingObject.tag.Equals("Cube"))
            {
                teleportingObject.gameObject.SetActive(true);
                playerParent.transform.Find("Rat").gameObject.SetActive(true);
                playerParent.transform.position = location + (Vector3.left / 5);
            }
        }
    }
}
