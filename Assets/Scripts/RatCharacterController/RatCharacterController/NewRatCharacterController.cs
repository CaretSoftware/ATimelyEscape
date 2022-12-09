using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace NewRatCharacterController { 
	[RequireComponent(typeof(CapsuleCollider)), DisallowMultipleComponent, SelectionBase]
public class NewRatCharacterController : MonoBehaviour {

	private StateMachine _stateMachine;
	private List<BaseState> _states = new List<BaseState> { new MoveState(), new JumpState(), new AirState(), new WallRunState(), new WallJumpState() };
	private Collider[] _OverlapCollidersNonAlloc = new Collider[10];
	private Transform _camera;
	private Transform _transform;
	private AnimationController _animationController;
	public AnimationController AnimationController { 
		get => _animationController;
		private set => GetComponent<AnimationController>();
	}

	// Animation
	private static readonly int DuckAnimationSpeedStringHash = Animator.StringToHash("Speed");

	// Collider
	private CapsuleCollider _collider;
	[HideInInspector] public float _colliderRadius;
	[HideInInspector] public Vector3 normalForce;
	
	// Input
	private const string Jump = "Jump";
	private const string Horizontal = "Horizontal";
	private const string Vertical = "Vertical";
	private Vector3 _inputMovement;
	private bool _holdingDuck;
	public bool PressedJump { get; private set; }
	public bool HoldingJump { get; private set; }
	public bool Jumped { get; private set; }
	public Vector3 GroundNormal { get; private set; }
	[HideInInspector] public Vector3 _velocity;

	
	[Space(10), Header("Character Controller Implementation Details")]
	[SerializeField, Tooltip("LayerMask(s) the character should collide with")]
	public LayerMask _collisionMask;
	[SerializeField, Range(0.0f, 0.15f), Tooltip("The distance the character should stop before a collider")]
	public float _skinWidth = 0.1f;
	[SerializeField, Range(0.0f, 0.2f), Tooltip("The distance the character should count as being grounded")]
	private float _groundCheckDistance = 0.15f;
	[SerializeField] 
	public Transform _point1Transform;
	[SerializeField] 
	public Transform _point2Transform;

	
	[Space(10), Header("\tJump"), Header("Character Settings")]
	[SerializeField] private float _jumpBuffer = .25f;
	[SerializeField] private float _coyoteTime = .2f;
	[FormerlySerializedAs("minAirControl")] [SerializeField] private float maxAirControl = .5f;
	[SerializeField] private float airControlTime = 1.0f;
	private float _lastGroundedMoment = -1.0f;
	private float _pressedJumpMoment = -1.0f;
	public bool Grounded { get; private set; }
	[HideInInspector] public bool _jumpedOnce;
	[HideInInspector] public float airTime;

	[SerializeField, Range(0.0f, 20.0f)]
	public float _defaultGravity = 15.0f;

	[SerializeField] [Range(0.0f, 4.0f)] 
	public float _fallGravityMultiplier = 2.0f;
	
	[SerializeField] [Range(0.0f, 100.0f)] [Tooltip("Max fall speed")]
	private float _terminalVelocity = 12.0f;
	
	
	[Space(10), Header("\tMovement")]
	[SerializeField, Range(0.0f, 30.0f)]
	private float _acceleration = 20.0f;

	[SerializeField, Range(0.0f, 10.0f), Tooltip("The deceleration when no input")]
	private float _deceleration = 1.5f;
	
	[SerializeField, Range(0.0f, 25.0f), Tooltip("Extra force when character is turning the opposite way")]
	private float _turnSpeedModifier = 2.0f; 

	[SerializeField] [Range(-20.0f, 20.0f)] 
	private float _turnSpeedModifierThreshold;
	
	[SerializeField, Range(0.0f, 30.0f), Tooltip("Max ground speed")]
	private float _maxVelocity = 2.0f;
	
	[SerializeField, Range(0.0f, 20.0f), Tooltip("Set before hitting [\u25BA]\nOnly changed during start")]
	public float _jumpForce = 10.0f;
	
	[SerializeField, Tooltip("The height the character can step over without jumping")] 
	public float stepHeight = 0.03f;

	[Space(10), Header("\tFriction")]
	[SerializeField, Range(0.0f, 1.0f), Tooltip("Force to overcome friction from a standstill")]
	public float _staticFrictionCoefficient = 0.5f; 

	[SerializeField, Range(0.0f, 1.0f), Tooltip("Force applied when moving\n(60-70% of static friction usually)")]
	public float _kineticFrictionCoefficient = 0.2f; 
	
	[SerializeField, Range(0.0f, 1.0f), Tooltip("Force affecting velocity")]
	private float _airResistanceCoefficient = .5f;

	
	[Space(10), Header("\tAnimation")]
	[SerializeField] 
	private Animator ducking;

	[ContextMenu("Reset Character Position")]
	private void ResetCharacterPosition() {
		_transform.position = Vector3.up;
		_velocity = Vector3.zero;
	}
	
	private void Awake() {
		_transform = transform;
		_animationController = FindObjectOfType<AnimationController>();
		_stateMachine = new StateMachine(this, _states);
		_collider = GetComponent<CapsuleCollider>();
		_camera = GetComponentInChildren<Camera>().transform;
	}

	private void Start() {
		
		_colliderRadius = _collider.radius;
		//ducking.Play("Ducking");
		//_point1 
		//_point1Transform.localPosition = _collider.center + Vector3.up * (_collider.height / 2 - _colliderRadius);
		//_point2 = _collider.center + Vector3.down * (_collider.height / 2 - _colliderRadius);
		//_point2Transform.localPosition = _point2;
	}
	
	private void Update() {
		
		_inputMovement = Vector3.zero;
		// Ducking();
		UpdateGrounded();
		Input();
		_stateMachine.Run();
		UpdateVelocity();
		ResolveOverlap();
		
		_transform.position += Time.deltaTime * _velocity;
		RotateTransform();
	}

	
	public void Ducking() {
		
		float animationProgress = ducking.GetCurrentAnimatorStateInfo(0).normalizedTime;
		if (_holdingDuck && animationProgress >= 1.0f ||
		    !_holdingDuck && animationProgress <= 0.0f) {
			ducking.SetFloat(DuckAnimationSpeedStringHash, 0);
			return;
		}
		
		ducking.SetFloat(DuckAnimationSpeedStringHash, _holdingDuck ? 1 : -1);
	}

	private void Input() {
		
		float right = UnityEngine.Input.GetAxisRaw(Horizontal);
		float forward = UnityEngine.Input.GetAxisRaw(Vertical);
		
		_inputMovement = new Vector3(right, 0.0f, forward);
		_inputMovement = InputToCameraProjection(_inputMovement);
		if (_inputMovement.magnitude > 1.0f) // > 1.0f to keep thumbstick input from being normalized
			_inputMovement.Normalize();

		_holdingDuck = UnityEngine.Input.GetKey(KeyCode.LeftShift);
		HoldingJump = UnityEngine.Input.GetButton(Jump);
		PressedJump = UnityEngine.Input.GetButtonDown(Jump);
		
		_pressedJumpMoment = PressedJump ? Time.time : _pressedJumpMoment;

		Jumped = !_jumpedOnce && 
		          (JumpBuffer() ||
		           PressedJump && (Grounded || CoyoteTime()));
	}

	private bool JumpBuffer() {
		
		return Grounded && _pressedJumpMoment + _jumpBuffer > Time.time;
	}
	
	private bool CoyoteTime() {
		
		if (Grounded)
			_lastGroundedMoment = Time.time;
		return !Grounded && _lastGroundedMoment + _coyoteTime > Time.time;
	}
	
	// public bool IsGrounded() {
	// 	
	// 	return Grounded;
	// }

	private void UpdateGrounded() {
		
		Grounded =
			Physics.SphereCast(
				_point2Transform.position,//transform.position + _point2, 
				_colliderRadius, 
				Vector3.down, 
				out var hit, 
				_groundCheckDistance + _skinWidth, 
				_collisionMask);
		
		_lastGroundedMoment = Grounded ? Time.time : _lastGroundedMoment;
		
		GroundNormal = Grounded ? hit.normal : Vector3.up;
	}

	private Vector3 InputToCameraProjection(Vector3 input) {
		
		if (_camera == null) 
			return input;

		Vector3 cameraRotation = _camera.rotation.eulerAngles;
		cameraRotation.x = Mathf.Min(cameraRotation.x, GroundNormal.y);
		input = Quaternion.Euler(cameraRotation) * input;
		return Vector3.ProjectOnPlane(input, GroundNormal);
	}

	public void HandleVelocity() {
		
		float right = UnityEngine.Input.GetAxisRaw(Horizontal);
		float forward = UnityEngine.Input.GetAxisRaw(Vertical);
		
		Vector3 velocityInWorldSpace = _transform.InverseTransformDirection(_velocity);
		
		Vector3 velocityChange = Vector3.ProjectOnPlane( new Vector3(right, 0.0f, forward), GroundNormal);

		Vector3 velocity = new Vector3( 
				Velocity(velocityInWorldSpace.x, velocityChange.x), 
				0.0f, 
				Velocity(velocityInWorldSpace.z, velocityChange.z));
		
		// velocity = Vector3.ClampMagnitude(velocity, _maxVelocity);

		float verticalVelocity = _velocity.y;
		_velocity = _transform.TransformDirection(velocity);
		_velocity.y = verticalVelocity;
		float angle = Vector3.Angle(Vector3.up, GroundNormal);
		if (angle < 40 && angle != 0)
			_velocity = Vector3.ProjectOnPlane(_velocity, GroundNormal).normalized * _velocity.magnitude;

		_velocity = Vector3.ClampMagnitude(_velocity, _maxVelocity);

		float Velocity(float vel, float inp) {

			if (Mathf.Abs(inp) > float.Epsilon)
				vel = Accelerate(vel, inp);
			else
				vel = Decelerate(vel);

			return vel;
			
			float Accelerate(float vel, float inp) {
				
				 return vel + inp * (
					 //InclineMultiplier() * 
					 ((ChangedDirection(inp, vel) ? _turnSpeedModifier : 1.0f) *
					  _acceleration) * Time.deltaTime);
			}

			float Decelerate(float vel) {
				
				if (Mathf.Abs(vel) < _deceleration * Time.deltaTime)
					return 0.0f;
				else
					return vel - _deceleration * Time.deltaTime * vel;
			}
		
			float InclineMultiplier() => Ease.EaseInQuart(Mathf.Clamp01(Vector3.Dot(GroundNormal, Vector3.up)));
			bool ChangedDirection(float inp, float vel) => inp > 0.0f && vel < 0.0f || inp < 0.0f && vel > 0.0f;
		}
	}
	
	public void Accelerate(Vector3 input) {
		// TODO Handle acceleration and deceleration on x and z axis separately 
		// TODO Clamp fallsSpeed outside of Acceleration 
		
		_velocity += input * (InclineMultiplier() * ((ChangedDirection() ? _turnSpeedModifier : 1.0f) *
		                                             _acceleration) * Time.deltaTime);

		// Clamp Velocities
		float verticalVelocity = Mathf.Clamp(_velocity.y, -_terminalVelocity, _maxVelocity);
		_velocity = Vector3.ProjectOnPlane(_velocity, Vector3.up);
		_velocity = Vector3.ClampMagnitude(_velocity, _maxVelocity);
		_velocity.y = verticalVelocity;

		float InclineMultiplier() => Ease.EaseInQuart(Mathf.Clamp01(Vector3.Dot(GroundNormal, Vector3.up)));
		bool ChangedDirection() => Vector3.Dot(input, _velocity) < _turnSpeedModifierThreshold;
	}

	public void Decelerate() {
		
		// TODO This might not have been the problem...
		// Probably the turnSpeedModifier is increasing the velocity?
		
		Vector3 projection = Vector3.ProjectOnPlane(_velocity, GroundNormal);

		if (VelocitySmallerThanDeceleration()) { // prevents vibration
			_velocity.x = 0.0f;
			_velocity.z = 0.0f;
		} else {
			float verticalVelocity = _velocity.y;
			_velocity -= _deceleration * Time.deltaTime * _velocity;

			//if (verticalVelocity <= 0.0f)
				_velocity.y = verticalVelocity;
		}
		
		bool VelocitySmallerThanDeceleration() => _deceleration * Time.deltaTime > projection.magnitude;
	}
	
	public void UpdateVelocity() {
		
		normalForce = Normal.Force(_velocity, GroundNormal);
		
		if (_velocity.magnitude < float.Epsilon) { 
			_velocity = Vector3.zero;
			return;
		}

		RaycastHit hit;
		int iterations = 0;
		do {

			hit = CapsuleCasts(_velocity, _transform.position);

			if (!hit.collider)
				continue;

			float skinWidth = _skinWidth / Vector3.Dot(_velocity.normalized, hit.normal);
			float distanceToSkinWidth = hit.distance + skinWidth;

			if (distanceToSkinWidth > _velocity.magnitude * Time.deltaTime) 
				return;
				
			if (distanceToSkinWidth > 0.0f)
				_transform.position += distanceToSkinWidth * _velocity.normalized;

			normalForce = Normal.Force(_velocity, hit.normal);
			
			_velocity += normalForce;
			
			ApplyFriction(normalForce);
			
		} while (hit.collider && iterations++ < 10);
		

		if (iterations > 9)
			Debug.Log("UpdateVelocity " + iterations);
	}
	
	public void ResolveOverlap() {

		int _exit = 0;
		int _count = Physics.OverlapCapsuleNonAlloc(
			_point1Transform.position,// transform.position + _point1,
			_point2Transform.position,//transform.position + _point2,
			_collider.radius,
			_OverlapCollidersNonAlloc,
			_collisionMask);

		while (_count > 0 && _exit++ < 10) {

			for (int i = 0; i < _count; i++) {
				if (Physics.ComputePenetration(
					    _collider, // TODO optimize this? _collider.transform.position should be _transform.position and .rotation??
					    _collider.transform.position,
					    _collider.transform.rotation, 
					    _OverlapCollidersNonAlloc[i], 
					    _OverlapCollidersNonAlloc[i].gameObject.transform.position, 
					    _OverlapCollidersNonAlloc[i].gameObject.transform.rotation,
					    out var direction,
					    out var distance)) {

					Vector3 separationVector = direction * distance;
					_transform.position += separationVector + separationVector.normalized * _skinWidth;
					_velocity += Normal.Force(_velocity, direction);
				}
			}

			_count = Physics.OverlapCapsuleNonAlloc(
				_point1Transform.position,//transform.position + _point1,
				_point2Transform.position,//transform.position + _point2,
				_collider.radius,
				_OverlapCollidersNonAlloc,
				_collisionMask);
			_exit++;
		}

		if (_exit > 10) 
			Debug.Log("ResolveOverlap Exited");
	}

	private void ApplyFriction(Vector3 normalForce) {
		
		if (_velocity.magnitude < normalForce.magnitude * _staticFrictionCoefficient) {
			_velocity = Vector3.zero;
		} else {
			_velocity -= 
				_kineticFrictionCoefficient * normalForce.magnitude * _velocity.normalized;
		}
	}
	
	public void AirControl() {
		
		float airControlPercentage = Mathf.InverseLerp(1, 0, airTime / airControlTime);
		float airControl = Mathf.Lerp( 0, maxAirControl, airControlPercentage);
		airTime += Time.deltaTime;
		_velocity +=  airControl * 10.0f * Time.deltaTime * _inputMovement;
	}

	public void ApplyAirFriction() {
		float y = _velocity.y;
		_velocity *= Mathf.Pow(1.0f - _airResistanceCoefficient, Time.deltaTime);
		_velocity.y = y;
	} 

	private RaycastHit CapsuleCasts(Vector3 direction, Vector3 position, float distance = float.PositiveInfinity) {
		
		Physics.CapsuleCast( _point1Transform.position,//position + _point1, 
			_point2Transform.position,//position + _point2, 
			_colliderRadius, 
			direction, 
			out var hit, 
			distance, 
			_collisionMask);
		return hit;
	}

	private void RotateTransform() {
		
		Vector3	lookDirection = Vector3.ProjectOnPlane(_camera.forward, Vector3.up);
		
		_transform.rotation = Quaternion.LookRotation(lookDirection);
	}

	// private Vector3 _debugCollider;
	// private Color _debugColor = new Color(10, 20, 30);
	// private void OnDrawGizmos() {
	// 	
	// 	_debugCollider = _transform.position + _velocity * Time.deltaTime + Vector3.up * .5f;
	// 	
	// 	Gizmos.color = _debugColor;
	// 	
	// 	Gizmos.DrawWireSphere(_debugCollider, .5f);
	// 	
	// 	Gizmos.DrawLine(_debugCollider + Vector3.back * .5f, _debugCollider + Vector3.up + Vector3.back * .5f);
	// 	
	// 	Gizmos.DrawLine(_debugCollider + Vector3.forward * .5f, _debugCollider + Vector3.up + Vector3.forward * .5f);
	// 	
	// 	Gizmos.DrawLine(_debugCollider + Vector3.right * .5f, _debugCollider + Vector3.up + Vector3.right * .5f);
	// 	
	// 	Gizmos.DrawLine(_debugCollider + Vector3.left * .5f, _debugCollider + Vector3.up + Vector3.left * .5f);
	// 	
	// 	Gizmos.DrawWireSphere(_debugCollider + Vector3.up, .5f);
	// }
}
}
