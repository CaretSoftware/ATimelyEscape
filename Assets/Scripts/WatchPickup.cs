using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Update = UnityEngine.PlayerLoop.Update;

[RequireComponent(typeof(SphereCollider))]
public class WatchPickup : MonoBehaviour {
	
	[SerializeField] private float rotationSpeed = 25.0f;

	private void Awake() {
		GetComponent<SphereCollider>().isTrigger = true;
		transform.position += Vector3.up * .1f;
		Rigidbody _rb = GetComponent<Rigidbody>();
		if (_rb != null) {
			Destroy(_rb);
		}
	}

	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Player")) {
			RatCharacterController.CharacterInput.CanTimeTravel(true);
			Destroy(this.gameObject);
		}
	}

	private Vector3 rot = new Vector3(.23f, .7f, .13f);
	// public bool debug;
	private void Update() {
		// Time.timeScale = debug ? 0.0f : 1.0f;
		
		transform.rotation *=
			Quaternion.Euler(Time.unscaledDeltaTime * rot * rotationSpeed).normalized;
	}
}
