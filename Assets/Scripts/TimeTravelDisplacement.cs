using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTravelDisplacement : MonoBehaviour {
	
	private static readonly int DisplacementPropertyID = Shader.PropertyToID("_Displacement");
	private static readonly int RotationPropertyID = Shader.PropertyToID("_EulerRotation");
	private static readonly int PositionPropertyID = Shader.PropertyToID("_Direction");
	
	[SerializeField] private Transform other;
	[SerializeField] private float time = .2f;
	
	private Transform _transform;
	private MaterialPropertyBlock mpb;
	private MeshRenderer mr;

	private void Awake() {
		_transform = transform;
		mpb = new MaterialPropertyBlock();
		mr = GetComponent<MeshRenderer>();
	}

	// private void Update() {
		// t = Mathf.Sin(Time.time * Mathf.PI) * .5f + .5f;
		//  Displace(t);
		// if (Input.GetKeyDown(KeyCode.Q))
		// 	MoveAndRotateMesh();
	// }

	public void MoveAndRotateMesh() {
		if (other == null || mr == null) return;
		StopAllCoroutines();
		StartCoroutine(Displace());
	}

	private IEnumerator Displace() {
		float t = 1.0f;
		float velocity = 1 / time;
		while (t > 0.0f) {
			Displace(Ease.EaseInOutCubic(t));
			t -= velocity * Time.deltaTime;
			yield return null;
		}
		Displace(0.0f);
	}
	
	private void Displace(float t) {
		Quaternion localSpaceRotation = Quaternion.Inverse(_transform.rotation) * other.rotation;
		Vector3 eulerRotation = localSpaceRotation.eulerAngles;
		Vector3 offset = _transform.InverseTransformDirection(other.position - _transform.position);

		mpb.SetFloat(DisplacementPropertyID, t);
		mpb.SetVector(RotationPropertyID, eulerRotation);
		mpb.SetVector(PositionPropertyID, offset);

		mr.SetPropertyBlock(mpb);
	}
}
