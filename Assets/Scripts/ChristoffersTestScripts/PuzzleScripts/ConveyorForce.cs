using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorForce : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float speedMultiplier;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MeshRenderer meshRenderer2;
    private MaterialPropertyBlock _matPropBlock;
    private float cubeSpeedMultiplyer = 150f;
    private bool isOn;

    void Awake()
    {
        _matPropBlock = new MaterialPropertyBlock();
    }

    private void Start()
    {
        isOn = true;
        

    }
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

        if (_matPropBlock != null)
        {
            _matPropBlock.SetFloat("_Scrolling_Time_X", 0f);

            // Apply the edited values to the renderer.
            meshRenderer.SetPropertyBlock(_matPropBlock);
        }
        if (_matPropBlock != null)
        {
            _matPropBlock.SetFloat("_Scrolling_Time_X", 0f);

            // Apply the edited values to the renderer.
            meshRenderer2.SetPropertyBlock(_matPropBlock);
        }
    }


}
