using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Axiom.Core;
using UnityEngine.UI;
using TMPro;

public class SettingsUI : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    [Header("Mouse Sensitivity")]
    public Slider mouseSensSlider;
    public TMP_Text mouseSensText;

    [Header("BGM Volume")]
    public Slider bgmVolSlider;
    public TMP_Text bgmVolText;

    [Header("SFX Volume")]
    public Slider sfxVolSlider;
    public TMP_Text sfxVolText;

    [Header("Dialog Volume")]
    public Slider dialogVolumeSlider;
    public TMP_Text dialogVolumeText;

    [Header("Text Size")]
    public Slider textSizeSlider;
    public TMP_Text textSizeText;

    [Header("Text Background")]
    public Slider textBackgroundOpacitySlider;
    public TMP_Text textBackgroundOpacityText;

    private bool isMenuActive;

    private void Awake()
    {
        mouseSensSlider.onValueChanged.AddListener(SetMouseSensitivity);
        bgmVolSlider.onValueChanged.AddListener(SetBGMVol);
        sfxVolSlider.onValueChanged.AddListener(SetSFXVol);
        textSizeSlider.onValueChanged.AddListener(SetTextSize);
        textBackgroundOpacitySlider.onValueChanged.AddListener(SetTextBackgroundOpacity);

        SetMouseSensitivity(mouseSensSlider.value);
        SetBGMVol(bgmVolSlider.value);
        SetSFXVol(sfxVolSlider.value);
        SetTextSize(textSizeSlider.value);
        SetTextBackgroundOpacity(textBackgroundOpacitySlider.value);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isMenuActive) SetCanvasInactive();
            else SetCanvasActive();
        }
    }

    private void SetCanvasActive()
    {
        PlayerMovementDetails.DisableAllMovementInput();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        isMenuActive = true;
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private void SetCanvasInactive()
    {
        PlayerMovementDetails.EnableAllMovementInput();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isMenuActive = false;
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void SetMouseSensitivity(float value)
    {
        SettingsData.mouseSensitivity = (int)value;
        mouseSensText.text = mouseSensSlider.value.ToString();

        SettingsData.SettingsUpdated();
    }

    public void SetBGMVol(float value)
    {
        SettingsData.bgmVolume = (int)value;
        bgmVolText.text = bgmVolSlider.value.ToString();

        SettingsData.SettingsUpdated();
    }

    public void SetSFXVol(float value)
    {
        SettingsData.sfxVolume = (int)value;
        sfxVolText.text = sfxVolSlider.value.ToString();

        SettingsData.SettingsUpdated();
    }

    public void SetDialogVol(float value)
    {
        SettingsData.dialogVolume = (int)value;
        dialogVolumeText.text = dialogVolumeSlider.value.ToString();

        SettingsData.SettingsUpdated();
    }

    public void SetTextSize(float value)
    {
        SettingsData.textSize = (int)value;
        switch (SettingsData.textSize)
        {
            case 0:
                textSizeText.text = "Small";
                break;
            case 1:
                textSizeText.text = "Medium";
                break;
            case 2:
                textSizeText.text = "Large";
                break;
        }

        SettingsData.SettingsUpdated();
    }

    public void SetTextBackgroundOpacity(float value)
    {
        SettingsData.textBackgroundOpacity = (int)value;
        textBackgroundOpacityText.text = textBackgroundOpacitySlider.value.ToString();

        SettingsData.SettingsUpdated();
    }
}
