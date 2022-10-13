using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float startingHealth;
    [SerializeField] private float lowHealth;
    [SerializeField] private float healthRestorationRate;
    public float CurrentHealth { get { return CurrentHealth; } private set { CurrentHealth = Mathf.Clamp(value, 0, startingHealth); } }
    private void Start()
    {
        CurrentHealth = startingHealth;
    }
    private void Update()
    {
        RestoreHealth();
    }
    private void RestoreHealth()
    {
        CurrentHealth += Time.deltaTime * healthRestorationRate;
    }

}
