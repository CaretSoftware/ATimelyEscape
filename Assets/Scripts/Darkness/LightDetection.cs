using System.Collections;
using UnityEngine;

public class LightDetection : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The camera who scans for light.")]
    public Camera lightScan;
    [Tooltip("Show the light value in the log.")]
    public bool lightValueLog = false;
    [Tooltip("Time between light value updates (default = 0.1f).")]
    public float updateTime = 0.1f;

    public static float lightValue;

    private const int textureSize = 1;

    private Texture2D m_texLight;
    private RenderTexture m_texTemp;
    private Rect m_rectLight;
    private Color m_LightPixel;

    private void Start()
    {
        StartLightDetection();
    }

    /// <summary>
    /// Prepare all needed variables and start the light detection coroutine.
    /// </summary>
    private void StartLightDetection()
    {
        m_texLight = new Texture2D(textureSize, textureSize, TextureFormat.RGB24, false);
        m_texTemp = new RenderTexture(textureSize, textureSize, 24);
        m_rectLight = new Rect(0f, 0f, textureSize, textureSize);

        StartCoroutine(LightDetectionUpdate(updateTime));
    }

    /// <summary>
    /// Updates the light value each x seconds.
    /// </summary>
    /// <param name="updateTime">Time in seconds between updates.</param>
    /// <returns></returns>
    private IEnumerator LightDetectionUpdate(float updateTime)
    {
        while (true)
        {
            //Set the target texture of the cam.
            lightScan.targetTexture = m_texTemp;
            //Render into the set target texture.
            lightScan.Render();

            //Set the target texture as the active rendered texture.
            RenderTexture.active = m_texTemp;
            //Read the active rendered texture.
            m_texLight.ReadPixels(m_rectLight, 0, 0);

            //Reset the active rendered texture.
            RenderTexture.active = null;
            //Reset the target texture of the cam.
            lightScan.targetTexture = null;

            //Read the pixel in middle of the texture.
            m_LightPixel = m_texLight.GetPixel(textureSize / 2, textureSize / 2);

            //Calculate light value, based on color intensity (from 0f to 1f).
            lightValue = (m_LightPixel.r + m_LightPixel.g + m_LightPixel.b) / 3f;

            if (lightValueLog)
            {
                Debug.Log("Light Value: " + lightValue);
            }

            yield return new WaitForSeconds(updateTime);
        }
    }
}