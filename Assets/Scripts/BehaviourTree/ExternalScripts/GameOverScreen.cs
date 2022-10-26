using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameOverScreen : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    public static GameOverScreen Instance;
    private float fadeDuration = 1f;

    private void Start()
    {
        canvasGroup.gameObject.SetActive(false);
        canvasGroup.alpha = 0;
    }

    public void FadeCanvasGroup(float fadeDuration)
    {
        this.fadeDuration = fadeDuration;
        canvasGroup.gameObject.SetActive(true);
        StartCoroutine(FadeCoroutine());
    }

    private IEnumerator FadeCoroutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            yield return null;
        }
    }
}
