using Axiom.Core;
using Axiom.Player.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableSceneLoad : MonoBehaviour,  IInteractable
{
    public string sceneToLoad;
    public string textToShow;

    public void StartInteraction()
    {
        SceneLoad_Manager.LoadSpecificScene(sceneToLoad);
    }

    public void StopInteraction()
    {
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
