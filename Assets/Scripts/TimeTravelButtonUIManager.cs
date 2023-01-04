using System;using UnityEngine;

public class TimeTravelButtonUIManager : MonoBehaviour {
	public delegate void TimeTravelButtonActive(TimeTravelPeriod period, bool active);
	public static TimeTravelButtonActive buttonActiveDelegate;
	
	public delegate void TimeTravelButtonPressed(
			TimeTravelPeriod period, 
			TimeTravelPeriod currentTimePeriod, 
			bool canTimeTravelToPeriod);
	public static TimeTravelButtonPressed buttonPressedDelegate;
	
	[SerializeField] private TimeTravelUIButton[] timeTravelUIButtons;

	private void Awake() {
		buttonActiveDelegate += TimeTravelButton;
		buttonPressedDelegate += ButtonPressed;
	}

	private void OnDestroy() {
		buttonActiveDelegate -= TimeTravelButton;
		buttonPressedDelegate -= ButtonPressed;
	}

	private void TimeTravelButton(TimeTravelPeriod period, bool active) {
		timeTravelUIButtons[(int)period].ActivateButton(active);
	}
	
	private void ButtonPressed(TimeTravelPeriod period, TimeTravelPeriod currentTimePeriod, bool canTimeTravelToPeriod) {
		timeTravelUIButtons[(int)period].PressedButton(canTimeTravelToPeriod);
	}
}