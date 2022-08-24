using System;
using UnityEngine;

namespace Axiom.Player.Movement
{
    public class InputDetection : MonoBehaviour
    {
        public Vector3 movementInput { get; private set; }
        public bool crouchInput { get; private set; }
        public bool leftHoldInput { get; private set; }
        public bool rightHoldInput { get; private set; }
        public float crouchPressedTime { get; private set; }

        private PlayerInputActions playerInputActions;

        public event Action OnJumpPressed;

        private void Awake()
        {
            playerInputActions = new PlayerInputActions();

            playerInputActions.Enable();
        }

        private void Update()
        {
            DetectMovementInput();
            DetectJumpInput();
            DetectCrouchInput();
            DetectLeftHoldInput();
            DetectRightHoldInput();
        }

        private void DetectMovementInput()
        {
            movementInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        }

        private void DetectJumpInput()
        {
            if (Input.GetButtonDown("Jump")) OnJumpPressed?.Invoke();
        }

        private void DetectCrouchInput()
        {
            if (Input.GetKeyDown(KeyCode.LeftControl)) crouchPressedTime = Time.time;
            crouchInput = Input.GetKey(KeyCode.LeftControl);
        }

        private void DetectLeftHoldInput()
        {
            if (Input.GetMouseButtonDown(0)) leftHoldInput = true;
            else if (Input.GetMouseButtonUp(0)) leftHoldInput = false;
        }

        private void DetectRightHoldInput()
        {
            if (Input.GetMouseButtonDown(1)) rightHoldInput = true;
            else if (Input.GetMouseButtonDown(1)) rightHoldInput = false;
        }
    }
}
