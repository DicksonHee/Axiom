using Axiom.Core;
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

    public DialogSize dialogSize;

    private void Awake()
    {
        ApplySettings();
    }

    private void OnEnable()
    {
        SettingsData.OnSettingUpdated += ApplySettings;
    }

    private void OnDisable()
    {
        SettingsData.OnSettingUpdated -= ApplySettings;
    }

    private void ApplySettings()
    {
        switch(SettingsData.textSize)
        {
            case 0:
                tmpText.fontSize = 20f;
                break;
            case 1:
                tmpText.fontSize = 25f;
                break;
            case 2:
                tmpText.fontSize = 30f;
                break;
        }

        Color bgCol = backgroundImage.color;
        bgCol.a = SettingsData.textBackgroundOpacity;
        backgroundImage.color = bgCol;
    }
}

public enum DialogSize
{
    Small,
    Medium,
    Large
}
