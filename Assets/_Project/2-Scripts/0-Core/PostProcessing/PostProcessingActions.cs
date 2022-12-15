using SCPE;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace Axiom.Core
{
    public class PostProcessingActions : MonoBehaviour
    {
        public static PostProcessingActions current;
        public PPVolumeManager volumeManager;

        private Volume ppVolume;
        private Vignette vignette;
        private RadialBlur radialBlur;
        private Overlay overlay;

        private Coroutine vignetteCoroutine;
        private Coroutine respawnCoroutine;
        
        private void Awake()
        {
            current = this;

            ppVolume = GetComponent<Volume>();
            VolumeUpdated();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            //Testing
            //if(scene.name == "Rhys_RealWorld") ChangeVolume(AreaName.RealWorld);
            //else if(scene.name == "Rhys_EscapeSequence" ||
            //   scene.name == "Rhys_Mindscape" ||
            //   scene.name == "Tom_Mindscape_1" ||
            //   scene.name == "Dreamscape_Whole") ChangeVolume(AreaName.Dreamscape);
            //else if(scene.name == "MainMenu") ChangeVolume(AreaName.MainMenu);
        }
        
        #region ChangeVolume
        public void ChangeVolume(AreaName areaName) => volumeManager.ChangeVolume(areaName);

        public void VolumeUpdated()
        {
            if (!ppVolume.profile.TryGet(out vignette)) vignette = ppVolume.profile.Add<Vignette>();
            
            if (!ppVolume.profile.TryGet(out radialBlur)) radialBlur = ppVolume.profile.Add<RadialBlur>();
            radialBlur.amount.overrideState = true;
            radialBlur.amount.value = 0f;

            if (!ppVolume.profile.TryGet(out overlay)) overlay = ppVolume.profile.Add<Overlay>();
            overlay.intensity.overrideState = true;
            overlay.intensity.value = 0f;
        }
        #endregion

        #region Vignette
        public void SetVignetteIntensity(float intensity)
        {
            if (respawnCoroutine != null) return;
            if (vignetteCoroutine != null) StopCoroutine(vignetteCoroutine);
            vignetteCoroutine = StartCoroutine(LerpVignetteIntensity_CO(intensity, 0.2f));
        }
        
        private IEnumerator LerpVignetteIntensity_CO(float targetIntensity, float duration)
        {
            float initialIntensity = vignette.intensity.value;
            float counter = 0f;
            while (counter < duration)
            {
                counter += Time.deltaTime;
                vignette.intensity.value = Mathf.Lerp(initialIntensity, targetIntensity, counter / duration);
                yield return null;
            }

            vignette.intensity.value = targetIntensity;
        }
        #endregion

        #region Respawn
        public void RespawnAnimation(float duration)
        {
            if(respawnCoroutine != null) StopCoroutine(respawnCoroutine);
            respawnCoroutine = StartCoroutine(RespawnAnimation_CO(duration));
        }

        private IEnumerator RespawnAnimation_CO(float duration)
        {
            float counter = 0f;
            
            while (counter < duration/2)
            {
                counter += Time.deltaTime;
                radialBlur.amount.value = Mathf.Lerp(0, 1, counter / (duration/2));
                overlay.intensity.value = Mathf.Lerp(0, 1, counter / (duration/2));
                yield return null;
            }
            
            while (counter < duration)
            {
                counter += Time.deltaTime;
                radialBlur.amount.value = Mathf.Lerp(1, 0, counter / duration);
                overlay.intensity.value = Mathf.Lerp(1, 0, counter / duration);
                yield return null;
            }

            radialBlur.amount.value = 0f;
            overlay.intensity.value = 0f;
        }
        #endregion
    }
}