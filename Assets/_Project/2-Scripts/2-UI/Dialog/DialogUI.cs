using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogUI : MonoBehaviour
{
    public static DialogUI current;
    public TMP_Text tmpText;
    public DialogResizer dialogResizer;
    public CanvasGroup canvasGroup;

    private bool isShowing;
    
    private void Awake()
    {
        if (current == null && current != this)
        {
            current = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        HideText();
    }

    public void ShowText()
    {
        canvasGroup.alpha = 1f;
        isShowing = true;
    }

    public void UpdateText(string textToShow)
    {
        if(!isShowing) ShowText();
        CancelInvoke(nameof(HideText));
        
        tmpText.text = textToShow;
        dialogResizer.Resize();
        Invoke(nameof(HideText), 10f);
    }
    
    public void HideText()
    {
        canvasGroup.alpha = 0f;
        isShowing = false;
    }
}
