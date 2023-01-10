using UnityEngine;

public class LookAt : MonoBehaviour
{
    public Transform cameraTarget;
    [SerializeField] private Camera thirdPersonCamera;
    [SerializeField] private Camera exitCamera;
    void Start()
    {
        exitCamera.enabled = false;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab)) {
            thirdPersonCamera.transform.LookAt(cameraTarget);
            thirdPersonCamera.enabled = false;
            exitCamera.enabled = true;
        }
        if(Input.GetKeyUp(KeyCode.Tab)) {
            exitCamera.enabled = false;
            thirdPersonCamera.enabled = true;
        }
    }
}
