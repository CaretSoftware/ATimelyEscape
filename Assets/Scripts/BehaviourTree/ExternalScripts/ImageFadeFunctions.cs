using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageFadeFunctions : MonoBehaviour
{
    private Image image;
    private CanvasGroup canvasGroup;

    private const float fadeDividerVar = 2;
    private const float messageFadeDividerVar = 0.5f;
    void Start()
    {
        image = GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    //fade blackScreenCanvasGroup from transparent to black.
    private IEnumerator FadeToBlack()
    {
        image.color = Color.black;
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime / fadeDividerVar;
            yield return null;
        }
    }

    private IEnumerator FadeToWhite()
    {
        float time = 0;
        while (image.color != Color.white)
        {
            time += Time.deltaTime / fadeDividerVar;
            image.color = Color.Lerp(image.color, Color.white, time);
            yield return null;
        }
    }
    
    private IEnumerator FadeIn()
    {
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime / messageFadeDividerVar;
            yield return null;
        }
    }

    private IEnumerator FadeOut()
    {
        //player.position = checkpoint.position;
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime / messageFadeDividerVar;
            yield return null;
        }
    }

    public void RunFadeToBlack() { StartCoroutine(FadeToBlack()); }
    public void RunFadeToWhite() { StartCoroutine(FadeToWhite()); }
    public void RunFadeIn() { StartCoroutine(FadeIn()); }
    public void RunFadeOut() { StartCoroutine(FadeOut()); }
    
}
