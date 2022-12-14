using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorForce : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float speedMultiplier;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material offMaterial;
    private float cubeSpeedMultiplyer = 150f;
    private bool isOn = true;
    private void OnTriggerStay(Collider other)
    {
        if (isOn)
        {
            if (other.gameObject.tag == "Player")
            {
                other.gameObject.GetComponent<Rigidbody>().AddForce((transform.forward * speed) * Time.deltaTime, ForceMode.Impulse);
            }
            else if (other.gameObject.tag == "Cube")
            {
                other.gameObject.GetComponent<Rigidbody>().AddForce((transform.forward * ((speed * speedMultiplier) * cubeSpeedMultiplyer)) * Time.deltaTime, ForceMode.Impulse);
            }
            else
            {
                other.gameObject.GetComponent<Rigidbody>().AddForce((transform.forward * (speed * speedMultiplier)) * Time.deltaTime, ForceMode.Impulse);
            }
        }
    }

    public void TurnOff()
    {
        isOn = false;
        if (meshRenderer != null)
        {
            Material[] materials = meshRenderer.sharedMaterials;
            materials[1] = offMaterial;
            meshRenderer.material = offMaterial;
            meshRenderer.sharedMaterials = materials;
        }
    }
}
