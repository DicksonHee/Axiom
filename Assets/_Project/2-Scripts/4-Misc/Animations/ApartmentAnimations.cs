using System;
using System.Collections;
using System.Collections.Generic;
using SCPE;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class ApartmentAnimations : MonoBehaviour
{
    public float openEyesDuration;
    public UnityEvent2 unityEvent;
    
    private Volume apartmentVolume;
    private BlackBars blackBars;
    private DoubleVision doubleVision;

    private void Awake()
    {
        apartmentVolume = GetComponent<Volume>();

        if (apartmentVolume.profile.TryGet(out blackBars))
        {
            blackBars.size.overrideState = true;
            blackBars.size.value = 1f;
        }
        
        if (apartmentVolume.profile.TryGet(out doubleVision))
        {
            doubleVision.intensity.overrideState = true;
            doubleVision.intensity.value = 0.2f;
        }

        OpenEyes();
        StartCoroutine(DoubleVision_CO(Random.Range(0.1f, 0.2f), Random.Range(0.5f, 1f), true));
    }

    public void EndPPAnimations()
    {
        StopAllCoroutines();
        StartCoroutine(DoubleVision_CO(0, 1f, false));
    }
    
    public void OpenEyes()
    {
        StartCoroutine(OpenEyes_CO());
    }

    public IEnumerator DoubleVision_CO(float targetIntensity, float duration, bool repeat)
    {
        float counter = 0f;
        float currentIntensity = doubleVision.intensity.value;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            doubleVision.intensity.value = Mathf.Lerp(currentIntensity, targetIntensity, counter / duration);
            yield return null;
        }

        if(repeat) StartCoroutine(DoubleVision_CO(Random.Range(0.1f, 0.2f), Random.Range(0.5f, 1f), true));
    }
    
    private IEnumerator OpenEyes_CO()
    {
        float counter = 0f;
        while (counter < openEyesDuration)
        {
            counter += Time.deltaTime;
            blackBars.size.value = Mathf.Lerp(1f, 0f, counter / openEyesDuration);
            yield return null;
        }

        blackBars.size.value = 0f;
        unityEvent.InvokeWithDelay(1f);
    }
}
