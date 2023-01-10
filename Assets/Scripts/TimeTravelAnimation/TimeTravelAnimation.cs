using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CallbackSystem;


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

    private SkinnedMeshRenderer _meshRenderer;
    private Material[] _defaultMaterials;
    private List<ParticleSystem> _particleSystems;
    private float _initialDistance;
    private float _initialAmount;
    private float _initialFresnelPower;
    private float _initialWobbleSpeed;
    private float _initialTransparency;
    private float _initialSwirlSpeed;
    private bool _playing;


    private void Start()
    {
        _meshRenderer = transform.parent.transform.GetComponentInChildren<SkinnedMeshRenderer>();
        watchEffectRenderer.material.CopyPropertiesFromMaterial(watchEffectMaterial);
        _defaultMaterials = _meshRenderer.materials;
        LoadMaterialProperties();
        LoadParticleSystems();
        TimePeriodChanged.AddListener<TimePeriodChanged>(PlayTimeTravelEffect);
    }

    private void LoadMaterialProperties()
    {
        _initialDistance = timeTravelMaterial.GetFloat("_Distance");
        _initialAmount = timeTravelMaterial.GetFloat("_Amount");
        _initialFresnelPower = timeTravelMaterial.GetFloat("_FresnelPower");
        _initialWobbleSpeed = timeTravelMaterial.GetFloat("_WobbleSpeed");

        if (watchEffectMaterial)
        {
            _initialTransparency = watchEffectRenderer.material.GetFloat("_Transparency");
            _initialSwirlSpeed = watchEffectRenderer.material.GetFloat("_SwirlSpeed");
        }
    }

    private void LoadParticleSystems()
    {
        _particleSystems = new List<ParticleSystem>();
        foreach (Transform child in transform)
        {
            ParticleSystem particles = child.gameObject.GetComponent<ParticleSystem>();
            if (particles)
            {
                _particleSystems.Add(particles);
            }
        }
    }

    private void PlayTimeTravelEffect(TimePeriodChanged e)
    {
        if (e.IsReload) return;

        if (gameObject.activeInHierarchy) {
            if (_playing)
            {
                StopAllCoroutines();
            }

            {
                _playing = true;
                PlayParticles(true);
                StartCoroutine(AnimateTravel());
            }
        }
        else
        {
            _playing = false;
            ResetProperties();
        }
    }

    private IEnumerator AnimateTravel()
    {
        // Start watch-effect
        float elapsedTime = 0f;
        while (elapsedTime < watchStartDuration)
        {
            elapsedTime += Time.deltaTime;
            watchEffectRenderer.material.SetFloat("_Transparency", Ease.EaseInOutSine(Mathf.Lerp(_initialTransparency, watchTransparency, elapsedTime / watchStartDuration)));
            watchEffectRenderer.material.SetFloat("_SwirlSpeed", Ease.EaseInOutSine(Mathf.Lerp(_initialSwirlSpeed, swirlSpeed, elapsedTime / watchStartDuration)));
            yield return null;
        }

        // Start rat body-effect
        SetTimeTravelMaterial(false);
        elapsedTime = 0f;
        while (elapsedTime < startDuration)
        {
            elapsedTime += Time.deltaTime;
            timeTravelMaterial.SetFloat("_Distance", Ease.EaseInBack(Mathf.Lerp(_initialDistance, distance, elapsedTime / startDuration)));
            timeTravelMaterial.SetFloat("_Amount", Ease.EaseInOutSine(Mathf.Lerp(_initialAmount, amount, elapsedTime / startDuration)));
            timeTravelMaterial.SetFloat("_FresnelPower", Ease.EaseOutCubic(Mathf.Lerp(_initialFresnelPower, fresnelPower, elapsedTime / startDuration)));
            timeTravelMaterial.SetFloat("_WobbleSpeed", Ease.EaseInBack(Mathf.Lerp(_initialWobbleSpeed, wobbleSpeed, elapsedTime / startDuration)));
            yield return null;
        }

        // End rat body-effect
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

        // Reset property values
        ResetProperties();

        // Set default material on rat and stop particles
        SetTimeTravelMaterial(true);
        PlayParticles(false);

        // End watch-effect
        elapsedTime = 0f;
        while (elapsedTime < watchEndDuration)
        {
            elapsedTime += Time.deltaTime;
            watchEffectRenderer.material.SetFloat("_Transparency", Ease.EaseInSine(Mathf.Lerp(watchTransparency, 0f, elapsedTime / watchEndDuration)));
            watchEffectRenderer.material.SetFloat("_SwirlSpeed", Ease.EaseInSine(Mathf.Lerp(swirlSpeed, 0.2f, elapsedTime / watchEndDuration)));
            yield return null;
        }

        _playing = false;
    }

    private void SetTimeTravelMaterial(bool reset)
    {
        Material[] mats = _meshRenderer.materials;

        for (int i = 0; i < _defaultMaterials.Length; ++i)
        {
            mats[i] = reset ? _defaultMaterials[i] : timeTravelMaterial;
        }

        _meshRenderer.materials = mats;
    }

    private void PlayParticles(bool play)
    {
        if (_particleSystems != null && _particleSystems.Count > 0)
        {
            for (int i = 0; i < _particleSystems.Count; ++i)
            {
                if (play)
                {
                    _particleSystems[i].Play();
                }
                else
                {
                    _particleSystems[i].Stop();
                }
            }
        }
    }

    private void ResetProperties()
    {
        timeTravelMaterial.SetFloat("_Distance", 0f);
        timeTravelMaterial.SetFloat("_Amount", 0f);
        timeTravelMaterial.SetFloat("_FresnelPower", 0f);
        timeTravelMaterial.SetFloat("_WobbleSpeed", 0f);

        watchEffectRenderer.material.SetFloat("_Transparency", 0f);
        watchEffectRenderer.material.SetFloat("_SwirlSpeed", 0.2f);
    }

    private void OnApplicationQuit()
    {
        // Make sure values are reset on application quit
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
