using Axiom.NonEuclidean;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeamlessColourChange : MonoBehaviour
{
    public Material material;
    public Color currentColor;

    public List<ColorPair> colorPairs;

    private void Awake()
    {
        currentColor = material.GetColor("Color");
    }

    private void OnEnable()
    {
        GravityRotator.OnGravityChanged += CalculateDir;
    }

    public void CalculateDir(Vector3 currentGrav)
    {
        foreach (ColorPair pair in colorPairs)
        {
            if(Vector3.Dot(pair.gravityDirection, currentGrav) > 0.95f)
            {
                ChangeColor(pair.color);
            }
        }
    }

    public void ChangeColor(Color newColor)
    {
        if (currentColor == newColor) return;

        StartCoroutine(ChangeColor_CO(newColor));
    }

    private IEnumerator ChangeColor_CO(Color newColor)
    {
        float counter = 0f;

        while(counter < 1f)
        {
            counter += Time.deltaTime;
            material.SetColor("Color", Color.Lerp(currentColor, newColor, counter));
            yield return null;
        }

        currentColor = newColor;
    }
}

[Serializable]
public class ColorPair
{
    public Color color;
    public Vector3 gravityDirection;
}
