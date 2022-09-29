using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using PathCreation;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;

[ExecuteInEditMode]
public class BezierFollow : MonoBehaviour
{
    public PathCreator bezierCurve;
    public List<VisualEffect> listOfVFX;
    
    public void CreateTexture2D()
    {
        Texture2D texture2D = new Texture2D(64, 64, TextureFormat.RGBAFloat, false);
        for (int ii = 0; ii < 64; ii++)
        {
            for (int jj = 0; jj < 64; jj++)
            {
                Vector3 point = bezierCurve.path.GetPointAtTime((64f * ii + jj) / 4096f);
                texture2D.SetPixel(ii, jj, new Color(point.x, point.y, point.z));
            }
        }
        texture2D.Apply();
        
        foreach (VisualEffect vfx in listOfVFX)
        {
            vfx.SetTexture("Tex", texture2D);
        }
    }
}

[CustomEditor(typeof(BezierFollow))]
public class BezierFollowEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        BezierFollow myTarget = (BezierFollow)target;

        if (GUILayout.Button("Create Texture2D"))
        {
            myTarget.CreateTexture2D();
        }
    }
}
