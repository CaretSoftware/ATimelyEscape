using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDeathVisualization : MonoBehaviour
{
    private static PlayerDeathVisualization _instance;
    public static PlayerDeathVisualization Instance
    {
        get { return _instance; }
    }
    
    [SerializeField] private float fadeOutDividerVar = 2;
    [SerializeField] private float fadeBackDividerVar = 1;

    //Place Gameobject with this script as child of main camera.
    private Animator hyperDriveAnimator;

    //Place Gameobject as child of Canvas.
    private CanvasGroup blackScreenCanvasGroup;

    private ParticleSystem ps;
    private Transform checkpoint;
    private Transform player;

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }

    private void Start()
    {
        player = GameObject.Find("Player").transform;
        hyperDriveAnimator = GetComponent<Animator>();
        blackScreenCanvasGroup = GameObject.Find("Canvas/BlackScreen").GetComponent<CanvasGroup>();
        ps = GetComponent<ParticleSystem>();
    }

    //Call on when player dies.
    public void PlayDeathVisualization(Transform checkpoint)
    {
        this.checkpoint = checkpoint;
        StartCoroutine(FadeToBlack());
        hyperDriveAnimator.SetTrigger("DeathAnimTrigger");
    }

    //enables/disables relevant components.
    private void StopVisualization()
    {
        ps.Stop();
        blackScreenCanvasGroup.alpha = 0;
    }

    //fade blackScreenCanvasGroup from transparent to black.
    private IEnumerator FadeToBlack()
    {
        while (blackScreenCanvasGroup.alpha < 1)
        {
            blackScreenCanvasGroup.alpha += Time.deltaTime / fadeOutDividerVar;
            yield return null;
        }
    }

    //called on as animation event when player should spawn and animation end.
    public void RespawnPlayer()
    {
        player.position = checkpoint.position;
        StartCoroutine(FadeBack());
        StopVisualization();
        hyperDriveAnimator.ResetTrigger("DeathAnimTrigger");
    }
    
    private IEnumerator FadeBack()
    {
        while (blackScreenCanvasGroup.alpha > 0)
        {
            blackScreenCanvasGroup.alpha -= Time.deltaTime / fadeBackDividerVar;
            yield return null;
        }

    }
}
