using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Axiom.Player.Movement
{
    public class CameraLook : MonoBehaviour
    {
        [SerializeField] private Transform camHolder;
        [SerializeField] private Transform orientation;
        [SerializeField] private Camera cam;
        [SerializeField] private Camera groundCamera;

        [Header("Mouse Variables")]
        [SerializeField] private float sensX;
        [SerializeField] private float sensY;
        [SerializeField] private Vector2 xRotLimits;
        [SerializeField] private float multiplier = 2f;
        

        [Header("WallRun")] 
        [SerializeField] private float lTiltAmount;
        [SerializeField] private float rTiltAmount;
        [SerializeField] private float wallRunFov;


        private float initialFov;
        private float initialMultiplier;
        private float initialSensX;
        private float initialSensY;
        private Quaternion gravityAlignment;

        public float mouseX { get; private set; }
        public float mouseY { get; private set; }

        public float xRotation { get; private set; }
        public float yRotation { get; private set; }


        private Vector2 orbitAngles = new(-90, 0);
        private Quaternion orbitRotation;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            initialFov = cam.fieldOfView;
            initialMultiplier = multiplier;
            initialSensX = sensX;
            initialSensY = sensY;
            
            camHolder.transform.localRotation = orbitRotation = Quaternion.Euler(orbitAngles);
            gravityAlignment = Quaternion.identity;
        }

        private void Update()
        {
            GetInput();
        }

        private void LateUpdate()
        {
            // if (ManualRotation())
            // {
            //     ConstrainAngles();
            //     orbitRotation = Quaternion.Euler(orbitAngles);
            // }
            //
            // Quaternion lookRotation = gravityAlignment * Quaternion.Euler(orbitAngles);
            //
            // camHolder.transform.localRotation = lookRotation;
            // groundCamera.transform.localRotation = lookRotation;
            // lookRotation.x = 0;
            // lookRotation.z = 0;
            // orientation.transform.localRotation = lookRotation;
        }

        private bool ManualRotation()
        {
            Vector2 input = new Vector2(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X")
            );
            const float e = 0.001f;
            if (input.x < -e || input.x > e || input.y < -e || input.y > e)
            {
                orbitAngles += multiplier * Time.unscaledDeltaTime * input;
                return true;
            }

            return false;
        }

        private void ConstrainAngles()
        {
            orbitAngles.x = Mathf.Clamp(orbitAngles.x, xRotLimits.x, xRotLimits.y);

            if (orbitAngles.y < 0f) orbitAngles.y += 360f;
            else if (orbitAngles.y >= 360f) orbitAngles.y -= 360f;
        }
        
        private void GetInput()
        {
            mouseX = Input.GetAxis("Mouse X"); 
            mouseY = Input.GetAxis("Mouse Y");
            
            //Find current look rotation
            Vector3 rot = camHolder.transform.localRotation.eulerAngles;
            yRotation = rot.y + mouseX * sensX * Time.fixedDeltaTime * multiplier;
            
            //Rotate, and also make sure we dont over- or under-rotate.
            xRotation -= mouseY * sensY * Time.fixedDeltaTime * multiplier;
            xRotation = Mathf.Clamp(xRotation, xRotLimits.x, xRotLimits.y);
            
            //Perform the rotations
            camHolder.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
            groundCamera.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
            orientation.transform.localRotation = Quaternion.Euler(0, yRotation, 0);
        }

        public void ResetFov() => ChangeFov(initialFov);
        public void ResetTilt() => ChangeTilt(0);
        public void ChangeFov(float targetFov) => cam.DOFieldOfView(targetFov, 0.25f);
        public void ChangeTilt(float zTilt) => cam.transform.DOLocalRotate(new Vector3(0,0, zTilt), 0.25f);
        public void LockCamera() => multiplier = 0;
        public void LockCameraXAxis() => sensX = 0;
        public void ApplyCameraXAxisMultiplier(float val) => sensX *= val;
        public void ResetCameraXSens() => sensX = initialSensX;
        public void LockCameraYAxis() => sensY = 0;
        public void ApplyCameraYAxisMultiplier(float val) => sensY *= val;
        public void ResetCameraYSens() => sensY = initialSensY;
        public void UnlockCamera() => multiplier = initialMultiplier;

        public void StartLeftWallRunCamera()
        {
            cam.DOFieldOfView(wallRunFov, 0.25f);
            cam.transform.DOLocalRotate(new Vector3(0,0, lTiltAmount), 0.25f);
        }
        
        public void StartRightWallRunCamera()
        {
            cam.DOFieldOfView(wallRunFov, 0.25f);
            cam.transform.DOLocalRotate(new Vector3(0,0, rTiltAmount), 0.25f);
        }

        public void StartSlideCamera()
        {
            LockCameraXAxis();
            ApplyCameraYAxisMultiplier(0.5f);
        }

        public void EndSlideCamera()
        {
            ResetCameraXSens();
            ResetCameraYSens();
        }
        
        public void ResetCamera()
        {
            ResetFov();
            ResetTilt();
        }

        public void TransformForward(Matrix4x4 transformation)
        {
            camHolder.forward = transformation.MultiplyVector(camHolder.forward);
            groundCamera.transform.forward = transformation.MultiplyVector(groundCamera.transform.forward);
            orientation.forward = transformation.MultiplyVector(orientation.forward);
        }
    }
}

