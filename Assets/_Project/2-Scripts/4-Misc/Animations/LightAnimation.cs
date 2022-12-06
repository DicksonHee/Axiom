using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightAnimation : MonoBehaviour
{
    public Light light;

    public void LerpLightIntensity(float startVal, float endVal, float duration)
    {
        StartCoroutine(LerpLightIntensity_CO(startVal, endVal, duration));
    }

    private IEnumerator LerpLightIntensity_CO(float startVal, float endVal, float duration)
    {
        float elapsedTime = 0f;

        while(elapsedTime < duration)
        {
            light.intensity = Mathf.Lerp(startVal, endVal, elapsedTime/duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        light.intensity = endVal;
    }
}