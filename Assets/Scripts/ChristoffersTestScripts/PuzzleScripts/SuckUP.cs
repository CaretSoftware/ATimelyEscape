using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SuckUP : MonoBehaviour
{
    [SerializeField] private Transform suckPosition;
    [SerializeField] private float suckSpeed;
    [SerializeField] private bool buttonOneOn;
    [SerializeField] private bool buttonTwoOn;
    private bool hasStarted;
    private float spinX;
    private float spinY;
    private float spinZ;
    private Vector3 suckDirection;
    private Animator animator;
    private ParticleSystem ps;
    private NavMeshAgent roombaNav; 

    private void Start()
    {
        animator = GetComponentInParent<Animator>();
        ps = GetComponentInChildren<ParticleSystem>();
    }
    private void Update()
    {
        if (buttonOneOn && buttonTwoOn && !hasStarted)
        {
            hasStarted = true;
            animator.SetBool("Start", true);
            StartCoroutine(Suck());
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Roomba")
        {
          /*  roombaNav = other.gameObject.GetComponent<NavMeshAgent>();
            
            Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
            other.gameObject.GetComponent<PatrolNavAgent>().enabled = false;
            //Invoke("TurnOffNav", 0.5f);
            suckDirection = suckPosition.position - other.gameObject.transform.position;
            spinX = Random.Range(50, 100) * Time.deltaTime;
            spinY = Random.Range(20, 150) * Time.deltaTime;
            spinZ = Random.Range(50, 300) * Time.deltaTime;
            other.gameObject.transform.Rotate(spinX, spinY, spinZ, Space.Self);
            rb.AddForce((suckDirection * suckSpeed) * Time.deltaTime);*/
            Destroy(other.gameObject);
        }
    }

    public void ButtonOneOn()
    {
        buttonOneOn = true;
    }
    public void ButtonTwoOn()
    {
        buttonTwoOn = true;
    }
    private IEnumerator Suck()
    {
        Debug.Log("Här");
        yield return new WaitForSeconds(1.3f);
        ps.Play();
        yield return new WaitForSeconds(4.0f);
        ps.Stop();
        
    }
 /*   private void TurnOffNav()
    {
        roombaNav.enabled = false;
    }*/



}
