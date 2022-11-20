using System.Collections;
using System.Numerics;
using Axiom.Core;
using DG.Tweening;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Axiom.Player.Movement
{
    public class CameraLook : MonoBehaviour
    {
        [SerializeField] private Transform animatorCamPosition;
        [SerializeField] private Transform camHolder;
        [SerializeField] private Transform orientation;
        [SerializeField] private Camera cam;
        [SerializeField] private Camera noPostCam;
        [SerializeField] private PlayerAnimation playerAnimation;
        [SerializeField] private RigidbodyDetection rbInfo;

        [Header("Mouse Variables")] [SerializeField]
        private float sensX;

        [SerializeField] private float sensY;
        [SerializeField] private Vector2 xRotLimits;
        [SerializeField] private float multiplier = 2f;


        [Header("WallRun")] [SerializeField] private float lTiltAmount;
        [SerializeField] private float rTiltAmount;
        [SerializeField] private float wallRunFov;

        [Header("WallCLimb")] [SerializeField] private Vector2 wallRunXRotLimits;

        private bool isAffectedByAnimator;
        private float initialFov;
        private float initialMultiplier;
        private float initialSensX;
        private float initialSensY;
        private Vector2 initialXRotLimits;
        private bool disableInput;
        private bool addYRot;
        private bool addXRot;
        private bool subYRot;
        private GameObject currentWall;

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
            if (!PlayerMovementDetails.cameraLookEnabled) return;

            GetInput();
        }

        private void GetInput()
        {
            if (multiplier == 0f) return;

            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");

            //Find current look rotation
            Vector3 rot = camHolder.transform.localRotation.eulerAngles;
            rot = ApplyAdditionalYRot(rot);
            yRotation = rot.y + mouseX * sensX * Time.fixedDeltaTime * multiplier;
            
            //Rotate, and also make sure we dont over- or under-rotate.
            xRotation -= mouseY * sensY * Time.fixedDeltaTime * multiplier;
            ApplyAdditionalXRot();
            xRotation = Mathf.Clamp(xRotation, xRotLimits.x, xRotLimits.y);

            //Perform the rotations
            camHolder.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
            orientation.transform.localRotation = Quaternion.Euler(0, yRotation, 0);
        }

        private Vector3 ApplyAdditionalYRot(Vector3 initialRot)
        {
            if (addYRot)
            {
                initialRot.y += 3f;
                if (!rbInfo.IsLookingAtWall(currentWall)) addYRot = false;
            }
            if (subYRot)
            {
                initialRot.y -= 3f;
                if (!rbInfo.IsLookingAtWall(currentWall)) subYRot = false;
            }

            return initialRot;
        }

        private void ApplyAdditionalXRot()
        {
            if (addXRot)
            {
                xRotation -= 5f;
                if (xRotation <= xRotLimits.x) addXRot = false;
            }
        }
        
        public void ResetFov() => ChangeFov(initialFov);

        public void ResetTilt()
        {
            cam.transform.DOLocalRotate(Vector3.zero, 0.25f);
            noPostCam.transform.DOLocalRotate(Vector3.zero, 0.25f);
        }

        public void ChangeFov(float targetFov)
        {
            cam.DOFieldOfView(targetFov, 0.25f);
            noPostCam.DOFieldOfView(targetFov, 0.25f);
        }

        public void ChangeTilt(float zTilt)
        {
            cam.transform.DOLocalRotate(new Vector3(0, 0, zTilt), 0.25f);
            noPostCam.transform.DOLocalRotate(new Vector3(0, 0, zTilt), 0.25f);
        }

        public void LockCamera() => multiplier = 0;
        public void LockCameraXAxis() => sensX = 0;
        public void ApplyCameraXAxisMultiplier(float val) => sensX *= val;
        public void ResetCameraXSens() => sensX = initialSensX;
        public void LockCameraYAxis() => sensY = 0;
        public void ApplyCameraYAxisMultiplier(float val) => sensY *= val;
        public void ResetCameraYSens() => sensY = initialSensY;
        public void UnlockCamera() => multiplier = initialMultiplier;
        public void ResetXRotLimits() => xRotLimits = initialXRotLimits;

        public void StartLeftWallRunCamera(GameObject wall)
        {
            ChangeFov(wallRunFov);
            ChangeTilt(lTiltAmount);
            addYRot = true;
            currentWall = wall;
        }
        
        public void StartRightWallRunCamera(GameObject wall)
        {
            ChangeFov(wallRunFov);
            ChangeTilt(rTiltAmount);
            subYRot = true;
            currentWall = wall;
        }

        public void StartSlideCamera()
        {
            //ChangeFov(wallRunFov);
            ApplyCameraXAxisMultiplier(0.3f);
            ApplyCameraYAxisMultiplier(0.5f);
        }

        public void StartVaultCamera()
        {
            ApplyCameraXAxisMultiplier(0f);
            ApplyCameraYAxisMultiplier(0f);
            ChangeTilt(rTiltAmount);
        }

        public void StartHardLandingCamera()
        {
            LockCamera();
            camHolder.DOLocalRotate(new Vector3(70f, yRotation, 0), 0.15f);
            
            Invoke(nameof(EndHardLandingCamera), 1.5f);
        }

        public void EndHardLandingCamera()
        {
            xRotation = 0f;
            camHolder.DOLocalRotate(new Vector3(0f, yRotation, 0), 0.25f);
            Invoke(nameof(ResetCamera), 0.25f);
        }
        
        public void StartRollCamera()
        {
            LockCamera();
            StartCoroutine(RotateCam_CO(new Vector3(360f, 0, 0), 0.4f));
            //cam.transform.DOLocalRotate(new Vector3(360f, 0, 0), 0.6f, RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.Linear);
            Invoke(nameof(ShowModel), 0.4f);
        }

        private void ShowModel() => playerAnimation.ShowModel();

        private IEnumerator RotateCam_CO(Vector3 rotateAmount, float duration)
        {
            Vector3 initialRot = cam.transform.localRotation.eulerAngles;
            float counter = 0f;

            while (counter < duration)
            {
                counter += Time.deltaTime;
                cam.transform.localRotation = Quaternion.Euler(Vector3.Lerp(initialRot, rotateAmount, counter/duration));
                noPostCam.transform.localRotation = Quaternion.Euler(Vector3.Lerp(initialRot, rotateAmount, counter/duration));
                yield return null;
            }
            
            cam.transform.localRotation = Quaternion.Euler(rotateAmount);
            noPostCam.transform.localRotation = Quaternion.Euler(rotateAmount);
        }
        
        public void StartClimbCamera()
        {
            addXRot = true;
            //cam.transform.DOLocalRotate(new Vector3(wallRunXRotLimits.y, 0, 0), 0.25f);
            //noPostCam.transform.DOLocalRotate(new Vector3(wallRunXRotLimits.y, 0, 0), 0.25f);
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

            addXRot = false;
            addYRot = false;
            subYRot = false;
        }

        public void TransformForwardRotateTo(Quaternion rot)
        {
            transform.localRotation = Quaternion.identity;
            camHolder.localRotation = rot;
            orientation.localRotation = rot;
        }

        public void TransformForwardRotateBy(Quaternion transformation)
        {
            Vector3 targetTrans = transformation.eulerAngles;

            print($"before: {camHolder.localRotation.eulerAngles}");
            camHolder.localRotation = Quaternion.Euler(targetTrans) * camHolder.localRotation;
            print($"after: {camHolder.localRotation.eulerAngles}");
            //targetTrans.z = 0;
            yRotation = camHolder.localRotation.eulerAngles.y;
            //xRotation = (camHolder.localRotation.eulerAngles.x - 180);
            xRotation -= targetTrans.x;
            //targetTrans.x = 0;
            orientation.localRotation = Quaternion.Euler(targetTrans) * orientation.localRotation;

            Debug.Log(targetTrans + new Vector3(0, yRotation, 0));

            playerAnimation.ForceRotate();
        }
    }
}

