using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] private float speed;
    private Vector3 rotation;

    private void Start()
    {
        rotation = Vector3.down;
    }
    void Update()
    {
        transform.Rotate(rotation * speed * Time.deltaTime);
    }
}
