using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxChanger : MonoBehaviour
{
    public Material newSkybox;

    public void ChangeSkybox()
    {
        RenderSettings.skybox = newSkybox;
    }
}
