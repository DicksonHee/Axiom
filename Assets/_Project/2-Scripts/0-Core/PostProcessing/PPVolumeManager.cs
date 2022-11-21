using System;
using System.Collections;
using System.Collections.Generic;
using Axiom.Core;
using UnityEngine;
using UnityEngine.Rendering;

public class PPVolumeManager : MonoBehaviour
{
    public Volume currentVolume;
    public List<PPProfile_SO> areaProfiles;
    public AreaName defaultArea;
    
    private GameObject currentSparkles;
    private AreaName currentAreaName;

    private void Awake()
    {
        ChangeVolume(defaultArea);
    }

    public void ChangeVolume(AreaName areaName)
    {
        if (areaName == currentAreaName) return;
        
        foreach (PPProfile_SO profile in areaProfiles)
        {
            if (profile.areaName == areaName)
            {
                StartCoroutine(TransitionVolume(profile.volumeProfile));
                currentAreaName = areaName;
                break;
            }
        }
    }

    private IEnumerator TransitionVolume(VolumeProfile newProfile)
    {
        float counter = 0f;

        while (counter < 1f)
        {
            counter += Time.deltaTime;
            currentVolume.weight = 1f - counter;
            yield return null;
        }

        counter = 0f;
        currentVolume.profile = newProfile;
        
        while (counter < 1f)
        {
            counter += Time.deltaTime;
            currentVolume.weight = counter;
            yield return null;
        }
        
        PostProcessingActions.current.VolumeUpdated();
    }
}