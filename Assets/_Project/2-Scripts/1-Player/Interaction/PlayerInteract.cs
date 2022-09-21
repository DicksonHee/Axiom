using System;
using System.Collections;
using System.Collections.Generic;
using Axiom.Core;
using UnityEngine;
using static UnityEngine.Screen;

namespace Axiom.Player.Interaction
{
    public class PlayerInteract : MonoBehaviour
    {
        public Camera mainCamera;
        public LayerMask interactableLayer;
        
        private readonly Vector3 screenMiddle = new(width / 2f, height / 2f, 0);
        
        private void Update()
        {
            CheckForInteractableObject();
        }

        private void CheckForInteractableObject()
        {
            if (!Physics.Raycast(mainCamera.ScreenPointToRay(screenMiddle), out RaycastHit hitInfo, 10f, interactableLayer)) return;
            if (!hitInfo.collider.TryGetComponent(out IInteractable interactable)) return;

            interactable.Hover();
            if(Input.GetMouseButtonDown(0)) interactable.Interact();
        }
    }
}