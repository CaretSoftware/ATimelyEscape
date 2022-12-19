using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuckUP : MonoBehaviour
{
    [SerializeField] private Transform suckPosition;
    [SerializeField] private GameObject suckArea;
    [SerializeField] private float suckSpeed;
    private bool buttonOneOn;
    private bool buttonTwoOn;
    private bool hasStarted;
    private float spinX;
    private float spinY;
    private float spinZ;
    private Vector3 suckDirection;
    private Animator animator;
    private ParticleSystem ps;
    private void Awake()
    {
        suckArea.SetActive(false);
    }
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
            //StartCoroutine(Suck());
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
            suckDirection = suckPosition.position - other.gameObject.transform.position;
            spinX = Random.Range(50, 100) * Time.deltaTime;
            spinY = Random.Range(20, 150) * Time.deltaTime;
            spinZ = Random.Range(50, 300) * Time.deltaTime;
            other.gameObject.transform.Rotate(spinX, spinY, spinZ, Space.Self);
            rb.AddForce((suckDirection * suckSpeed) * Time.deltaTime);
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
        //yield return new WaitForSeconds(1.3f);
        ps.Play();
        suckArea.SetActive(true);
        yield return new WaitForSeconds(4.0f);
        ps.Stop();
        //animator.SetBool("Stop", true);
    }



}
