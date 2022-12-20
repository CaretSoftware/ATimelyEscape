using CallbackSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


namespace CallbackSystem
{
    public class LightDetection : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("The camera who scans for light.")]
        public Camera lightScan;
        [Tooltip("Show the light value in the log.")]
        public bool lightValueLog = false;
        [Tooltip("Time between light value updates")]
        public float updateTimeForDetector = 0.1f;
        [Tooltip("Time between darkness updates")]
        public float updateTimeForDarkness = 0f;
        [Tooltip("Less light than this and Darkness starts coming")]
        public float minumumLight = .3f;
        [Tooltip("This is the speed att which the deaths approaches in just under minimumLight")]
        public float deathTimeInMinumumLight = 25f;
        [Tooltip("This is the speed att which the deaths approaches in Total Darkness")]
        public float deathTimeInTotalDarkness = 1f;
        [Tooltip("This is the speed att which the light returnes in just at minimumLight")]
        public float lightTimeInMinumumLight = 5f;
        [Tooltip("This is the speed att which the light returnes in Total Darkness")]
        public float lightTimeInTotalLight = 1f;



        public static float lightValue;

        private const int textureSize = 1;

        private Texture2D texLight;
        private RenderTexture texTemp;
        private Rect rectLight;
        private Color lightPixel;
        private float timer = 0f;
        private FailStateEvent fail;


        [SerializeField] private Volume postProcesingVolume;
        private Vignette vignette;


        private void Start()
        {
            texLight = new Texture2D(textureSize, textureSize, TextureFormat.RGB24, false);
            texTemp = new RenderTexture(textureSize, textureSize, 24);
            rectLight = new Rect(0f, 0f, textureSize, textureSize);
            SetVignetteModifierEvent.AddListener<SetVignetteModifierEvent>(SetVignetteModifier);
            TimePeriodChanged.AddListener<TimePeriodChanged>(ActivateLightDetection);

            postProcesingVolume.profile.TryGet(out vignette);
            fail = new();
        }

        private void ActivateLightDetection(TimePeriodChanged obj)
        {
            if(obj.to == TimeTravelPeriod.Future)
            {
                StartCoroutine(LightDetectionUpdate(updateTimeForDetector));
                StartCoroutine(Darkness(updateTimeForDarkness));
            }
            else
            {
                StopAllCoroutines();
                if(vignette != null)
                    vignette.intensity.value = timer;
            }
        }

        private IEnumerator Darkness(float updateTime)
        {
            while (true || vignette.intensity.value < 1)
            {
                yield return new WaitForSeconds(updateTime);
                if (lightValue < minumumLight)
                {
                    //timer += (Time.deltaTime / Mathf.Lerp(timeInTotalDarkness, timeInMinumumLight, lightValue * (1 / minumumLight)));
                    timer += (updateTime / Mathf.Lerp(deathTimeInTotalDarkness, deathTimeInMinumumLight, (lightValue / minumumLight)));
                }
                else if(lightValue >= minumumLight && timer > 0)
                {
                    timer -= (updateTime / Mathf.Lerp(lightTimeInMinumumLight, lightTimeInTotalLight, (lightValue - minumumLight) / (1 - minumumLight) ));
                }
                if (timer >= 1)
                {
                    LightDetectior();
                    if (lightValue < minumumLight)
                    {
                        timer = 0f;
                        fail.Invoke();
                        OnboardingHandler.TimeTravelFutureDiscovered = true;
                    }
                }
                vignette.intensity.value = timer;
    
            }
        }


        private void StartLightDetection()
        {
            texLight = new Texture2D(textureSize, textureSize, TextureFormat.RGB24, false);
            texTemp = new RenderTexture(textureSize, textureSize, 24);
            rectLight = new Rect(0f, 0f, textureSize, textureSize);

            StartCoroutine(LightDetectionUpdate(updateTimeForDetector));
        }

        private IEnumerator LightDetectionUpdate(float updateTime)
        {
            while (true)
            {
                LightDetectior();
                yield return new WaitForSeconds(updateTime);
            }
        }

        private void LightDetectior()
        {
            lightScan.targetTexture = texTemp;

            lightScan.Render();

            RenderTexture.active = texTemp;

            texLight.ReadPixels(rectLight, 0, 0);

            RenderTexture.active = null;

            lightScan.targetTexture = null;

            lightPixel = texLight.GetPixel(textureSize / 2, textureSize / 2);

            lightValue = (lightPixel.r + lightPixel.g + lightPixel.b) / 3f;

            if (lightValueLog)
            {
                Debug.Log("Light Value: " + lightValue);
            }
        }

        public float GetFailTimer()
        {
            return timer;
        }

        public void SetVignetteModifier(SetVignetteModifierEvent setVignetteModifierEvent)
        {
            vignette = setVignetteModifierEvent.vignette;
        }
    }
}