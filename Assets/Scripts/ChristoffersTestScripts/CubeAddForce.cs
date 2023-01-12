using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeAddForce : MonoBehaviour
{
    [SerializeField] private Transform goal;
    [SerializeField] private float speed;
    private Vector3 moveDirection;
    private CubePush cubePush;
    private bool isOn;
    private float counter;

    private void Start()
    {
        cubePush = GetComponent<CubePush>();
    }

    private void FixedUpdate()
    {
        if (isOn)
        {
            if (counter < 10)
            {
                moveDirection = goal.position - gameObject.transform.position;
                cubePush.Push((moveDirection * speed));
                counter += 1; 
            }
        }
    }

    public void ForceCube()
    {
        Invoke("On", 0.03f);
    }
    private void On()
    {
        isOn = true;
    }

}
