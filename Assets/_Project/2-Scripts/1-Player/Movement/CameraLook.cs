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

        [SerializeField] private float multiplier = 2f;

        public float xRotation { get; private set; }
        public float yRotation { get; private set; }

        private float desiredX;
        
        private void Start()
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
            
            //Find current look rotation
            Vector3 rot = cam.transform.localRotation.eulerAngles;
            desiredX = rot.y + mouseX * sensX * Time.fixedDeltaTime * multiplier;
            
            //Rotate, and also make sure we dont over- or under-rotate.
            xRotation -= mouseY * sensY * Time.fixedDeltaTime * multiplier;
            xRotation = Mathf.Clamp(xRotation, -90f, 60f);
            
            //Perform the rotations
            cam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
            orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);
        }
    }
}

