using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Axiom.Core;

public class DialogResizer : MonoBehaviour
{
    public TMP_Text tmpText;
    public Image dialogBackground;

    public float padding;
    public float minWidth;
    public float maxWidth;
    public float heightStep;
    
    public void Resize()
    {
        Vector2 textDimension = new Vector2(tmpText.preferredWidth, tmpText.preferredHeight);

        if (textDimension.x < minWidth)
        {
            textDimension.x = minWidth;
            textDimension.y = heightStep;
        }
        else if (textDimension.x > maxWidth)
        {
            textDimension.y = Mathf.Ceil(textDimension.x / maxWidth) * heightStep;
            textDimension.x = maxWidth;
        }

        dialogBackground.rectTransform.sizeDelta = textDimension + new Vector2(padding, padding);
    }
}
