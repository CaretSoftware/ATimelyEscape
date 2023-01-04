using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CallbackSystem;
using RatCharacterController;

public class TimeTravelAnimation : MonoBehaviour
{
    [SerializeField] private Material timeTravelMaterial;
    [SerializeField] private Material watchEffectMaterial;
    [SerializeField] private MeshRenderer watchEffectRenderer;
    [SerializeField] private float watchStartDuration = 0.01f;
    [SerializeField] private float watchEndDuration = 3f;
    [SerializeField] private float startDuration = 0.01f;
    [SerializeField] private float endDuration = 0.5f;
    [SerializeField] private float distance = 0.2f;
    [SerializeField] private float amount = 0.2f;
    [SerializeField] private float fresnelPower = 2f;
    [SerializeField] private float wobbleSpeed = 2f;
    [SerializeField] private float watchTransparency = 0.5f;
    [SerializeField] private float swirlSpeed = 1f;

    private SkinnedMeshRenderer meshRenderer;
    private Material[] defaultMaterials;
    private List<ParticleSystem> particleSystems;
    private float initialDistance;
    private float initialAmount;
    private float initialFresnelPower;
    private float initialWobbleSpeed;
    private float initialTransparency;
    private float initialSwirlSpeed;
    private bool playing;


    private void Start()
    {
        meshRenderer = transform.parent.transform.GetComponentInChildren<SkinnedMeshRenderer>();
        watchEffectRenderer.material.CopyPropertiesFromMaterial(watchEffectMaterial);
        defaultMaterials = meshRenderer.materials;
        LoadMaterialProperties();
        LoadParticleSystems();
        TimePeriodChanged.AddListener<TimePeriodChanged>(PlayTimeTravelEffect);
    }

    private void LoadMaterialProperties()
    {
        initialDistance = timeTravelMaterial.GetFloat("_Distance");
        initialAmount = timeTravelMaterial.GetFloat("_Amount");
        initialFresnelPower = timeTravelMaterial.GetFloat("_FresnelPower");
        initialWobbleSpeed = timeTravelMaterial.GetFloat("_WobbleSpeed");

        if (watchEffectMaterial)
        {
            initialTransparency = watchEffectRenderer.material.GetFloat("_Transparency");
            initialSwirlSpeed = watchEffectRenderer.material.GetFloat("_SwirlSpeed");
        }
    }

    private void LoadParticleSystems()
    {
        particleSystems = new List<ParticleSystem>();
        foreach (Transform child in transform)
        {
            ParticleSystem particles = child.gameObject.GetComponent<ParticleSystem>();
            if (particles)
            {
                particleSystems.Add(particles);
            }
        }
    }

    private void PlayTimeTravelEffect(TimePeriodChanged e)
    {
        if (e.IsReload) return;

        if (playing)
        {
            StopAllCoroutines();
        }

        playing = true;
        PlayParticles(true);
        StartCoroutine(AnimateTravel());
    }

    private IEnumerator AnimateTravel()
    {
        float elapsedTime = 0f;
        while (elapsedTime < watchStartDuration)
        {
            elapsedTime += Time.deltaTime;
            watchEffectRenderer.material.SetFloat("_Transparency", Ease.EaseInOutSine(Mathf.Lerp(initialTransparency, watchTransparency, elapsedTime / watchStartDuration)));
            watchEffectRenderer.material.SetFloat("_SwirlSpeed", Ease.EaseInOutSine(Mathf.Lerp(initialSwirlSpeed, swirlSpeed, elapsedTime / watchStartDuration)));
            yield return null;
        }

        SetTimeTravelMaterial(false);
        elapsedTime = 0f;
        while (elapsedTime < startDuration)
        {
            elapsedTime += Time.deltaTime;
            timeTravelMaterial.SetFloat("_Distance", Ease.EaseInBack(Mathf.Lerp(initialDistance, distance, elapsedTime / startDuration)));
            timeTravelMaterial.SetFloat("_Amount", Ease.EaseInOutSine(Mathf.Lerp(initialAmount, amount, elapsedTime / startDuration)));
            timeTravelMaterial.SetFloat("_FresnelPower", Ease.EaseOutCubic(Mathf.Lerp(initialFresnelPower, fresnelPower, elapsedTime / startDuration)));
            timeTravelMaterial.SetFloat("_WobbleSpeed", Ease.EaseInBack(Mathf.Lerp(initialWobbleSpeed, wobbleSpeed, elapsedTime / startDuration)));
            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < endDuration)
        {
            elapsedTime += Time.deltaTime;
            timeTravelMaterial.SetFloat("_Distance", Ease.EaseInSine(Mathf.Lerp(distance, 0f, elapsedTime / endDuration)));
            timeTravelMaterial.SetFloat("_Amount", Ease.EaseInSine(Mathf.Lerp(amount, 0f, elapsedTime / endDuration)));
            timeTravelMaterial.SetFloat("_FresnelPower", Ease.EaseInSine(Mathf.Lerp(fresnelPower, 0f, elapsedTime / endDuration)));
            timeTravelMaterial.SetFloat("_WobbleSpeed", Ease.EaseInSine(Mathf.Lerp(wobbleSpeed, 0f, elapsedTime / endDuration)));
            yield return null;
        }

        timeTravelMaterial.SetFloat("_Distance", 0f);
        timeTravelMaterial.SetFloat("_Amount", 0f);
        timeTravelMaterial.SetFloat("_FresnelPower", 0f);
        timeTravelMaterial.SetFloat("_WobbleSpeed", 0f);

        watchEffectRenderer.material.SetFloat("_Transparency", 0f);
        watchEffectRenderer.material.SetFloat("_SwirlSpeed", 0.2f);

        SetTimeTravelMaterial(true);
        PlayParticles(false);

        elapsedTime = 0f;
        while (elapsedTime < watchEndDuration)
        {
            elapsedTime += Time.deltaTime;
            watchEffectRenderer.material.SetFloat("_Transparency", Ease.EaseInSine(Mathf.Lerp(watchTransparency, 0f, elapsedTime / watchEndDuration)));
            watchEffectRenderer.material.SetFloat("_SwirlSpeed", Ease.EaseInSine(Mathf.Lerp(swirlSpeed, 0.2f, elapsedTime / watchEndDuration)));
            yield return null;
        }

        playing = false;
    }

    private void SetTimeTravelMaterial(bool reset)
    {
        Material[] mats = meshRenderer.materials;

        for (int i = 0; i < defaultMaterials.Length; ++i)
        {
            mats[i] = reset ? defaultMaterials[i] : timeTravelMaterial;
        }

        meshRenderer.materials = mats;
    }

    private void PlayParticles(bool play)
    {
        if (particleSystems != null && particleSystems.Count > 0)
        {
            for (int i = 0; i < particleSystems.Count; ++i)
            {
                if (play)
                {
                    particleSystems[i].Play();
                }
                else
                {
                    particleSystems[i].Stop();
                }
            }
        }
    }

    private void OnApplicationQuit()
    {
        SetTimeTravelMaterial(true);
        timeTravelMaterial.SetFloat("_Distance", 0f);
        timeTravelMaterial.SetFloat("_Amount", 0f);
        timeTravelMaterial.SetFloat("_FresnelPower", 0f);
        timeTravelMaterial.SetFloat("_WobbleSpeed", 0f);
    }

    private void OnDestroy() {
        if(EventSystem.Current != null) TimePeriodChanged.RemoveListener<TimePeriodChanged>(PlayTimeTravelEffect);
    }
}
