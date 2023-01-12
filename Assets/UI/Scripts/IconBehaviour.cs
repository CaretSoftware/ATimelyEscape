using System;
using System.Collections;
using System.Collections.Generic;
using NewRatCharacterController;
using UnityEngine;

public class IconBehaviour : MonoBehaviour
{
    
    private float maxDistance = 0;

    private Transform player_;

    private CubePush cubePush;

    [Header("UI")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private IconSwitchHandler[] sides = new IconSwitchHandler[4];

    private float _maxAlpha = 1f;

    
    // Start is called before the first frame update
    private void Start()
    {
        CubePushState.pushCubeUIOn += UIOn;
        
        maxDistance = GetComponent<SphereCollider>().radius * transform.parent.localScale.x;
        cubePush = transform.parent.GetComponent<CubePush>();

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        canvasGroup.alpha = 0;
    }

    private void OnDestroy()
    {
        CubePushState.pushCubeUIOn -= UIOn;
    }

    private void UIOn(bool on) {
        _maxAlpha = on ? .9f : 0f;
    }

    private void FixedUpdate()
    {
        if (player_ != null)
        {
            canvasGroup.alpha = Mathf.Min(_maxAlpha, 1 - (Vector3.Distance(player_.position, transform.position) / maxDistance));
        }
        else
        {
            canvasGroup.alpha = 0;
        }

        if (cubePush.enabled)
            IsCharged(cubePush.Pushable());
        else
            IsCharged(false);
    }

    public void IsCharged(bool isCharged)
    {  
        for (int i = 0; i < sides.Length; i++)
        {
            sides[i].ChangeAnimatorCharge(isCharged);
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
