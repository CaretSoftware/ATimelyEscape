using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CallbackSystem;

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
        TimePeriodChanged.AddListener<TimePeriodChanged>(TimeMachineOff);
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
    private void TimeMachineOff(TimePeriodChanged e)
    {
        if (e.from == TimeTravelPeriod.Past && e.to == TimeTravelPeriod.Present)
        {
            isOn = false;
        }
        else if (e.from == TimeTravelPeriod.Past && e.to == TimeTravelPeriod.Future)
        {
            isOn = false;
        }
        else if (e.from == TimeTravelPeriod.Future && e.to == TimeTravelPeriod.Past)
        {
            isOn = true;
        }
        else if (e.from == TimeTravelPeriod.Present && e.to == TimeTravelPeriod.Past)
        {
            isOn = true;
        }
    }
    private void OnDestroy()
    {
        if (EventSystem.Current != null) TimePeriodChanged.RemoveListener<TimePeriodChanged>(TimeMachineOff);
    }


}
