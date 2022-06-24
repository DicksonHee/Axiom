using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputDetection : MonoBehaviour
{
    public Vector3 movementInput { get; private set; }
    public bool jumpInput { get; private set; }
    public bool crouchInput { get; private set; }
    public bool leftHoldInput { get; private set; }
    public bool rightHoldInput { get; private set; }

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
        if (Input.GetButtonDown("Jump")) jumpInput = true;
        else if (Input.GetButtonUp("Jump")) jumpInput = false;
    }

    private void DetectCrouchInput()
    {
        if (Input.GetButtonDown("Crouch")) crouchInput = true;
        else if (Input.GetButtonUp("Crouch")) crouchInput = false;
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
