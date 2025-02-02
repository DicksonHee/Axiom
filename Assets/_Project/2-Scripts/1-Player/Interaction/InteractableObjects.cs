using System;
using System.Collections;
using System.Collections.Generic;
using Axiom.Core;
using Axiom.Player.Interaction;
using UnityEngine;

public class InteractableObjects : MonoBehaviour, IInteractable
{
    public InteractableObject_SO objectSO;
    public string textToShow;

    public void StartInteraction()
    {
        PlayerInteract.InvokeStartInteract(objectSO);
    }

    public void StopInteraction()
    {
        PlayerInteract.InvokeStopInteract();
    }

    public void Hover()
    {
        PlayerInteract.InvokeStartHover(textToShow);
    }
    
    public void EndHover()
    {
        PlayerInteract.InvokeEndHover();
    }
}
