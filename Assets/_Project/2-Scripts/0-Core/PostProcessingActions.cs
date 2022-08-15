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
    
        private void Awake()
        {
            current = this;

            ppVolume = GetComponent<Volume>();
            ppVolume.profile.TryGet(out vignette);
        }

        public void SetVignetteIntensity(float intensity)
        {
            StopAllCoroutines();
            StartCoroutine(LerpVignetteIntensity_CO(intensity, 0.2f));
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
    }
}

