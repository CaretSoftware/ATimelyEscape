using System;
using System.Collections;
using System.Collections.Generic;
using RatCharacterController;
using UnityEngine;

public class CameraFollow : MonoBehaviour { 
	public Transform Follow { get; private set; }
	[SerializeField] private Transform follow;
	private Transform _transform;
	private Transform _camera;
	private Vector3 _currentVelocity;
	[SerializeField] private float smoothness = 0.2f;
	[SerializeField] private float maxVelocity = 1.0f;
	[SerializeField] private float largestMagnitude = .07f;

	public float maxSpeed;
	public float smoothTime;
	private void Awake() {
		if (Follow == null)
			Follow = FindObjectOfType<CharacterAnimationController>().transform;
		
		_transform = transform;
		_camera = GetComponentInChildren<Camera>().transform; 
		_transform.position = Follow.position;
	}

	public float superSonic = 5.0f;
	public float superSmooth = 0.01f;

	private void LateUpdate() {
		Transform thisTransform = _transform;
		Vector3 position = thisTransform.position;
		Vector3 followPosition = Follow.position;
		Vector3 direction = followPosition - position;
		direction = _camera.InverseTransformDirection(direction);

		float magnitude = direction.ProjectOnPlane().z;
		
		float t = Mathf.InverseLerp(0.0f, largestMagnitude, magnitude);
		
		maxSpeed = Mathf.Lerp(superSonic, maxVelocity, t);
		smoothTime = Mathf.Lerp(superSmooth,smoothness, t);

		Vector3 newPosition = Vector3.SmoothDamp(position, followPosition, ref _currentVelocity, smoothTime, maxSpeed);
		
		thisTransform.position = newPosition;
	}

	public void SetFollowTransform(Transform newTransform) => Follow = newTransform;
}
