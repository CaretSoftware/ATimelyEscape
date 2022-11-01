using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonCharger : MonoBehaviour
{
    [SerializeField] private GameObject chargerON;
    [SerializeField] private GameObject chargerOFF;

    // Start is called before the first frame update
    void Start()
    {
        chargerON.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Cube")
        {
            chargerON.SetActive(true);
            chargerOFF.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
