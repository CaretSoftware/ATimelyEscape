using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TimeZoneTravelSetter : MonoBehaviour {

	[Header("Timezones on")]
	[SerializeField]
	private bool PastOn;
	[SerializeField]
	private bool PresentOn;
	[SerializeField]
	private bool FutureOn;
	[SerializeField] private TimeTravelPeriod travelToThisPeriod = TimeTravelPeriod.Dummy;

	private void Awake() {
		Rigidbody rb = GetComponent<Rigidbody>();
		GetComponent<MeshRenderer>().enabled = false; 

		rb.isKinematic = true;
		rb.useGravity = false;
	}

	private void OnTriggerEnter(Collider other) {
		if (other.gameObject.CompareTag("Player")) {
			print("Collision");
			NewRatCharacterController.NewCharacterInput rat =
				other.GetComponent<NewRatCharacterController.NewCharacterInput>();

			rat.CanTimeTravelPast = PastOn;
			rat.CanTimeTravelPresent = PresentOn;
			rat.CanTimeTravelFuture = FutureOn;

			if (travelToThisPeriod != TimeTravelPeriod.Dummy) { TimeTravelManager.DesiredTimePeriod(travelToThisPeriod); print(TimeTravelManager.currentPeriod); }
		}
	}
}
