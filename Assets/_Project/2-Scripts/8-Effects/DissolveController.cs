using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveController : MonoBehaviour
{
    public List<MaterialEntry> materialEntries;

    public static DissolveController current;
    private Dictionary<string, MaterialEntry> materialDict;

    private void Start()
    {
        if (current != null && current != this) Destroy(this);
        else current = this;

        materialDict = new Dictionary<string, MaterialEntry>();
        foreach(var entry in materialEntries)
        {
            entry.material.SetFloat(entry.floatRef, entry.defaultValue);
            materialDict.Add(entry.name, entry);
        }    
    }

    public void StartDissolve(string materialName, float duration, float startValue = -1)
    {
        StartCoroutine(StartDissolve_CO(materialDict[materialName], duration, startValue));
    }

    public void ResetDissolve(string materialName, float duration)
    {
        StartCoroutine(ResetDissolve_CO(materialDict[materialName], duration));
    }
    private IEnumerator StartDissolve_CO(MaterialEntry materialEntry, float duration, float startValue)
    {
        float counter = 0f;
        float startVal = startValue == -1 ? materialEntry.defaultValue : startValue;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            materialEntry.material.SetFloat(materialEntry.floatRef, Mathf.Lerp(startVal, -5f, counter / duration));
            yield return null;
        }
    }

    private IEnumerator ResetDissolve_CO(MaterialEntry materialEntry, float duration)
    {
        float counter = 0f;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            materialEntry.material.SetFloat(materialEntry.floatRef, Mathf.Lerp(-5f, materialEntry.defaultValue, counter / duration));
            yield return null;
        }
    }
}

[Serializable]
public class MaterialEntry
{
    public string name;
    public string floatRef;
    public Material material;
    public float defaultValue;    
}