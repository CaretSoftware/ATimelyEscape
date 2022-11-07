using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TimeTravelDisplacement : MonoBehaviour {
	
	private static readonly int DisplacementPropertyID = Shader.PropertyToID("_Displacement");
	private static readonly int RotationPropertyID = Shader.PropertyToID("_EulerRotation");
	private static readonly int PositionPropertyID = Shader.PropertyToID("_Direction");
	private static readonly int MatrixPropertyID = Shader.PropertyToID("__Matrix4x4");
	[SerializeField] private Transform other;
	private Transform _transform;
	private MaterialPropertyBlock mpb;
	private MeshRenderer mr;

	// public float Displacement { get; set; }
	// public Transform OtherTransform { get; set; }
	[Range(0f, 1f)] public float t = 0f; 
	private void Awake() {
		_transform = transform;
		mpb = new MaterialPropertyBlock();
		mr = GetComponent<MeshRenderer>();
	}

	private void Update() {
		Displace(t);
	}

	private void Displace(float t) {
		Vector3 position = _transform.position;
		Vector3 eulerRotation = _transform.InverseTransformDirection((_transform.rotation * Quaternion.Inverse(_transform.rotation )).eulerAngles); 
			// (other.rotation * Quaternion.Inverse(_transform.rotation)).eulerAngles;
		
		eulerRotation.x = eulerRotation.x > 180 ? eulerRotation.x - 360 : eulerRotation.x;
		eulerRotation.y = eulerRotation.y > 180 ? eulerRotation.y - 360 : eulerRotation.y;
		eulerRotation.z = eulerRotation.z > 180 ? eulerRotation.z - 360 : eulerRotation.z;
		
		Vector3 offset = _transform.InverseTransformDirection(other.position - position);
		// offset = _transform.InverseTransformDirection(offset);
		
		mpb.SetFloat(DisplacementPropertyID, t);
		mpb.SetVector(PositionPropertyID, offset);
		mpb.SetVector(RotationPropertyID, eulerRotation);
		mpb.SetMatrix(MatrixPropertyID, Matrix4x4.TRS(position, _transform.rotation, Vector3.one));
		
		
		mr.SetPropertyBlock(mpb);
	}
}
