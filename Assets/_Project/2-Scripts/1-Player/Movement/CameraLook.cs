using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Axiom.Player.Movement
{
    public class CameraLook : MonoBehaviour
    {
        [SerializeField] private float sensX;
        [SerializeField] private float sensY;
        [SerializeField] private Transform orientation;
        [SerializeField] private Camera cam;
        
        public float mouseX { get; private set; }
        public float mouseY { get; private set; }

        [SerializeField] [Range(0.001f, 0.02f)] private float multiplier = 0.01f;

        public float xRotation { get; private set; }
        public float yRotation { get; private set; }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            GetInput();

            cam.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0f);
        }

        private void GetInput()
        {
            mouseX = Input.GetAxisRaw("Mouse X");
            mouseY = Input.GetAxisRaw("Mouse Y");

            yRotation += mouseX * sensX * multiplier;
            xRotation -= mouseY * sensY * multiplier;

            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        }
    }
}

