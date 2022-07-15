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
        
        public float mouseX { get; private set; }
        public float mouseY { get; private set; }

        public float xRotation { get; private set; }
        public float yRotation { get; private set; }


        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            initialFov = cam.fieldOfView;
            initialMultiplier = multiplier;
            initialSensX = sensX;
            initialSensY = sensY;
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
    }
}

