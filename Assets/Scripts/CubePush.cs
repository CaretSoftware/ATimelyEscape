using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubePush : MonoBehaviour {

	private Rigidbody rb;
	[SerializeField] private float pushSpeed = 1.5f;
	public static CubePush closestCube;
	
	private void Start() {
		rb = GetComponent<Rigidbody>();
	}

	public void Closest() {
		closestCube = this;
		Debug.Log("closest", this);
	}

	public static void NotClosest() {
		Debug.Log("NOT CLOSEST");
		// closestCube = null;
	} 
	
	public void Push(Vector2 direction) {
		Vector3 dir = new Vector3(direction.x, 0.0f, direction.y);
		rb.MovePosition(transform.position + Time.deltaTime * pushSpeed * dir);
	}
}
