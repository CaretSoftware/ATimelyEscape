using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Ping = System.Net.NetworkInformation.Ping;

public class PingPong : MonoBehaviour {
   private static readonly int VoronoiAngleOffset = Shader.PropertyToID("_VoronoiAngleOffset");
   [SerializeField] private Material mat;
   private MaterialPropertyBlock mpb;
   private MeshRenderer mr;
   private float _t;

   private void Awake() {
      mpb = new MaterialPropertyBlock();
      mr = GetComponent<MeshRenderer>();
   }


   private void Update() {
      _t = Mathf.Sin(Time.time) * .5f + .5f;
      VoronoiOffset(_t);
   }

   private void VoronoiOffset(float t) {
      float offset = Mathf.Lerp(-1, 1, t);
      Debug.Log(offset);
      mpb.SetFloat(VoronoiAngleOffset, offset);
      mr.SetPropertyBlock(mpb);
   }
}
