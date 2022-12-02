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
        public static event Action OnInteractableHovered;
        public static event Action OnInteractableExitHover;
        public static event Action<InteractableObject_SO> OnStartInteract;
        public static event Action OnStopInteract;

        public Camera mainCamera;
        public LayerMask interactableLayer;

        private IInteractable interactable;
        private bool isHovering;
        private bool isInteracting;

        private readonly Vector3 screenMiddle = new(width / 2f, height / 2f, 0);

        public static void InvokeStartInteract(InteractableObject_SO objectSO) => OnStartInteract?.Invoke(objectSO);
        public static void InvokeStopInteract() => OnStopInteract?.Invoke();

        private void Update()
        {
            if (isInteracting && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape)))
            {
                StopInteraction();
                return;
            }

            CheckForInteractableObject();
        }

        private void CheckForInteractableObject()
        {
            bool wasHovering = isHovering;

            if (!Physics.Raycast(mainCamera.ScreenPointToRay(screenMiddle), out RaycastHit hitInfo, 2.5f, interactableLayer) ||
                !hitInfo.collider.TryGetComponent(out interactable))
            {
                isHovering = false;
            }
            else
            {
                isHovering = true;
                if (Input.GetKeyDown(KeyCode.E) && !isInteracting) StartInteraction();
            }

            if (!wasHovering && isHovering) OnInteractableHovered?.Invoke();
            else if(wasHovering && !isHovering) OnInteractableExitHover?.Invoke();
        }

        private void StartInteraction()
        {
            interactable.StartInteraction();
            isInteracting = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            PlayerMovementDetails.cameraLookEnabled = false;
            PlayerMovementDetails.movementInputEnabled = false;
        }

        private void StopInteraction()
        {
            interactable.StopInteraction();
            isInteracting = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            PlayerMovementDetails.cameraLookEnabled = true;
            PlayerMovementDetails.movementInputEnabled = true;
        }
    }
}