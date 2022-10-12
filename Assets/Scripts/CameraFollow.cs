using System;
using System.Collections;
using System.Collections.Generic;
using RatCharacterController;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
	[SerializeField] private Transform follow;
	private Transform _transform;

	private void Start() {
		if (follow == null)
			follow = FindObjectOfType<CharacterAnimationController>().transform;
		
		_transform = transform;
	}

	private void LateUpdate() {
		_transform.position = follow.position;	// TODO smoothDamp, turn CameraFollow into CineMachine
	}
}
