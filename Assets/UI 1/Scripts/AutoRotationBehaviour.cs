using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotationBehaviour : MonoBehaviour
{

    [SerializeField] private float maxSpeed = 0.5f;
    [SerializeField] private float minSpeed = 0.1f;

    private float currentSpeed;

    private void Awake()
    {
        currentSpeed = Random.Range(minSpeed, maxSpeed);

        if (0 < Random.Range(0, 2))
        {
            currentSpeed = -currentSpeed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, 360 * currentSpeed * Time.deltaTime);
    }
}
