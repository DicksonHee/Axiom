using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearDissolve : MonoBehaviour
{
    public Material dissolveMaterial;
    public Material hologramMaterial;

    public List<GameObject> deactivateObjects;
    public List<GameObject> activateObjects;

    private bool hasActivated;

    private void Awake()
    {
        hologramMaterial.SetFloat("_Transparency", 7.5f);
        dissolveMaterial.SetFloat("_Height", 5f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O)) PlayAnim();
        if (Input.GetKeyDown(KeyCode.P)) DissolveController.current.ResetDissolve("WhiteBear", 1f);
    }

    public void PlayAnim()
    {
        if (hasActivated) return;

        StartCoroutine(PlayAnim_CO());
    }

    private IEnumerator PlayAnim_CO()
    {
        float counter = 0;
        hasActivated = true;
        foreach (GameObject go in activateObjects) go.SetActive(true);
        foreach (GameObject go in deactivateObjects) go.SetActive(false);
        DissolveController.current.StartDissolve("WhiteBear", 1f, 7f);
        while (counter < 3f)
        {
            counter += Time.deltaTime;
            hologramMaterial.SetFloat("_Transparency", Mathf.Lerp(15f, 200f, counter/3f));
            yield return null;
        }
    }
}
