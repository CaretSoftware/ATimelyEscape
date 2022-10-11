using System;
using System.Collections;
using System.Collections.Generic;
using RatCharacterController;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RatCharacterController {

   public class CharacterInput : MonoBehaviour {
      
      private const string CharacterMovement = "CharacterMovement";
      private const string BoxMovement = "BoxMovement";
      private CharacterAnimationController _characterAnimationController;
      private Transform _cameraTransform;
      
      private void Start() {
         _characterAnimationController = GetComponent<CharacterAnimationController>();
         _cameraTransform = FindObjectOfType<Camera>().transform;
      }

      public void Jump(InputAction.CallbackContext context) {
         if (context.performed) Debug.Log("jump");
      }

      private void Update() {

         Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
         
         Vector3 input3 = new Vector3(input.x, 0, input.y);

         Vector3 projectedInput = InputToCameraProjection(input3);

         Vector3 transformInputDir = transform.InverseTransformDirection(projectedInput);

         Vector2 v2 = new Vector2(transformInputDir.x, transformInputDir.z);

         _characterAnimationController.InputVector(v2);
      }

      public void CharMovement(InputAction.CallbackContext context) {
         
         if (context.performed) {
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
         return Vector3.ProjectOnPlane(input, Vector3.up).normalized;
      }
   }
}