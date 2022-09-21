using System.Collections;
using System.Collections.Generic;
using Axiom.Core;
using UnityEngine;

public class InteractableObjects : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log(name + "Interacted");
    }

    public void Hover()
    {
        Debug.Log(name);
    }
}
