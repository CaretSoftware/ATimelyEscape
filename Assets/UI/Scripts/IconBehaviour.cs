using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CallbackSystem;

public class IconBehaviour : MonoBehaviour
{
    private float maxDistance = 0;

    private Transform player_;

    private CubePush cubePush;

    [Header("UI")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private IconSwitchHandler[] sides = new IconSwitchHandler[4];

    // Start is called before the first frame update
    private void Start()
    {
        CubeIconStateEvent.AddListener<CubeIconStateEvent>(ChangeIcon);

        maxDistance = GetComponent<SphereCollider>().radius * transform.parent.localScale.x;
        cubePush = transform.parent.GetComponent<CubePush>();

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

    private void ChangeIcon(CubeIconStateEvent e)
    {
        for (int i = 0; sides.Length > i; i++)
        {
            if (e.objectCharged)
                sides[i].PlayMovable();
            else
                sides[i].PlayBattery();
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
