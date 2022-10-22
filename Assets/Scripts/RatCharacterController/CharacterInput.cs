using System;
using System.Collections;
using System.Collections.Generic;
using RatCharacterController;
using Unity.Mathematics;
using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RatCharacterController {

   public class CharacterInput : MonoBehaviour {
      
      private CharacterAnimationController _characterAnimationController;
      private CameraFollow _cameraFollow;
      private Transform _cameraTransform;
      private CameraController _camController;
      private PlayerInputActions _playerInputActions;
      private CapsuleCollider _collider;
      [SerializeField] private LayerMask groundedLayerMask;
      [SerializeField] private LayerMask cubeLayerMask;
      private Transform _playerTransform;
      private Rigidbody _rigidBody;
      private Vector3 _pushedCubeOffset;
      private bool _pushing;
      private float _characterHalfHeight;
      private bool _characterMovement = true;

      private void Start() {
         _playerInputActions = new PlayerInputActions();
         _playerInputActions.CameraControls.Enable();
         _playerInputActions.CharacterMovement.Enable();
         _playerInputActions.Interact.Enable();
         _playerInputActions.CharacterMovement.Jump.performed += Jump;
         _playerInputActions.Interact.Interact.performed += Interact;
         _playerInputActions.Interact.Interact.canceled += StopInteract;

         _cameraTransform = FindObjectOfType<Camera>().transform;
         _camController = FindObjectOfType<CameraController>();
         _cameraFollow = FindObjectOfType<CameraFollow>();
         
         _playerTransform = transform;
         _rigidBody = _playerTransform.GetComponent<Rigidbody>();
         _characterAnimationController = GetComponent<CharacterAnimationController>();
         _collider = GetComponent<CapsuleCollider>();
         _characterHalfHeight = _collider.height * .5f;

         if (_camController == null)
            Debug.LogWarning($"Missing Camera Follow Prefab in scene, add prefab before going into playmode", this.gameObject);
      }

      private void Update() {
         CameraInput();
         _characterAnimationController.SetGrounded(Grounded());

         if (!_pushing)
            MovementInput( _playerInputActions.CharacterMovement.Movement.ReadValue<Vector2>() );
         else
            PushCubeInput( _playerInputActions.BoxMovement.Movement.ReadValue<Vector2>() );
      }
      
      private void Jump(InputAction.CallbackContext context) {
         // TODO
         // check if Grounded() && in front of box && fits on top of box
         // lerp player in front of box
         // slerp players rotation to inverse raycast.normalDirection
         // Jump To freeHang
         if (Grounded()) _characterAnimationController.JumpToFreeHang();
      }

      private void CameraInput() {

         Vector2 cameraStickInput = _playerInputActions.CameraControls.CameraThumbstick.ReadValue<Vector2>();
         Vector2 cameraMouseInput = _playerInputActions.CameraControls.CameraMouseInput.ReadValue<Vector2>();
         
         _camController.StickInput(cameraStickInput);
         _camController.MouseInput(cameraMouseInput);
      }

      private void MovementInput(Vector2 input) => MovementInput(input.ToVector3());
      private void MovementInput(Vector3 input) {

         Vector3 projectedInput = InputToCameraProjection(input);

         Vector3 transformInputDir = transform.InverseTransformDirection(projectedInput);

         _characterAnimationController.InputVector(transformInputDir);
      }

      private void PushCubeInput(Vector2 input) => PushCubeInput(input.ToVector3());
      private void PushCubeInput(Vector3 input) {
         Transform playerTransform = _playerTransform;
         Ray ray = new Ray(_playerTransform.position + Vector3.up * _characterHalfHeight * _playerTransform.localScale.y, _playerTransform.forward);

         Vector3 projectedInput = InputToCameraProjection(input);

         if (projectedInput.magnitude > 1.0f)
            projectedInput.Normalize();

         OffsetPlayerPosition();
         RotatePlayerToSurface();

         void OffsetPlayerPosition() {
            CubePush cube = CubePush.closestCube;
            if (cube != null) {
               cube.Push(projectedInput);
               playerTransform.position = cube.transform.position + _pushedCubeOffset;
            }
         }
         
         void RotatePlayerToSurface() {
            if (Physics.Raycast(ray, out RaycastHit hitInfo, .1f, cubeLayerMask)) {
               Vector3 rotation = (-hitInfo.normal).ProjectOnPlane().normalized;
               if (rotation != Vector3.zero)
                  playerTransform.rotation = Quaternion.LookRotation(rotation, Vector3.up);
            }
         }
      }

      private void Interact(InputAction.CallbackContext context) {
         
         Transform playerTransform = _playerTransform;
         Ray ray = new Ray(transform.position + Vector3.up * _characterHalfHeight * playerTransform.localScale.y, playerTransform.forward);

         if (Physics.Raycast(ray, out RaycastHit hitInfo, .1f, cubeLayerMask)) {
            _pushing = true;
            Transform cube = hitInfo.transform;
            
            cube.GetComponent<CubePush>().Closest();

            _pushedCubeOffset = playerTransform.position - cube.position;

            _characterAnimationController.Push(true);
            
            _cameraFollow.SetFollowTransform(cube);
            
            _playerInputActions.BoxMovement.Enable();
            _playerInputActions.CharacterMovement.Disable();
         }
      }

      private void StopInteract(InputAction.CallbackContext context) {
         
         _pushing = false;
         _characterAnimationController.Push(false);
         
         _cameraFollow.SetFollowTransform(_playerTransform);
         

         CubePush.NotClosest();
         
         _playerInputActions.BoxMovement.Disable();
         _playerInputActions.CharacterMovement.Enable();
      }

      private Vector3 InputToCameraProjection(Vector3 input) {
		
         if (_cameraTransform == null) 
            return input;

         Vector3 cameraRotation = _cameraTransform.transform.rotation.eulerAngles;
         
         input = Quaternion.Euler(cameraRotation) * input;

         float magnitude = input.magnitude;
         
         return Vector3.ProjectOnPlane(input, Vector3.up).normalized * magnitude;
      }

      private bool Grounded() {
         Transform playerTransform = _playerTransform;
         float radius = _collider.radius * playerTransform.localScale.y;
         float margin = 0.01f;
         float maxDistance = .02f + margin;
         Vector3 origin = playerTransform.position + (radius * Vector3.up) + (margin * Vector3.up);
         Ray ray = new Ray(origin, Vector3.down);
         
         Debug.DrawRay(origin, Vector3.down * maxDistance);
         
         return Physics.SphereCast(ray, radius, maxDistance, groundedLayerMask);
      }
   }
}