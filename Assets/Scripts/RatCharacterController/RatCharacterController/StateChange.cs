using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StateChange : MonoBehaviour {
	private static TextMeshProUGUI _text;
	public delegate void StateUpdate(string state);
	public static StateUpdate stateUpdate;
	
	private void Awake() {
		
		_text = GetComponent<TextMeshProUGUI>();
		stateUpdate = MessageHandler;
	}

	private static void MessageHandler(string state) => _text.text = state;
}
