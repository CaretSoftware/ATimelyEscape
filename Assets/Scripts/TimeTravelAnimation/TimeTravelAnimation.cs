using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CallbackSystem;
using RatCharacterController;

public class TimeTravelAnimation : MonoBehaviour
{
    [SerializeField] private GameObject ratTimeTravelMesh;
    private Transform playerTransform;
    private SkinnedMeshRenderer meshRenderer;
    private bool playing;

    private void Start()
    { 
        playerTransform = FindObjectOfType<CharacterAnimationController>().transform;
        meshRenderer = playerTransform.GetComponentInChildren<SkinnedMeshRenderer>();
        TimePeriodChanged.AddListener<TimePeriodChanged>(PlayTimeTravelEffect);
    }

    private void PlayTimeTravelEffect(TimePeriodChanged e)
    {
        if (!playing)
        {
            meshRenderer.enabled = false;
            GameObject go = Instantiate(ratTimeTravelMesh);
            go.transform.position = playerTransform.position;
            go.transform.rotation = playerTransform.rotation;
            playing = true;
            StartCoroutine(Wait(go));
        }
    }

    private IEnumerator Wait(GameObject go)
    {
        yield return new WaitForSecondsRealtime(2f);
        Destroy(go);
        playing = false;
        meshRenderer.enabled = true;
    }
}
