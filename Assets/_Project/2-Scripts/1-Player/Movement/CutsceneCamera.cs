using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Axiom.Player.Movement
{
    public class CutsceneCamera : MonoBehaviour
    {
        public Camera mainCam;
        
        [Header("Mouse Variables")]
        [SerializeField] private float sensX;
        [SerializeField] private float sensY;
        [SerializeField] private Vector2 xRotLimits;
        [SerializeField] private Vector2 yRotLimits;
        [SerializeField] private float multiplier = 2f;
        
        public float mouseX { get; private set; }
        public float mouseY { get; private set; }

        public float xRotation { get; private set; }
        public float yRotation { get; private set; }

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            GetInput();
        }

        private void GetInput()
        {
            mouseX = Input.GetAxis("Mouse X"); 
            mouseY = Input.GetAxis("Mouse Y");

            //Rotate, and also make sure we dont over- or under-rotate.
            xRotation -= mouseY * sensY * Time.fixedDeltaTime * multiplier;
            yRotation += mouseX * sensX * Time.fixedDeltaTime * multiplier;

            xRotation = Mathf.Clamp(xRotation, xRotLimits.x, xRotLimits.y);
            yRotation = Mathf.Clamp(yRotation, yRotLimits.x, yRotLimits.y);

            //Perform the rotations
            mainCam.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
        }
    }
}