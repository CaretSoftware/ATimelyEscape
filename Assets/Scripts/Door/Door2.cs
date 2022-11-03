using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door2 : MonoBehaviour {
	[SerializeField] private Animator animator;
	private static readonly int Open = Animator.StringToHash("Open");

	public void SetDoor(bool open) {
		animator.SetBool(Open, open);
	}
}
