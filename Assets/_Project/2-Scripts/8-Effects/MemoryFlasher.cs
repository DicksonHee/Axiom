using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MemoryFlasher : MonoBehaviour
{
    public Flash[] flashes;
    public AnimationCurve flashCurve;
    public string sceneToLoad;

    private Camera cam;
    private Transform player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            cam = other.GetComponentInChildren<Camera>();
            player = other.transform;

            StartFlashing();
        }
    }

    public void StartFlashing() => StartCoroutine(FlashSequence());

    private IEnumerator FlashSequence()
    {
        if (!SceneManager.GetSceneByName("Load_Scene").isLoaded) SceneManager.LoadSceneAsync("Load_Scene", LoadSceneMode.Additive);
        while (!SceneManager.GetSceneByName("Load_Scene").isLoaded)
        {
            yield return null;
        }

        LoadScreen.current.SetBlack();
        LoadScreen.current.SetOpaque();
        print("black");

        yield return new WaitForSeconds(2f);

        player.position = transform.position;
        player.GetComponentInChildren<ParticleSystem>().gameObject.SetActive(false);

        Axiom.Core.PlayerMovementDetails.DisableAllMovementInput();
        Axiom.Core.PlayerMovementDetails.cameraLookEnabled = false;
        cam.fieldOfView = 60f;

        print("clear");
        LoadScreen.current.SetBlack();
        LoadScreen.current.SetClear();

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

        LoadScreen.current.SetBlack();
        LoadScreen.current.SetOpaque();
        yield return new WaitForSeconds(2f);

        SceneLoad_Manager.LoadSpecificScene(sceneToLoad);
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
