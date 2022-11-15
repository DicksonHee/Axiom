using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Screenshot : MonoBehaviour
{
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha0)) ScreenCapture.CaptureScreenshot("" + GUID.Generate() + ".png", 2);
    }
}
