using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CallbackSystem;

public class TimeTravelUIButton : MonoBehaviour {
	[SerializeField] private ParticleSystem particleSystem;
	[SerializeField] private MeshRenderer meshRenderer;
	[SerializeField] private TimeTravelPeriod timeTravelPeriod;
	
	private static TimeTravelPeriod _lastPressedPeriod;
	
	private static readonly int EmissionColorPropertyID = Shader.PropertyToID("_EmissionColor");
	private static readonly int AlphaClippingPropertyID = Shader.PropertyToID("_Cutoff");

	private Transform _buttonTransform;
	private float _buttonPressLength = 0.0043f;
	private float pressAnimationTime = .2f;
	private float turnOnAnimationTime = .5f;
	private float fadeOutAnimationTime = .2f;
	private float shakeButtonAnimationTime = .3f;
	private float _defaultAlphaClipping = .88f;
	private float _pulseAlphaClipping = .66f;
	
	private Vector3 _localStartPosition;
	private Vector3 _localScale;
	private Color _emissionColor;
	
	private MaterialPropertyBlock _mpb;
	
	private bool _blocked;

	private void Awake() {
		_mpb = new MaterialPropertyBlock();
		_buttonTransform = meshRenderer.transform;
		_emissionColor = meshRenderer.material.GetColor(EmissionColorPropertyID);
		_localStartPosition = _buttonTransform.localPosition;
		_localScale = _buttonTransform.localScale;
	}

	private void Start() {
		CallHintAnimation.AddListener<CallHintAnimation>(Blocked);
		DebugEvent.AddListener<TimePeriodChanged>(TimePeriodChanged);
	}

	private void OnDestroy() {
		CallHintAnimation.RemoveListener<CallHintAnimation>(Blocked);
		DebugEvent.RemoveListener<TimePeriodChanged>(TimePeriodChanged);
	}

	
	private void Update() {
		if (TimeTravelManager.currentPeriod == timeTravelPeriod) {
			_mpb.SetFloat(AlphaClippingPropertyID, _defaultAlphaClipping);
			meshRenderer.SetPropertyBlock(_mpb);
			return;
		}
			
		float t = Mathf.Sin(Time.unscaledTime * Mathf.PI * 2f) * .5f + .5f;
		float pulse = Mathf.Lerp(_defaultAlphaClipping, _pulseAlphaClipping, t);
		_mpb.SetFloat(AlphaClippingPropertyID, pulse);
		meshRenderer.SetPropertyBlock(_mpb);
	}
	
	private void TimePeriodChanged(TimePeriodChanged t) {
		FadeButton(t.to == timeTravelPeriod);
	}

	// pulsate button Coroutine

	private void Blocked(CallHintAnimation c) {
		if (c.animationName.Equals("TravelWarning") && _lastPressedPeriod == timeTravelPeriod)
			AnimateButtonBlocked();
	}

	private void AnimateButtonBlocked() {
		_blocked = true;
		StartCoroutine(ShakeButton());
	}

	public float shakeFrequency = 5f;
	public float shakeMagnitude = .001f;
	private IEnumerator ShakeButton() {
		float t = 0f;
		
		while (t < 1f) {
			float sin = Mathf.Sin(Ease.EaseInQuart(1f - t) * Mathf.PI);
			_buttonTransform.localPosition = 
				_localStartPosition 
				+ _buttonTransform.InverseTransformDirection(_buttonTransform.right) 
				* (shakeMagnitude * sin) 
				* Mathf.Sin(t * shakeFrequency);

			t += Time.unscaledDeltaTime * (1f / shakeButtonAnimationTime);
			yield return null;
		}

		_buttonTransform.localPosition = _localStartPosition;
	}

	public void ActivateButton(bool active) {
		if (active) {
			meshRenderer.enabled = true;
			StartCoroutine(TurnOnButton());
		} else {
			PlayParticleSystem();
			meshRenderer.enabled = false;
		}
	}

	private void FadeButton(bool fade) {
		if (fade)
			StartCoroutine(FadeButtonOut());
		else {
			_mpb.SetColor(EmissionColorPropertyID, _emissionColor.gamma);
			meshRenderer.SetPropertyBlock(_mpb);
		}
	}

	private IEnumerator FadeButtonOut() {
		float t = 0f;

		while (t < 1.0f && !_blocked) {
			Color color = Color.Lerp(_emissionColor.gamma, Color.grey, Ease.EaseInQuart(t));
			_mpb.SetColor(EmissionColorPropertyID, color.gamma);
			meshRenderer.SetPropertyBlock(_mpb);
			t += Time.unscaledDeltaTime * (1f / fadeOutAnimationTime);
			yield return null;
		}

		_mpb.SetColor(EmissionColorPropertyID,
			TimeTravelManager.currentPeriod == timeTravelPeriod ? Color.grey : _emissionColor.gamma);

		meshRenderer.SetPropertyBlock(_mpb);
	}

	private void PlayParticleSystem() {
		particleSystem.Play();
	}

	public void PressedButton(bool canTimeTravelToPeriod) {
		_blocked = false;
		_lastPressedPeriod = timeTravelPeriod;
		
		if (!canTimeTravelToPeriod)
			return;

		StartCoroutine(ButtonPressedAnimation());
	}

	private IEnumerator TurnOnButton() {
		float t = 0f;
		_buttonTransform.localScale = Vector3.zero;

		while (t < 1f) {
			_buttonTransform.localScale = Vector3.LerpUnclamped(Vector3.zero, _localScale, Ease.EaseOutBack(t));

			t += Time.unscaledDeltaTime * (1f / turnOnAnimationTime);
			yield return true;
		}
		_buttonTransform.localScale = _localScale;
	}

	private IEnumerator ButtonPressedAnimation() {
		if (_blocked) {
			_buttonTransform.localPosition = _localStartPosition;
			yield break;
		} 
		
		float t = 0f;

		while (t < 1.0f && !_blocked) {
			float f = Mathf.LerpUnclamped(Ease.EaseOutQuint(t), Ease.EaseInElastic(t), Ease.EaseInQuint(t));
			_buttonTransform.localPosition = Vector3.LerpUnclamped(_localStartPosition, _localStartPosition + _buttonTransform.InverseTransformDirection(_buttonTransform.forward) * _buttonPressLength, f);
			t += Time.unscaledDeltaTime * (1f / pressAnimationTime);
			yield return null;
		}
		_buttonTransform.localPosition = _localStartPosition;
	}
	
	[ContextMenu("Turn On")]
	public void TurnOn() {
		ActivateButton(true);
	}
	
	[ContextMenu("Turn Off")]
	public void TurnOff() {
		ActivateButton(false);
	}
}
