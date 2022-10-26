using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CounterOverTime : MonoBehaviour
{
    [SerializeField] private bool active;
    [SerializeField] private TextMeshProUGUI textMeshPro;
    private float nextActionTime = 1.0f;
    private float timer = 0f;
    private int points;

    void Update()
    {
        if (active)
            func();
    }

    private void func()
    {
        if (timer > nextActionTime)
        {
            ++points;
            textMeshPro.text = points.ToString();
            timer = 0;
        }
        timer += Time.deltaTime;
    }
}
