using UnityEngine;
using TMPro;

[RequireComponent(typeof(SphereCollider))]
public class WatchPickup : MonoBehaviour {
	
	[SerializeField] private float rotationSpeed = 25.0f;
	[SerializeField] private TextMeshProUGUI instructions;
	private NewRatCharacterController.NewCharacterInput characterInput;

	private void Awake() {
		GetComponent<SphereCollider>().isTrigger = true;
		transform.position += Vector3.up * .1f;
		Rigidbody _rb = GetComponent<Rigidbody>();
		if (_rb != null) {
			_rb.isKinematic = true;
		}
		characterInput = FindObjectOfType<NewRatCharacterController.NewCharacterInput>();
	}

	private void OnTriggerEnter(Collider other)
	{
		//Debug.Log("PICKUP TRIGGER");
		if (other.CompareTag("Player")) {
			//Debug.Log("PICKUP TRIGGER");

			if (instructions != null)
				instructions.text = "Use the timetravel device <sprite name=\"X\"> to timetravel one year to the past";
			FindObjectOfType<AudioManager>().Play("2");
			characterInput.CanTimeTravel = true;
			characterInput.CanTimeTravelPresent = false;
			characterInput.CanTimeTravelFuture = false;
			Destroy(this.gameObject);
			//Debug.Log("PICKUP TRIGGER");
		}
	}

	private Vector3 rot = new Vector3(.23f, .7f, .13f);
	// public bool debug;
	private void Update() {
		// Time.timeScale = debug ? 0.0f : 1.0f;
		
		transform.rotation *=
			Quaternion.Euler(Time.unscaledDeltaTime * rot * rotationSpeed).normalized;
	}
}
