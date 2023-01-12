using System;
using UnityEditor;
using UnityEngine;


public class CubeSideActivator : MonoBehaviour {
	private Vector3 debug;
	private Vector3[] sides = {Vector3.left, Vector3.right, Vector3.back, Vector3.forward};
	[Tooltip("Left, Right, Back Forward — objects local transform directions in that order!"), SerializeField] 
	private GameObject[] objects;
	
	private float maxDistance;

	private CanvasGroup canvasGroup;

	private Transform player;

	private int activeUI;


	// Start is called before the first frame update
	void Start()
	{
		
		maxDistance = gameObject.GetComponent<SphereCollider>().radius * transform.localScale.x;
		canvasGroup = gameObject.GetComponent<CanvasGroup>();

		canvasGroup.alpha = 0;
	}

	private void FixedUpdate()
	{
		if (player != null) {
			ActivateOnlyClosestSide();
		}

		void ActivateOnlyClosestSide() {
		
			Vector3 directionToPlayer = 
				transform.InverseTransformDirection( (player.position - transform.position).normalized );
			int closestSide = -1;
			float largestDotProduct = -1;
			
			for (int side = 0; side < sides.Length; side++) {
				float sideDotProduct = Vector3.Dot(directionToPlayer, sides[side]);    
           
				if (sideDotProduct > largestDotProduct) {    // if this side is pointing closer than previous sides
					largestDotProduct = sideDotProduct;
					closestSide = side;
				}
			}

			for (int obj = 0; obj < objects.Length; obj++) {
				//objects[obj].SetActive(obj == closestSide);	// set active if it’s the closest side

				if (obj == closestSide)
					objects[obj].GetComponent<FadeScript>().FadeIn();
				else
					objects[obj].GetComponent<FadeScript>().FadeOut();
			}
		}
	}


	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			player = other.gameObject.transform;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			player = null;
			canvasGroup.alpha = 0;
		}
	}

#if UNITY_EDITOR
	private void OnDrawGizmos() {
		if (!Application.isPlaying) return;
		
		Color prev = Gizmos.color;
		Gizmos.color = Color.red;
		
		Gizmos.DrawSphere(debug, .5f);
		
		Gizmos.color = prev;
	}
#endif
}
