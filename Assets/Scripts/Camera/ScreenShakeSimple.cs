using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShakeSimple : MonoBehaviour
{
    public float shakeTime = 0.3f;
    public float shakeFreq = 10;
    public float shakeAmplitude = 0.3f;
    public static ScreenShakeSimple Instance;
    Vector3 originalPos;

    private void Awake()
    {
        Instance = this;
        originalPos = transform.position;
    }

    [ContextMenu("Shake")]
    public void Shake(float amplitude01)
    {
        StopAllCoroutines();
        StartCoroutine(ShakeC(amplitude01));
    }

    IEnumerator ShakeC(float amplitude01)
    {
        float t = 0;
        while(t<1)
        {
            t += Time.deltaTime / shakeTime;

            float amplitude = Curves.QuadEaseOut(shakeAmplitude * amplitude01, 0, Mathf.Clamp01(t));
            float shakeX = Curves.QuadEaseInOut(-1, 1, Mathf.PingPong(t * shakeFreq, 1));
            float shakeY = Curves.QuadEaseInOut(-1, 1, Mathf.PingPong(t * (shakeFreq+1), 1));
            transform.position = originalPos + new Vector3(shakeX, shakeY)* amplitude;

            yield return null;
        }
    }
}
