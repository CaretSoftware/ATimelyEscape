
using UnityEngine;

public static class ExtensionMethods {
   public static Vector3 ToVector3(this Vector2 vector2) {
      return new Vector3(vector2.x, 0.0f, vector2.y);
   }
   
   public static Vector2 ToVector2(this Vector3 vector3) {
      return new Vector2(vector3.x, vector3.z);
   }

   public static Vector3 ProjectOnPlane(this Vector3 vector3) {
      return Vector3.ProjectOnPlane(vector3, Vector3.up);
   }
}