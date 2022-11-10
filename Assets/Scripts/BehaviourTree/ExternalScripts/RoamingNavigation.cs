using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(MeshCollider))]
public class RoamingNavigation : MonoBehaviour
{
    private const float sight = 100.0f;

    [SerializeField] private float movementSpeed = 5.0f;
    [SerializeField] private float rotationSpeed = 4.0f;
    [SerializeField] private float visionAngle = 70.0f;
    [SerializeField] [Range(3, 11)] private int traces = 3;

    private List<Collider> detectedColliders;
    private MeshCollider col;
    private RaycastHit hitInfo;
    private Mesh mesh;

    private Vector3 movementVector;
    private Vector3 direction;
    private Vector3 offset;
    private Vector3 perp;

    private float angle;
    private float radius;
    private float distance;
    private float stepAngle;

    [SerializeField] private bool debugDraw;

    void Update()
    {
        CalculateMovementVector();
        LerpRotation();
        ResolveCollisions();
    }

    private void Start()
    {
        col = GetComponent<MeshCollider>();
        mesh = GetComponent<MeshFilter>().mesh;

        radius = Vector3.Distance(transform.position, mesh.vertices[0]);
        offset = mesh.bounds.size.y * 0.5f * Vector3.up;
    }

    public void CalculateMovementVector()
    {
        movementVector = Vector3.zero;
        for (int i = 0; i < traces; i++)
        {
            stepAngle = (visionAngle * 2.0f) / (traces - 1);
            angle = (90.0f + visionAngle - (i * stepAngle)) * Mathf.Deg2Rad;
            direction = transform.TransformDirection(new Vector3(Mathf.Cos(angle), 0.0f, Mathf.Sin(angle)));

            if (debugDraw)
            {
                Debug.DrawLine(transform.position, transform.position + direction * hitInfo.distance);

                perp = Vector3.Cross(direction, Vector3.up);
                Debug.DrawLine(hitInfo.point + perp, hitInfo.point - perp, Color.red);
            }

            if (Physics.Raycast(transform.position, direction, out hitInfo, sight))
                movementVector += direction * hitInfo.distance;
            else
            {
                movementVector += direction * sight;
                if (debugDraw)
                    Debug.DrawLine(transform.position, transform.position + direction * sight);
            }

        }
    }

    private void LerpRotation()
    {
        transform.forward = Vector3.Lerp(transform.forward, movementVector.normalized, Time.deltaTime * rotationSpeed);
        transform.position += transform.forward * movementSpeed * Time.deltaTime;
    }

    public void ResolveCollisions()
    {
        detectedColliders = Physics.OverlapCapsule(
            transform.position + offset,
            transform.position - offset,
            radius)
            .Where(c => c.transform != transform)
            .ToList();

        if (detectedColliders.Count > 0)
        {
            if (Physics.ComputePenetration(
                col,
                transform.position,
                transform.rotation,
                detectedColliders[0],
                detectedColliders[0].transform.position,
                detectedColliders[0].transform.rotation,
                out direction,
                out distance))
            {
                transform.position += direction * distance;
            }
        }
    }
}