using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Memory2Anim : MonoBehaviour
{
    public float animTime, delay;
    public FadeMaterial[] fadeMaterials;
    public GameObject[] inObjects, outObjects;

    private const float opaque = 3.5f;
    private const float transparent = 60f;

    private bool hasActivated = false;

    private void Awake()
    {
        SetMaterialTransparency(0);
    }

    public void PlayAnim() 
    {
        if (hasActivated) return;

        StartCoroutine(Anim());
        hasActivated = true;
    }

    private void SetMaterialTransparency(float t)
    {
        foreach (var mat in fadeMaterials)
            mat.SetTransparency(t);
    }

    private IEnumerator Anim()
    {
        yield return new WaitForSeconds(delay);

        foreach (var obj in inObjects)
            obj.SetActive(true);

        for (float elapsed = 0f; elapsed < animTime; elapsed += Time.deltaTime)
        {
            float t = elapsed / animTime;
            SetMaterialTransparency(t);
            yield return null;
        }

        SetMaterialTransparency(1f);

        foreach (var obj in outObjects)
            obj.SetActive(false);
    }

    private void OnDestroy()
    {
        SetMaterialTransparency(0);
    }

    [System.Serializable]
    public class FadeMaterial
    {
        public Material material;
        public Vector2 transparencyRange;

        public void SetTransparency(float t) => 
            material.SetFloat("_Transparency", Mathf.Lerp(transparencyRange.x, transparencyRange.y, t));
    }
}
