using UnityEngine;
using TMPro;

[RequireComponent(typeof(SphereCollider))]
public class WatchPickup : MonoBehaviour {
	
	[SerializeField] private float rotationSpeed = 25.0f;
	[SerializeField] private TextMeshProUGUI instructions;
	private NewRatCharacterController.NewCharacterInput _characterInput;

	private void Awake() {
		GetComponent<SphereCollider>().isTrigger = true;
		transform.position += Vector3.up * .1f;
		Rigidbody _rb = GetComponent<Rigidbody>();
		if (_rb != null) {
			_rb.isKinematic = true;
		}
		_characterInput = FindObjectOfType<NewRatCharacterController.NewCharacterInput>();
	}

	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Player")) {

			if (instructions != null)
				instructions.text = "Use the timetravel device <sprite name=\"X\"> to timetravel one year to the past";
			FindObjectOfType<AudioManager>().Play("2");
			_characterInput.CanTimeTravel = true;
			_characterInput.CanTimeTravelPresent = false;
			_characterInput.CanTimeTravelFuture = false;
			Destroy(this.gameObject);
		}
	}

	private Vector3 rot = new Vector3(.23f, .7f, .13f);
	private void Update() {
		transform.rotation *=
			Quaternion.Euler(Time.unscaledDeltaTime * rot * rotationSpeed).normalized;
	}
}
