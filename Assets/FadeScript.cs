using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeScript : MonoBehaviour
{
    [SerializeField] private float delay = 0.5f;
    [SerializeField] private float alpha;

    private bool faded = false;

    private CanvasGroup canvasGroup;

    private IEnumerator currentCoroutine;

    private void Start()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
    }

    private void Update()
    {
        alpha = canvasGroup.alpha;
    }

    public void FadeIn()
    {
        if (!faded)
            return;

        faded = false;

        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        currentCoroutine = FadingIn();

        StartCoroutine(currentCoroutine);
    }

    public void FadeOut()
    {
        if (faded)
            return;

        faded = true;

        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        currentCoroutine = FadingOut();

        StartCoroutine(currentCoroutine);
    }

    private IEnumerator FadingIn()
    {
        float time = 0;

        float startalpha = canvasGroup.alpha;

        while (canvasGroup.alpha < 1)
        {
            time += Time.deltaTime;

            canvasGroup.alpha = Mathf.Lerp(startalpha, 1, time / delay);
            
            yield return null;
        }

        canvasGroup.alpha = 1;
        currentCoroutine = null;
    }

    private IEnumerator FadingOut()
    {
        float time = 0;

        float startalpha = canvasGroup.alpha;

        while (canvasGroup.alpha > 0)
        {
            time += Time.deltaTime;

            canvasGroup.alpha = Mathf.Lerp(startalpha, 0, time / delay);

            yield return null;
        }

        canvasGroup.alpha = 0;
        currentCoroutine = null;
    }

    public bool IsFaded()
    {
        return faded;
    }
}
