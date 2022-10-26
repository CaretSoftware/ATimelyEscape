using System;
using RatCharacterController;
using Unity.VisualScripting;
using UnityEngine;
using Quaternion = System.Numerics.Quaternion;

[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class WatchPickup : MonoBehaviour {
	private Rigidbody _rb;
	private void Awake() {
		GetComponent<SphereCollider>().isTrigger = true;
		_rb = GetComponent<Rigidbody>();
		_rb.useGravity = false;
		transform.position += Vector3.up * .1f;
	}

	private void FixedUpdate() {
		Vector3 randRot = UnityEngine.Random.onUnitSphere;
		_rb.AddTorque(randRot);
	}

	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Player")) {
			CharacterInput.CanTimeTravel(true);
			Destroy(this.gameObject);
		}
	}
}
