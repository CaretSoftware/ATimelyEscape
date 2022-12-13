using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollBand : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]private float scrollSpeedX;
    [SerializeField]private float scrollSpeedY;
    [SerializeField]private Material material;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        material.mainTextureOffset = new Vector2(Time.realtimeSinceStartup * scrollSpeedX, Time.realtimeSinceStartup * scrollSpeedY);
    }
}
