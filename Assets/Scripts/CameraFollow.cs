using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
	[SerializeField] private Transform follow;
	private Transform _transform;

	private void Start() {
		_transform = transform;
	}

	private void LateUpdate() {
		_transform.position = follow.position;	// TODO smoothDamp, turn CameraFollow into CineMachine
	}
}
