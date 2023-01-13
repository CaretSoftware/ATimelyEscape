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
    [SerializeField] private GameObject[] dummyRoombas;
    private bool hasStarted;
    
/*    private float spinX;
    private float spinY;
    private float spinZ;
    private Vector3 suckDirection;*/
    private Animator animator;
    private ParticleSystem ps;
    //private NavMeshAgent roombaNav;
    //private Transform[] roombaPos;

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
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Roomba")
        {
/*            for (int i = 0; i < roombaPos.Length; i++)
            {
                roombaPos[i].position = other.gameObject.transform.position;
            }*/
            /*            roombaNav = other.gameObject.GetComponent<NavMeshAgent>();

                        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
                        other.gameObject.GetComponent<PatrolNavAgent>().enabled = false;*/
            //Invoke("TurnOffNav", 0.5f);
/*            Invoke("FakeRoombasGo", 0.1f);
            suckDirection = suckPosition.position - other.gameObject.transform.position;*/
            Destroy(other.gameObject);
        }
    }
    private void OnTriggerStay(Collider other)
    {
/*        if (other.gameObject.tag == "DummyRoomba")
        {

            Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
            spinX = Random.Range(50, 100) * Time.deltaTime;
            spinY = Random.Range(20, 150) * Time.deltaTime;
            spinZ = Random.Range(50, 300) * Time.deltaTime;
            other.gameObject.transform.Rotate(spinX, spinY, spinZ, Space.Self);
            rb.AddForce((suckDirection * suckSpeed) * Time.deltaTime);

        }*/
    }
/*    private void FakeRoombasGo()
    {
        for (int i = 0; i < roombaPos.Length; i++)
        {
        Instantiate(dummyRoomba, roombaPos[i].position, roombaPos[i].rotation);
        }
    }*/
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
        yield return new WaitForSeconds(1.3f);
        ps.Play();
        for (int i = 0; i < dummyRoombas.Length; i++)
        {
            dummyRoombas[i].GetComponent<Animator>().SetBool("On", true);
        }
        yield return new WaitForSeconds(4.0f);
        ps.Stop();

    }

    /*   private void TurnOffNav()
       {
           roombaNav.enabled = false;
       }*/



}
