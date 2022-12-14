using System;
using System.Collections;
using System.Collections.Generic;
using CallbackSystem;
using UnityEngine;

public class TimeTravelDisplacement : MonoBehaviour {
    private static readonly int DisplacementPropertyID = Shader.PropertyToID("_Displacement");
    private static readonly int OutlinePropertyID = Shader.PropertyToID("_Outline");
    private static readonly int AlphaPropertyID = Shader.PropertyToID("_Alpha");
    private static readonly int RotationPropertyID = Shader.PropertyToID("_EulerRotation");
    private static readonly int PositionPropertyID = Shader.PropertyToID("_Direction");

    [SerializeField] private Transform other;
    [SerializeField] private float time = .4f;

    private Transform _transform;
    private MaterialPropertyBlock mpb;
    private MeshRenderer mr;

    [SerializeField, Range(0.0f, 1.0f)] private float t;

    private void Awake() {
        _transform = transform;
        mpb = new MaterialPropertyBlock();
        mr = GetComponent<MeshRenderer>();
    }

    private void MoveAndRotateMesh() {
        if (other == null || mr == null) return;
        StopAllCoroutines();
        StartCoroutine(Displace());
    }

    public void Displace(Transform target) {
        other = target;
        MoveAndRotateMesh();
    }

    private IEnumerator Displace() {
        float t = 0.0f;
        float velocity = 1.0f / time;


        while (t < 1.0f) {
            t += velocity * Time.deltaTime;
            Displace(Ease.EaseInOutCubic(t));
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

    private IEnumerator FadeInOutline() {
        float t = 0.0f;
        while (t < 1.0f) {
            t += Time.deltaTime;
            OutlineStrength(t);
            yield return null;
        }

        OutlineStrength(1.0f);

    }

    private void OutlineStrength(float t) {
        mpb.SetFloat(OutlinePropertyID, t);
        mr.SetPropertyBlock(mpb);
    }
}