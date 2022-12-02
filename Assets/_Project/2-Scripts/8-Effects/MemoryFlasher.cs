using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryFlasher : MonoBehaviour
{
    public Flash[] flashes;
    public AnimationCurve flashCurve;

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
        StartFlashing();
    }

    public void StartFlashing() => StartCoroutine(FlashSequence());

    private IEnumerator FlashSequence()
    {
        yield return new WaitForSeconds(3);

        foreach (Flash flash in flashes)
        {
            Matrix4x4 m = cam.transform.localToWorldMatrix * flash.Camera.transform.worldToLocalMatrix * flash.Base.transform.localToWorldMatrix;
            flash.Base.transform.SetPositionAndRotation(m.GetPosition(), m.rotation);

            flash.Base.SetActive(true);

            for (float elapsed = 0; elapsed < flash.flashTime; elapsed += Time.deltaTime)
            {
                float t = elapsed / flash.flashTime;
                flash.SetLightIntensityMult(flashCurve.Evaluate(t));

                yield return null;
            }
            flash.Base.SetActive(false);

            yield return new WaitForSeconds(flash.nextDelay);
        }
    }

    [System.Serializable]
    public class Flash
    {
        public GameObject Base;
        public Light[] Lights;
        public Transform Camera;
        public float flashTime;
        public float nextDelay;

        private float[] lightIntensities;

        public void SetLightIntensityMult(float mult)
        {
            if (lightIntensities == null)
            {
                lightIntensities = new float[Lights.Length];
                for (int i = 0; i < Lights.Length; i++)
                    lightIntensities[i] = Lights[i].intensity;
            }

            for(int i = 0; i < Lights.Length; i++)
                Lights[i].intensity = lightIntensities[i] * mult;
        }
    }
}
