using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Axiom.Player.Interaction;
using TMPro;

public class InteractUI : MonoBehaviour
{
    public CanvasGroup interactionTextCanvasGroup;
    public CanvasGroup interactionDetailsCanvasGroup;

    public TMP_Text objectName;
    public TMP_Text objectDescription;

    private void Awake()
    {
        interactionTextCanvasGroup.alpha = 0f;
        interactionDetailsCanvasGroup.alpha = 0f;
    }

    private void OnEnable()
    {
        PlayerInteract.OnInteractableHovered += ShowInteractText;
        PlayerInteract.OnInteractableExitHover += HideIntereactText;
        PlayerInteract.OnStartInteract += ShowDetailsCanvas;
        PlayerInteract.OnStopInteract += HideDetailsCanvas;
    }

    private void OnDisable()
    {
        PlayerInteract.OnInteractableHovered -= ShowInteractText;
        PlayerInteract.OnInteractableExitHover -= HideIntereactText;
        PlayerInteract.OnStartInteract -= ShowDetailsCanvas;
        PlayerInteract.OnStopInteract -= HideDetailsCanvas;
    }

    public void ShowInteractText()
    {
        interactionTextCanvasGroup.alpha = 1f;
    }

    public void HideIntereactText()
    {
        interactionTextCanvasGroup.alpha = 0f;
    }

    private void ShowDetailsCanvas(InteractableObject_SO objectSO)
    {
        objectName.text = objectSO.objectName;
        objectDescription.text = objectSO.objectDescription;
        interactionDetailsCanvasGroup.alpha = 1f;
    }

    private void HideDetailsCanvas()
    {
        Debug.Log("ASd");
        interactionDetailsCanvasGroup.alpha = 0f;
    }
}
