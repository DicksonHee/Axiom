using System;
using System.Collections;
using System.Collections.Generic;
using Axiom.Core;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class PPVolumeManager : MonoBehaviour
{
    public Volume currentVolume;
    public List<PPProfile_SO> areaProfiles;
    public AreaName defaultArea;
    
    private AreaName currentAreaName;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ChangeVolume(scene.name);
    }

    public void ChangeVolume(string sceneName)
    {
        foreach (PPProfile_SO profile in areaProfiles)
        {
            if (profile.scene == sceneName)
            {
                currentVolume.profile = profile.volumeProfile;
                currentAreaName = profile.areaName;
                break;
            }
        }
    }

    public void ChangeVolume(AreaName areaName)
    {
        if (areaName == currentAreaName) return;
        
        foreach (PPProfile_SO profile in areaProfiles)
        {
            if (profile.areaName == areaName)
            {
                StartCoroutine(TransitionVolume(profile.volumeProfile));
                currentAreaName = profile.areaName;
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