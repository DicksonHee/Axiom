using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputDetection : MonoBehaviour
{
    public Vector3 movementInput { get; private set; }
    public bool crouchInput { get; private set; }
    public bool leftHoldInput { get; private set; }
    public bool rightHoldInput { get; private set; }

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
        movementInput = new Vector3(Input.GetAxis("Horizontal"),0f, Input.GetAxis("Vertical"));
    }

    private void DetectJumpInput()
    {
        if (Input.GetButtonDown("Jump")) OnJumpPressed?.Invoke();
    }

    private void DetectCrouchInput()
    {
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
