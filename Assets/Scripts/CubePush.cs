using CallbackSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[RequireComponent(typeof(CubeCharge))]
[RequireComponent(typeof(TimeTravelObject))]*/
public class CubePush : MonoBehaviour {

	private Rigidbody rb;
	//[SerializeField] private float pushSpeed = 1.5f;
	public static CubePush closestCube;
	[SerializeField] private bool pushable;
	private Transform _camera;
	private Vector3 desiredVelocity;

	private void Start() {
		_camera = FindObjectOfType<Camera>().transform;
		rb = GetComponent<Rigidbody>();
	}

	public void Closest() {
		closestCube = this;
	}

	public static void NotClosest() {
		closestCube = null;
	}

	public void Push(Vector2 direction) => Push(direction.ToVector3()); 
	public void Push(Vector3 direction)
	{
		if (!pushable) return;
		direction.y = rb.velocity.y;
		Vector3 velocity = direction;
		desiredVelocity = velocity;
		//RotateCube(velocity);
		OnboardingHandler.CubeInteractionsDiscovered = true;
	}

	private void FixedUpdate()
	{
		desiredVelocity.y = rb.velocity.y;
		rb.velocity = desiredVelocity;
		desiredVelocity = Vector3.zero;
	}

	/*
	private void RotateCube(Vector3 velocity) {
		float magnitude = velocity.magnitude;

		float t = Mathf.InverseLerp(0.0f, pushSpeed, magnitude);
		
		Debug.Log(magnitude);
		
		Vector3 currentDirection = velocity.ProjectOnPlane();
		Vector3 desiredDirection = _camera.forward.ProjectOnPlane().normalized;
		Vector3 vectorDelta = Vector3.Lerp(currentDirection, desiredDirection, t);
		
		rb.AddTorque(vectorDelta);
		// cube.rotation = Quaternion.Slerp(current, desired, Time.deltaTime);
	}
	*/
	
	public void SetPushable(bool pushable) => this.pushable = pushable;

	public bool Pushable() => pushable;
}
