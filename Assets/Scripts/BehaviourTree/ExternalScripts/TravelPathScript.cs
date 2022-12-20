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
    private int index;
    private int iterations = 0;


    private bool pathFromFirstIndexPad;

    private void Start()
    {
        line = GetComponent<LineRenderer>();
        positions = new Vector3[line.positionCount];
        pos = GetLinePointsInWorldSpace();
        objectToMove.SetActive(false);
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

        this.teleportingObject.transform.Find("Rat").gameObject.SetActive(false);
        if (this.teleportingObject.tag.Equals("Cube"))
            this.teleportingObject.gameObject.SetActive(false);
        
        objectToMove.SetActive(true);
        objectToMove.transform.position = pos[index];
    }

    private Vector3 location;
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
            teleportingObject.transform.position = objectToMove.transform.position;
            index = pathFromFirstIndexPad ? ++index : --index; 
            iterations++;
        }
        if (iterations == positions.Length)
        {
            teleportingObject.transform.position = location;
            objectToMove.SetActive(false);
            teleportingObject.transform.Find("Rat").gameObject.SetActive(true);
            if (teleportingObject.tag.Equals("Cube"))
                teleportingObject.gameObject.SetActive(true);
            
        }
    }
}
