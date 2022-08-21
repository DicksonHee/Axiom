using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Axiom.Player.Movement
{
    public class CameraLook : MonoBehaviour
    {
        [SerializeField] private Transform animatorCamPosition;
        [SerializeField] private Transform camHolder;
        [SerializeField] private Transform orientation;
        [SerializeField] private Camera cam;

        [Header("Mouse Variables")]
        [SerializeField] private float sensX;
        [SerializeField] private float sensY;
        [SerializeField] private Vector2 xRotLimits;
        [SerializeField] private float multiplier = 2f;
        

        [Header("WallRun")] 
        [SerializeField] private float lTiltAmount;
        [SerializeField] private float rTiltAmount;
        [SerializeField] private float wallRunFov;

        [Header("WallCLimb")] 
        [SerializeField] private Vector2 wallRunXRotLimits;

        private bool isAffectedByAnimator;
        private float initialFov;
        private float initialMultiplier;
        private float initialSensX;
        private float initialSensY;
        private Vector2 initialXRotLimits;

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
            initialXRotLimits = xRotLimits;
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
            orientation.transform.localRotation = Quaternion.Euler(0, yRotation, 0);
        }

        public void ResetFov() => ChangeFov(initialFov);
        public void ResetTilt() => cam.transform.DOLocalRotate(Vector3.zero, 0.25f);
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
        public void ResetXRotLimits() => xRotLimits = initialXRotLimits;

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
            ChangeFov(wallRunFov);
            ApplyCameraXAxisMultiplier(0.3f);
            ApplyCameraYAxisMultiplier(0.5f);
        }

        public void StartVaultCamera()
        {
            ApplyCameraXAxisMultiplier(0f);
            ApplyCameraYAxisMultiplier(0f);
            cam.transform.DOLocalRotate(new Vector3(0, 0, rTiltAmount), 0.25f);
        }

        public void StartHardLandingCamera(float downAngle)
        {
            LockCamera();
            cam.DOFieldOfView(110f, 0.25f);
            cam.transform.DOLocalRotate(new Vector3(downAngle, 0, 0), 0.25f);
        }

        public void StartRollCamera()
        {
            LockCamera();
            cam.transform.DOLocalRotate(new Vector3(360, 0, 0), 0.75f, RotateMode.FastBeyond360).SetEase(Ease.Flash);
        }

        public void StartClimbCamera()
        {
            cam.transform.DOLocalRotate(new Vector3(wallRunXRotLimits.y, 0, 0), 0.25f);
            xRotLimits = wallRunXRotLimits;
        }
        
        public void ResetCamera()
        {
            ResetFov();
            ResetTilt();
            ResetCameraXSens();
            ResetCameraYSens();
            ResetXRotLimits();
            UnlockCamera();
        }

        public void TransformForward(Quaternion transformation)
        {
            camHolder.transform.rotation = transformation;
            orientation.transform.rotation = transformation;
        }
    }
}

