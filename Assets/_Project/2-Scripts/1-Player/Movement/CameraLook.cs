using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Axiom.Player.Movement
{
    public class CameraLook : MonoBehaviour
    {
        [SerializeField] private float sensX;
        [SerializeField] private float sensY;
        [SerializeField] private Transform camHolder;
        [SerializeField] private Transform orientation;
        [SerializeField] private Camera cam;
        
        public float mouseX { get; private set; }
        public float mouseY { get; private set; }

        [SerializeField] private float multiplier = 2f;

        public float xRotation { get; private set; }
        public float yRotation { get; private set; }

        private float desiredX;
        private float initialFov;
        
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            initialFov = cam.fieldOfView;
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
            Vector3 rot = camHolder.transform.localRotation.eulerAngles;
            desiredX = rot.y + mouseX * sensX * Time.fixedDeltaTime * multiplier;
            
            //Rotate, and also make sure we dont over- or under-rotate.
            xRotation -= mouseY * sensY * Time.fixedDeltaTime * multiplier;
            xRotation = Mathf.Clamp(xRotation, -90f, 60f);
            
            //Perform the rotations
            camHolder.transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
            orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);
        }

        public void ResetFov() => cam.DOFieldOfView(initialFov, 0.25f);
        public void ResetTilt() => ChangeTilt(0);
        public void ChangeFov(float targetFov) => cam.DOFieldOfView(targetFov, 0.25f);
        public void ChangeTilt(float zTilt) => cam.transform.DOLocalRotate(new Vector3(0,0, zTilt), 0.25f);

    }
}

