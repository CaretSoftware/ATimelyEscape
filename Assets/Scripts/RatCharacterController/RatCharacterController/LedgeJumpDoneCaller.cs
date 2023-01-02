using UnityEngine;

namespace NewRatCharacterController {
	public class LedgeJumpDoneCaller : MonoBehaviour {

		public void InvokeLedgeJumpDone() {
			LedgeJumpState.ledgeJumpDone?.Invoke();
		}
	}
}