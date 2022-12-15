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
    [SerializeField] private float watchStartDuration = 0.5f;
    [SerializeField] private float watchEndDuration = 3f;
    [SerializeField] private float startDuration = 0.5f;
    [SerializeField] private float endDuration = 0.5f;
    [SerializeField] private float distance = 0.2f;
    [SerializeField] private float amount = 0.2f;
    [SerializeField] private float fresnelPower = 2f;
    [SerializeField] private float wobbleSpeed = 2f;
    [SerializeField] private float watchTransparency = 0.5f;
    [SerializeField] private float swirlSpeed = 1f;

    //private Transform playerTransform;
    private SkinnedMeshRenderer meshRenderer;
    private Material[] defaultMaterials;
    private MaterialPropertyBlock _matPropBlock; 
    private MaterialPropertyBlock _watchEffectMatPropBlock;
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
        //playerTransform = FindObjectOfType<CharacterInput>().transform;
        meshRenderer = transform.parent.transform.GetComponentInChildren<SkinnedMeshRenderer>();
        defaultMaterials = meshRenderer.materials;
        LoadMaterialProperties();
        LoadParticleSystems();
        _matPropBlock = new MaterialPropertyBlock();
        _watchEffectMatPropBlock = new MaterialPropertyBlock();
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
            initialTransparency = watchEffectMaterial.GetFloat("_Transparency");
            initialSwirlSpeed = watchEffectMaterial.GetFloat("_SwirlSpeed");
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
            if(_watchEffectMatPropBlock != null && watchEffectRenderer != null && watchEffectMaterial != null)
            {
                watchEffectMaterial.SetFloat("_Transparency", Ease.EaseInOutSine(Mathf.Lerp(initialTransparency, watchTransparency, elapsedTime / watchStartDuration)));
                watchEffectMaterial.SetFloat("_SwirlSpeed", Ease.EaseInOutSine(Mathf.Lerp(initialSwirlSpeed, swirlSpeed, elapsedTime / watchStartDuration)));

                watchEffectRenderer.SetPropertyBlock(_watchEffectMatPropBlock);           
            }
            yield return null;
        }

        SetTimeTravelMaterial(false);
        elapsedTime = 0f;
        while (elapsedTime < startDuration)
        {
            elapsedTime += Time.deltaTime;
            if (_matPropBlock != null)
            {
                timeTravelMaterial.SetFloat("_Distance", Ease.EaseInBack(Mathf.Lerp(initialDistance, distance, elapsedTime / startDuration)));
                timeTravelMaterial.SetFloat("_Amount", Ease.EaseInOutSine(Mathf.Lerp(initialAmount, amount, elapsedTime / startDuration)));
                timeTravelMaterial.SetFloat("_FresnelPower", Ease.EaseOutCubic(Mathf.Lerp(initialFresnelPower, fresnelPower, elapsedTime / startDuration)));
                timeTravelMaterial.SetFloat("_WobbleSpeed", Ease.EaseInBack(Mathf.Lerp(initialWobbleSpeed, wobbleSpeed, elapsedTime / startDuration)));

                meshRenderer.SetPropertyBlock(_matPropBlock);
            }
            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < endDuration)
        {
            elapsedTime += Time.deltaTime;
            if (_matPropBlock != null)
            {
                timeTravelMaterial.SetFloat("_Distance", Ease.EaseInSine(Mathf.Lerp(distance, 0f, elapsedTime / endDuration)));
                timeTravelMaterial.SetFloat("_Amount", Ease.EaseInSine(Mathf.Lerp(amount, 0f, elapsedTime / endDuration)));
                timeTravelMaterial.SetFloat("_FresnelPower", Ease.EaseInSine(Mathf.Lerp(fresnelPower, 0f, elapsedTime / endDuration)));
                timeTravelMaterial.SetFloat("_WobbleSpeed", Ease.EaseInSine(Mathf.Lerp(wobbleSpeed, 0f, elapsedTime / endDuration)));

                meshRenderer.SetPropertyBlock(_matPropBlock);
            }
            yield return null;
        }

        if (_matPropBlock != null)
        {
            timeTravelMaterial.SetFloat("_Distance", 0f);
            timeTravelMaterial.SetFloat("_Amount", 0f);
            timeTravelMaterial.SetFloat("_FresnelPower", 0f);
            timeTravelMaterial.SetFloat("_WobbleSpeed", 0f);

            meshRenderer.SetPropertyBlock(_matPropBlock);
        }

        if (_watchEffectMatPropBlock != null && watchEffectRenderer != null && watchEffectMaterial != null)
        {
            watchEffectMaterial.SetFloat("_Transparency", 0f);
            watchEffectMaterial.SetFloat("_SwirlSpeed", 0.2f);

            watchEffectRenderer.SetPropertyBlock(_watchEffectMatPropBlock);
        }


        SetTimeTravelMaterial(true);
        PlayParticles(false);

        elapsedTime = 0f;
        while (elapsedTime < watchEndDuration)
        {
            elapsedTime += Time.deltaTime;
            if (_watchEffectMatPropBlock != null && watchEffectRenderer != null && watchEffectMaterial != null)
            {
                watchEffectMaterial.SetFloat("_Transparency", Ease.EaseInSine(Mathf.Lerp(watchTransparency, 0f, elapsedTime / watchEndDuration)));
                watchEffectMaterial.SetFloat("_SwirlSpeed", Ease.EaseInSine(Mathf.Lerp(swirlSpeed, 0.2f, elapsedTime / watchEndDuration)));

                watchEffectRenderer.SetPropertyBlock(_watchEffectMatPropBlock);
            }
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
        if(particleSystems != null && particleSystems.Count > 0)
        {
            for(int i = 0; i < particleSystems.Count; ++i)
            {
                if (play)
                {
                    particleSystems[i].Play();
                }else
                {
                    particleSystems[i].Stop();
                }
            }
        }
    }
}
