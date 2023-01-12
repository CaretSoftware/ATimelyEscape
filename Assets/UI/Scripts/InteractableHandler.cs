using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CallbackSystem;
using RatCharacterController;

public class InteractableHandler : MonoBehaviour
{
    private Transform cameraTransform;

    [SerializeField] private string keypadTag;
    private LayerMask cubeLayerMask;

    private bool showClick;
    private Vector3 interactVector;
    private Transform cubeTransform;

    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = CameraController.Cam;
        cubeLayerMask = LayerMask.GetMask("Cube");

        cubeTransform = FindObjectOfType<Transform>();
        interactVector = Vector3.zero;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!CharacterInput.Paused())
            IsInteractable();
    }

    private void IsInteractable()
    {
        Transform playerTransform = transform;
        Ray ray = new Ray(transform.position + Vector3.up * GetComponent<CapsuleCollider>().height / 2 * playerTransform.localScale.y,
           playerTransform.forward);

        Ray cameraRay = new Ray(cameraTransform.position, cameraTransform.forward);

        if (GetComponent<CharacterInput>().IsPushing())
            return;

        if ((Vector3.Distance(interactVector, transform.position) > 0.1f) && Physics.Raycast(ray, out RaycastHit pad, 0.2f) && pad.transform.CompareTag(keypadTag))
        {
            interactVector = pad.point;
            CallHintAnimation call = new CallHintAnimation() { animationName = "LeftClick"};
            call.Invoke();
        }
        else if ((Vector3.Distance(cubeTransform.position, transform.position) > 0.1f && Physics.Raycast(ray, out RaycastHit cubeInfo, 0.3f, cubeLayerMask)))
        {
            cubeTransform = cubeInfo.transform;

            if (cubeInfo.transform.GetComponent<CubePush>().Pushable())
            {
                CallHintAnimation call = new CallHintAnimation() { animationName = "LeftClick"};
                call.Invoke();
            }
            else if (GetComponent<CharacterInput>().LedgeAhead(out Vector3 hitPosition))
            {
                CallHintAnimation call = new CallHintAnimation() { animationName = "JumpHint"};
                call.Invoke();
            }
        }
        else if(GetComponent<CharacterInput>().LedgeAhead(out Vector3 hitPosition) && !Physics.Raycast(ray, out RaycastHit obj, 0.3f, cubeLayerMask))
        {
            CallHintAnimation call = new CallHintAnimation() { animationName = "JumpHint"};
            call.Invoke();
        }
    }
}
