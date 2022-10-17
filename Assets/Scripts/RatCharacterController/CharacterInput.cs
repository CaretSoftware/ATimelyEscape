using System;
using System.Collections;
using System.Collections.Generic;
using RatCharacterController;
using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RatCharacterController {

   public class CharacterInput : MonoBehaviour {
      
      private const string CharacterMovement = "CharacterMovement";
      private const string BoxMovement = "BoxMovement";
      private CharacterAnimationController _characterAnimationController;
      private Transform _cameraTransform;
      private CameraController _camController;
      private PlayerInputActions _playerInputActions;
      private PlayerInput _playerInput;
      private CapsuleCollider _collider;
      [SerializeField] private LayerMask groundedLayerMask;
      [SerializeField] private LayerMask cubeLayerMask;

      private void Start() {
         _cameraTransform = FindObjectOfType<Camera>().transform;
         _camController = FindObjectOfType<CameraController>();
         _characterAnimationController = GetComponent<CharacterAnimationController>();
         _collider = GetComponent<CapsuleCollider>();
         _playerInput = GetComponent<PlayerInput>();

         if (_camController == null)
            Debug.LogWarning($"Missing Camera Follow Prefab in scene, add prefab before going into playmode", this.gameObject);
      }

      public void Jump(InputAction.CallbackContext context) {
         if (context.performed) _characterAnimationController.JumpToFreeHang();
      }

      public void Interact(InputAction.CallbackContext context) {
         
         if (context.performed) {
            Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * 1.0f, Color.cyan, 1.0f);
            
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 1.0f, cubeLayerMask)) {
               Debug.Log("HIT");
               hitInfo.collider.GetComponent<CubePush>().Closest();
               _playerInput.SwitchCurrentActionMap("BoxMovement");
               _characterAnimationController.Push(true);
            }
         }
         
         // } else if (context.canceled) {
         //    _characterAnimationController.Push(false);
         //    _playerInput.SwitchCurrentActionMap("CharacterMovement");
         //    CubePush.NotClosest();
      }

      public void StopPushCube(InputAction.CallbackContext context) {
         if (context.performed) {
            Debug.Log("STOP");
            _characterAnimationController.Push(false);
            _playerInput.SwitchCurrentActionMap("CharacterMovement");
            CubePush.NotClosest();
         }
      }

      public void PushCube(InputAction.CallbackContext context) {
         Vector2 direction = context.ReadValue<Vector2>();

         Vector3 dir = new Vector3(direction.x, 0, direction.y);

         Vector3 projectedInput = InputToCameraProjection(dir);

         if (projectedInput.magnitude > 1.0f)
            projectedInput.Normalize();

         Vector3 transformInputDir = transform.InverseTransformDirection(projectedInput);

         Vector2 v2 = new Vector2(transformInputDir.x, transformInputDir.z);
         
         
         if (CubePush.closestCube != null)
            CubePush.closestCube.Push(v2);
      }

      private void Update() {
         // CameraInput();
         _characterAnimationController.SetGrounded(Grounded());
         MovementInput();
      }

      private void MovementInput() {
         Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            // _playerInputActions.CharacterMovement.Movement.ReadValue<Vector2>();
         
         Vector3 input3 = new Vector3(input.x, 0, input.y);
         
         Vector3 projectedInput = InputToCameraProjection(input3);
         
         if (projectedInput.magnitude > 1.0f)
            projectedInput.Normalize();
         
         Vector3 transformInputDir = transform.InverseTransformDirection(projectedInput);
         
         Vector2 v2 = new Vector2(transformInputDir.x, transformInputDir.z) * 2.0f;
         
         _characterAnimationController.InputVector(v2);
      }

      // private void CameraInput() {
      //    // thumbstick
      //    Vector2 cameraStickInput =  //new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
      //       _playerInputActions.CharacterMovement.CameraThumbstick.ReadValue<Vector2>();
      //    _camController.StickInput(cameraStickInput);
      //
      //    // mouse
      //    Vector2 cameraMouseInput =
      //       _playerInputActions.CharacterMovement.CameraMouseInput.ReadValue<Vector2>();
      //    _camController.MouseInput(cameraMouseInput);
      // }

      public void CharMovement(InputAction.CallbackContext context) {
         
         if (context.performed) {
            // Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            // // _playerInputActions.CharacterMovement.Movement.ReadValue<Vector2>();
            //
            // Vector3 input3 = new Vector3(input.x, 0, input.y);
            //
            // Vector3 projectedInput = InputToCameraProjection(input3);
            //
            // if (projectedInput.magnitude > 1.0f)
            //    projectedInput.Normalize();
            //
            // Vector3 transformInputDir = transform.InverseTransformDirection(projectedInput);
            //
            // Vector2 v2 = new Vector2(transformInputDir.x, transformInputDir.z) * 2.0f;
            //
            // _characterAnimationController.InputVector(v2);
            
            // //Debug.Log(context.ReadValue<Vector2>());
            // Vector3 forward = transform.forward;
            // Vector3 input = context.ReadValue<Vector2>();
            // Vector3 projectedInput = InputToCameraProjection(new Vector3(input.x, 0.0f, input.y));
            // Vector3 diff = transform.rotation * projectedInput;
            // Debug.Log($"{diff}");
            //
            // _characterAnimationController.InputVector(diff);
         }
      }
      
      private Vector3 InputToCameraProjection(Vector3 input) {
		
         if (_cameraTransform == null) 
            return input;

         Vector3 cameraRotation = _cameraTransform.transform.rotation.eulerAngles;
         // cameraRotation.x = Mathf.Min(cameraRotation.x, _planeNormal.y);
         input = Quaternion.Euler(cameraRotation) * input;

         float magnitude = input.magnitude;
         
         return Vector3.ProjectOnPlane(input, Vector3.up).normalized * magnitude;
      }

      private bool Grounded() {
         float radius = _collider.radius;
         Vector3 origin = transform.position + (radius + 0.05f)* Vector3.up;
         const float maxDistance = .2f;
         Ray ray = new Ray(origin, Vector3.down);
         
         Debug.DrawRay(origin, Vector3.down * radius);
         
         return Physics.SphereCast(ray, radius, maxDistance);
         // return Physics.Raycast(ray, .5f, groundedLayerMask);
      }
   }
}