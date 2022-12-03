using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MindCoreMemoryFlash : MonoBehaviour
{
    public MeshRenderer funnel;
    public MeshRenderer[] cores;
    public Light fadeLight;
    public LineRenderer[] explosionLines;

    private Color baseColour, emission1Colour, emission2Colour, coreColour;
    private float fadeLightMaxIntensity;

    private void Awake()
    {
        baseColour = funnel.material.GetColor("_BaseColour");
        emission1Colour = funnel.material.GetColor("_EmissionColour");
        emission2Colour = funnel.material.GetColor("_Emission2_Colour");
        coreColour = cores[0].material.GetColor("_BaseColor");

        fadeLightMaxIntensity = fadeLight.intensity;
    }

    private void Start()
    {
        funnel.material.SetFloat("_ScrollOffset", Random.value);
    }

    private void Update()
    {
        float brightness = fadeLight.intensity.Remap(0, fadeLightMaxIntensity);

        funnel.material.SetColor("_BaseColour", baseColour * brightness);
        funnel.material.SetColor("_EmissionColour", emission1Colour * brightness);
        funnel.material.SetColor("_Emission2_Colour", emission2Colour * brightness);

        foreach(MeshRenderer core in cores)
            core.material.SetColor("_BaseColor", coreColour * brightness);

        if (explosionLines == null || explosionLines.Length <= 0)
            return;

        foreach (LineRenderer line in explosionLines)
        {
            line.startColor = Color.white * brightness;
            line.endColor = Color.white * brightness;
        }
    }
}
