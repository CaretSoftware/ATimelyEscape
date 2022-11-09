using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconBehaviour : MonoBehaviour
{
    private float maxDistance = 0;

    [SerializeField] private CanvasGroup canvasGroup;

    private Transform player_;

    // Start is called before the first frame update
    private void Start()
    {
        maxDistance = GetComponent<SphereCollider>().radius * transform.parent.localScale.x;

        if(canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        canvasGroup.alpha = 0;
    }

    private void FixedUpdate()
    {
        if (player_ != null)
        {
            canvasGroup.alpha = 1 - (Vector3.Distance(player_.position, transform.position) / maxDistance);
        }
        else
        {
            canvasGroup.alpha = 0;
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
        }
    }
}
