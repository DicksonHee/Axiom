using System.Collections;
using System.Collections.Generic;
using Axiom.Core;
using FMODUnity;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSFX : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
    public EventReference hoverRef;
    public EventReference clickRef;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.current.PlaySFX2D(hoverRef);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        AudioManager.current.PlaySFX2D(clickRef);
    }
}
