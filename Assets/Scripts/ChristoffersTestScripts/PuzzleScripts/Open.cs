using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CallbackSystem;

public class Open : MonoBehaviour
{
    [Header("Open Settings")]
    [SerializeField] private GameObject whatToOpen;
    [Header("Cube Settings")]
    [SerializeField] private bool onlyPast;
    [SerializeField] private bool onlyPresent;
    [SerializeField] private bool onlyFuture;
    private Animator whatToOpenAnim;
    //[SerializeField] private UnityEvent whatToDo;
    private void Start()
    {
        whatToOpenAnim = whatToOpen.GetComponent<Animator>();
        ButtonEvent.AddListener<ButtonEvent>(ButtonOn);
        
    }
    private void OnTriggerEnter(Collider other)
    {
        //whatToDo.Invoke();
        if(other.gameObject.tag == "Cube")
        {
            whatToOpenAnim.SetBool("Open", true);
            ButtonEvent buttonEvent = new ButtonEvent { };
            buttonEvent.Invoke();
            
        }
        if (other.gameObject.tag == "CubePast" && onlyPast)
        {
            whatToOpenAnim.SetBool("Open", true);
        }
        if (other.gameObject.tag == "CubePresent" && onlyPresent)
        {
            whatToOpenAnim.SetBool("Open", true);
        }
        if (other.gameObject.tag == "CubeFuture" && onlyFuture)
        {
            whatToOpenAnim.SetBool("Open", true);
        }
    }
    private void ButtonOn(ButtonEvent e)
    {
        if(e.open = this)
        {

        }
    }
}
