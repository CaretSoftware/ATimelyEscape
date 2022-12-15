using UnityEngine;

public static class Normal {
	
	public static Vector2 Force(Vector2 force, Vector2 normal) {
		float projection = Mathf.Min(0.0f, Vector2.Dot(force, normal));
		return -(projection * normal);
	}
	
	public static Vector3 Force(Vector3 force, Vector3 normal) {
		float projection = Mathf.Min(0.0f, Vector3.Dot(force, normal));
		return -(projection * normal);
	}
}