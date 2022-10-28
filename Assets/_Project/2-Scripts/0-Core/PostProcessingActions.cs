using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Axiom.Core
{
    public class PostProcessingActions : MonoBehaviour
    {
        public static PostProcessingActions current;

        private Volume ppVolume;
        private Vignette vignette;
        private SCPE.RadialBlur radialBlur;
        private SCPE.Overlay overlay;

        private Coroutine vignetteCoroutine;
        private Coroutine respawnCoroutine;
        
        private void Awake()
        {
            current = this;

            ppVolume = GetComponent<Volume>();
            ppVolume.profile.TryGet(out vignette);
            ppVolume.profile.TryGet(out radialBlur);
            ppVolume.profile.TryGet(out overlay);

            radialBlur.amount.overrideState = true;
            overlay.intensity.overrideState = true;

            radialBlur.amount.value = 0f;
            overlay.intensity.value = 0f;
        }

        public void SetVignetteIntensity(float intensity)
        {
            if (respawnCoroutine != null) return;
            if(vignetteCoroutine != null) StopCoroutine(vignetteCoroutine);
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
    }
}

