using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenKeyPadTest : MonoBehaviour
{

    [SerializeField] private GameObject keypad;

    //[SerializeField] private GameObject keyPadtext;

    //[SerializeField] private bool inReach;

    void Start()
    {
        //inReach = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            //inReach = true;
            keypad.SetActive(true);
            //keyPadtext.SetActive(true);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            //inReach = false;
            keypad.SetActive(false);
            //keyPadtext.SetActive(false);
        }
    }


}
