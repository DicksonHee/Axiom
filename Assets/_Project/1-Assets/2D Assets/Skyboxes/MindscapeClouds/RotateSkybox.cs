using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSkybox : MonoBehaviour
{
    void Update()
    {
        if (RenderSettings.skybox == null) return;
        if (!RenderSettings.skybox.HasFloat("_Rotation")) return;

        RenderSettings.skybox.SetFloat("_Rotation", Time.time * 2f);
        //To set the speed, just multiply Time.time with whatever amount you want.
    }
}
