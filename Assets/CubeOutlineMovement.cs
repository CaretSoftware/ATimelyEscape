using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeOutlineMovement : MonoBehaviour {
	[SerializeField] private Transform past;
	[SerializeField] private Transform present;
	[SerializeField] private Transform future;
	[SerializeField] private TimeTravelDisplacement timeTravelDisplacement;
	private Transform _currentTransform;

	private void Awake() {
		_currentTransform = present;
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			transform.SetPositionAndRotation(_currentTransform.position, _currentTransform.rotation);
			timeTravelDisplacement.Displace(past);
			_currentTransform = past;
		}
		if (Input.GetKeyDown(KeyCode.Alpha2)) {
			transform.SetPositionAndRotation(_currentTransform.position, _currentTransform.rotation);
			timeTravelDisplacement.Displace(present);
			_currentTransform = present;
		}
		if (Input.GetKeyDown(KeyCode.Alpha3)) {
			transform.SetPositionAndRotation(_currentTransform.position, _currentTransform.rotation);
			timeTravelDisplacement.Displace(future);
			_currentTransform = future;
		}
	}
}
