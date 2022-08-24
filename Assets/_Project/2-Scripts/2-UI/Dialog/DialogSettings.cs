using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogSettings : MonoBehaviour
{
    public TMP_Text tmpText;
    public Image backgroundImage;
    
    [Range(0,1)] public float alphaValue;
    public DialogSize dialogSize;

    private void Awake()
    {
        switch (dialogSize)
        {
            case DialogSize.Small:
                tmpText.fontSize = 20f;
                break;
            case DialogSize.Medium:
                tmpText.fontSize = 25f;
                break;
            case DialogSize.Large:
                tmpText.fontSize = 30f;
                break;
        }

        Color bgCol = backgroundImage.color;
        bgCol.a = alphaValue;
        backgroundImage.color = bgCol;
    }
}

public enum DialogSize
{
    Small,
    Medium,
    Large
}
