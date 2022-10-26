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

      private void Start() {
         Physics.queriesHitTriggers = false;    // ray-/capsule-/sphere-casts don't hit triggers
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

      private bool _jumping;
      public void JumpComplete() {
         _jumping = false;
      }
      
      private void Jump(InputAction.CallbackContext context) {
         // TODO
         // check if Grounded() && in front of box && fits on top of box
         // lerp player in front of box
         // slerp players rotation to inverse raycast.normalDirection
         // Jump To freeHang
         Transform playerTransform = _playerTransform;
         Vector3 playerPosition = playerTransform.position;
         float playerScale = playerTransform.localScale.y;
         float margin = .1f * playerScale;
         Ray ray = RayAtHalfHeight(playerTransform);
         CapsuleCollider capsuleCollider = _collider;
         float radius = capsuleCollider.radius * playerScale;
         _playerForward = playerTransform.forward;

         if (!_jumping && Grounded()) {
            if (LedgeAhead()) {
               _jumping = true;
               playerTransform.rotation = Quaternion.LookRotation(_playerForward, Vector3.up);
               _characterAnimationController.JumpToFreeHang();
            }
            else {
               _characterAnimationController.LeapJump();
            }
         }

         bool LedgeAhead() {
            Vector3 ledgeHeight = 2.0f * playerScale * Vector3.up;
         
            _point0 = playerPosition + 
                      ledgeHeight + 
                      (radius + margin) * Vector3.up;
            _point1 = playerPosition + 
                      ledgeHeight + 
                      (_collider.height + margin) * playerScale * Vector3.up - 
                      radius * Vector3.up;
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 1.0f * playerScale) && 
                Physics.OverlapCapsule(_point0, _point1, radius, groundedLayerMask).Length < 1) {

               _playerForward = -hitInfo.normal.ProjectOnPlane();

               return !Physics.CapsuleCast(
                  point1: _point0,
                  point2: _point1,
                  radius: radius - margin,
                  direction: _playerForward,
                  maxDistance: 1.0f * playerScale,
                  groundedLayerMask);
            }

            return false;
         }
      }

      private Vector3 _point0;
      private Vector3 _point1;
      private Vector3 _playerForward;
#if UNITY_EDITOR
      private void OnDrawGizmos() {
         if (!Application.isPlaying) return;
         float localScale = _playerTransform.localScale.z;
         float radius = _collider.radius * localScale;
         Vector3 point1 = _point0 + localScale * _playerForward;
         Vector3 point2 = _point1 + localScale * _playerForward;
         Gizmos.DrawWireSphere(point1, radius);
         Gizmos.DrawWireSphere(point2, radius);
         Gizmos.DrawLine(point1 + radius * Vector3.forward, point2 + radius * Vector3.forward);
         Gizmos.DrawLine(point1 + radius * Vector3.back, point2 + radius * Vector3.back);
         Gizmos.DrawLine(point1 + radius * Vector3.left, point2 + radius * Vector3.left);
         Gizmos.DrawLine(point1 + radius * Vector3.right, point2 + radius * Vector3.right);
      }
#endif
      
      private Ray RayAtHalfHeight(Transform playerTransform) {
         return new Ray(
               transform.position + Vector3.up * _characterHalfHeight * playerTransform.localScale.y, 
               playerTransform.forward);
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