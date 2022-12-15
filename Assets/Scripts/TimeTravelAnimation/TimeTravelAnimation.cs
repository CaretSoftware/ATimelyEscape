using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CallbackSystem;
using RatCharacterController;

public class TimeTravelAnimation : MonoBehaviour
{
    [SerializeField] private GameObject particles;
    [SerializeField] private Material timeTravelMaterial;
    [SerializeField] private float distance = 0.2f;
    [SerializeField] private float amount = 0.2f;
    [SerializeField] private float fresnelPower = 2f;
    [SerializeField] private float wobbleSpeed = 2f;


    private Transform playerTransform;
    private SkinnedMeshRenderer meshRenderer;
    private Material[] defaultMaterials;
    private MaterialPropertyBlock _matPropBlock;
    private bool playing;


    private void Start()
    {
        playerTransform = FindObjectOfType<CharacterInput>().transform;
        meshRenderer = playerTransform.GetComponentInChildren<SkinnedMeshRenderer>();
        defaultMaterials = meshRenderer.materials;
        _matPropBlock = new MaterialPropertyBlock();
        TimePeriodChanged.AddListener<TimePeriodChanged>(PlayTimeTravelEffect);
    }

    private void PlayTimeTravelEffect(TimePeriodChanged e)
    {
        if (playing)
        {
            StopAllCoroutines();
        }

        SetTimeTravelMaterial(false);
        playing = true;
        if (particles)
        {
            particles.SetActive(true);
        }
        StartCoroutine(AnimateTravel(true));
    }

    private IEnumerator AnimateTravel(bool fadeIn)
    {
        float initialDistance = timeTravelMaterial.GetFloat("_Distance");
        float initialAmount = timeTravelMaterial.GetFloat("_Amount");
        float initialFresnelPower = timeTravelMaterial.GetFloat("_FresnelPower");
        float initialWobbleSpeed = timeTravelMaterial.GetFloat("_WobbleSpeed");

        float targetDistance = fadeIn ? distance : 0;
        float targetAmount = fadeIn ? amount : 0;
        float targetFresnelPower = fadeIn ? fresnelPower : 0;
        float targetWobbleSpeed = fadeIn ? wobbleSpeed : 0;

        float elapsedTime = 0f;

        while (elapsedTime < 0.5f)
        {
            elapsedTime += Time.deltaTime;

            if (_matPropBlock != null)
            {
                timeTravelMaterial.SetFloat("_Distance", Ease.EaseInBack(Mathf.Lerp(initialDistance, targetDistance, elapsedTime / 0.5f)));
                timeTravelMaterial.SetFloat("_Amount", Ease.EaseInOutSine(Mathf.Lerp(initialAmount, targetAmount, elapsedTime / 0.5f)));
                timeTravelMaterial.SetFloat("_FresnelPower", Ease.EaseOutCubic(Mathf.Lerp(initialFresnelPower, targetFresnelPower, elapsedTime / 0.5f)));
                timeTravelMaterial.SetFloat("_WobbleSpeed", Ease.EaseInBack(Mathf.Lerp(initialWobbleSpeed, targetWobbleSpeed, elapsedTime / 0.5f)));

                meshRenderer.SetPropertyBlock(_matPropBlock);
            }
            yield return null;
        }

        if (fadeIn)
        {
            StartCoroutine(AnimateTravel(false));
        }else
        {
            if (_matPropBlock != null)
            {
                timeTravelMaterial.SetFloat("_Distance", 0f);
                timeTravelMaterial.SetFloat("_Amount", 0f);
                timeTravelMaterial.SetFloat("_FresnelPower", 0f);
                timeTravelMaterial.SetFloat("_WobbleSpeed", 0f);

                meshRenderer.SetPropertyBlock(_matPropBlock);
            }
            SetTimeTravelMaterial(true);
            playing = false;

            if (particles)
            {
                particles.SetActive(false);
            }
        }

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
}
