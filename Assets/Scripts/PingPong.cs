using UnityEngine;

public class PingPong : MonoBehaviour {
   private static readonly int VoronoiAngleOffset = Shader.PropertyToID("_VoronoiAngleOffset");
   private static readonly int PowerLevel = Shader.PropertyToID("_PowerLevel");
   private MaterialPropertyBlock mpb;
   private MeshRenderer mr;
   private float _t;

   private void Awake() {
      mpb = new MaterialPropertyBlock();
      mr = GetComponent<MeshRenderer>();
   }

   public void SetPower(float power) {
      mpb.SetFloat(PowerLevel, power);
      mr.SetPropertyBlock(mpb);
   }

   // private void Update() {
      // _t = Mathf.Sin(Time.time * 2.0f) * .5f + .5f;
      // _t = Time.time * .25f;
      // VoronoiOffset(_t);
   // }

   private void VoronoiOffset(float t) {
      // float offset = Mathf.Lerp(-2, 2, t);
      // Debug.Log(offset);
      mpb.SetFloat(VoronoiAngleOffset, t);
      mr.SetPropertyBlock(mpb);
   }
}
