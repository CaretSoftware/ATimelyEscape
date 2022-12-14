using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorForce : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float speedMultiplier;
    private float cubeSpeedMultiplyer = 150f; 
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<Rigidbody>().AddForce((transform.forward * speed) * Time.deltaTime, ForceMode.Impulse);
        }
        if(other.gameObject.tag == "Cube")
        {
            other.gameObject.GetComponent<Rigidbody>().AddForce((transform.forward * ((speed * speedMultiplier) * cubeSpeedMultiplyer)) * Time.deltaTime, ForceMode.Impulse);
        }
        else
        {
            other.gameObject.GetComponent<Rigidbody>().AddForce((transform.forward * (speed * speedMultiplier)) * Time.deltaTime, ForceMode.Impulse);
        }
    }
}
