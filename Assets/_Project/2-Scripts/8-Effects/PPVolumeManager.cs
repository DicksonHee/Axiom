using System;
using System.Collections;
using System.Collections.Generic;
using Axiom.Core;
using Axiom.Player.Movement.StateMachine;
using UnityEngine;
using UnityEngine.Rendering;

public class PPVolumeManager : MonoBehaviour
{
    public Volume currentVolume;
    public List<EffectsVolume> areaVolumes;

    private GameObject currentSparkles;

    private void Awake()
    {
        ChangeVolume(AreaName.Dreamscape);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E)) ChangeVolume(AreaName.Memory);
        if(Input.GetKeyDown(KeyCode.F)) ChangeVolume(AreaName.Dreamscape);
    }

    public void ChangeVolume(AreaName areaName)
    {
        foreach (EffectsVolume vol in areaVolumes)
        {
            if (vol.areaName == areaName)
            {
                StartCoroutine(TransitionVolume(vol));
                vol.sparkles.SetActive(true); 
                if(currentSparkles != null) currentSparkles.SetActive(false);
                currentSparkles = vol.sparkles;
                break;
            }
        }
    }

    private IEnumerator TransitionVolume(EffectsVolume effectsVolume)
    {
        float counter = 0f;

        while (counter < 1f)
        {
            counter += Time.deltaTime;
            currentVolume.weight = 1f - counter;
            yield return null;
        }

        counter = 0f;
        currentVolume.profile = effectsVolume.volumeProfile;
        
        while (counter < 1f)
        {
            counter += Time.deltaTime;
            currentVolume.weight = counter;
            yield return null;
        }
        
        PostProcessingActions.current.VolumeUpdated();
    }
}

[Serializable]
public class EffectsVolume
{
    public AreaName areaName;
    public VolumeProfile volumeProfile;
    public GameObject sparkles;
}

public enum AreaName
{
    Dreamscape,
    Memory,
    Mindscape,
    Nightmare,
    RealWorld
}