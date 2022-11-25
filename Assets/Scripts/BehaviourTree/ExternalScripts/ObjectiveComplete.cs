using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectiveComplete : MonoBehaviour
{
    [SerializeField] private float waitTimer;
    [SerializeField] private string TextComplete;

    [HideInInspector] private TextMeshProUGUI Text;
    [HideInInspector] public bool Complete;

    private void Start()
    {
        Text = GetComponent<TextMeshProUGUI>();
    }

    public void SetObjectiveComplete()
    {
        Complete = true;
        Text.text = TextComplete.ToString();
        StartCoroutine(DelayCoroutine());
    }

    private IEnumerator DelayCoroutine()
    {
        yield return new WaitForSeconds(waitTimer);
        DestroyImmediate(Text);
    }
}
