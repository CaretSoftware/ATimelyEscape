using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeTravelCollisionWarning : MonoBehaviour
{

    [SerializeField] GameObject warningObject;
    [SerializeField] TextMeshProUGUI context;
    [SerializeField] float activeTime = 1f;

    private bool isRunning;
    public void ShowWarning()
    {
        if (!isRunning) StartCoroutine(ShowWarningCoroutine());
    }

    private IEnumerator ShowWarningCoroutine()
    {
        isRunning = true;

        warningObject.SetActive(true);
        context.text = "You're trying to time travel into another object!";
        context.gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(activeTime);

        warningObject.SetActive(false);
        context.gameObject.SetActive(false);

        isRunning = false;
    }

}
