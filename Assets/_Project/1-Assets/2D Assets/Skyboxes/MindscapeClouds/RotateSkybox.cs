using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSkybox : MonoBehaviour
{
    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * 2f);
        //To set the speed, just multiply Time.time with whatever amount you want.
    }
}
