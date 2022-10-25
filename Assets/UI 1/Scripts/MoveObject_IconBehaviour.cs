using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject_IconBehaviour : MonoBehaviour
{
    private float maxDistance = 0;

    private CanvasGroup canvasGroup;

    [SerializeField] private Transform player_;

    // Start is called before the first frame update
    void Start()
    {
        maxDistance = gameObject.GetComponent<SphereCollider>().radius;
        canvasGroup = gameObject.GetComponent<CanvasGroup>();

        canvasGroup.alpha = 0;
    }

    private void FixedUpdate()
    {
        if (player_ != null)
        {
            canvasGroup.alpha = 1 - (Vector3.Distance(player_.position, gameObject.transform.position) / maxDistance);
        }
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player_ = other.gameObject.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player_ = null;
            canvasGroup.alpha = 0;
        }
    }
    
}
