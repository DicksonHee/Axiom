using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.YamlDotNet.Serialization;
using UnityEngine;
using UnityEngine.Serialization;
using static UnityEngine.Screen;

namespace Axiom.Player.Movement
{
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
    public class RigidbodyDetection : MonoBehaviour
    {
        #region Inspector Variables
        public Transform orientation;
        public Camera camera;
        
        [Header("Ground Detection")]
        public Transform groundDetectorTransform;
        public float groundDetectorRadius = 0.5f;

        [Header("WallRun Detection")]
        public Transform wallDetectorTransform;
        public float wallRunDetectionDistance = 0.5f;

        [Header("WallClimb Detection")] 
        public float wallClimbDetectionDistance;

        [Header("Ledge Detection")]
        public float ledgeHeightDetectionDistance;
        public float ledgeDetectorForwardOffset; 
        public float ledgeDetectorHeightOffset;

        [Header("Vault Detection")]
        public Transform vaultDetectorTransform;
        public float vaultDetectionDistance;

        [Header("Crouch Detection")] 
        public Transform crouchDetectorTransform;

        [Header("Layers")]
        public LayerMask groundLayer;
        public LayerMask wallLayer;
        #endregion

        public event Action OnPlayerLanded;
        public event Action OnSlopeEnded;
        
        private bool isGrounded;
        private bool isOnSlope;

        private bool isOnWall;
        private Vector3 enterWallRunRight;
        private Vector3 enterWallRunForward;
        
        private float currentVelocity;
        
        #region Raycasts
        private RaycastHit slopeHit;
        
        private RaycastHit cameraHit;
        private GameObject cameraHitObject;

        private RaycastHit groundHit;
        
        private RaycastHit rightWallHit;
        private RaycastHit rightFrontWallHit;
        private RaycastHit rightBackWallHit;

        private RaycastHit leftWallHit;
        private RaycastHit leftFrontWallHit;
        private RaycastHit leftBackWallHit;
        
        private RaycastHit vaultHit;
        #endregion

        #region WallRun Detection
        private bool leftWallDetected;
        private bool leftFrontWallDetected;
        private bool leftBackWallDetected;
        private bool rightWallDetected;
        private bool rightFrontWallDetected;
        private bool rightBackWallDetected;
        #endregion

        #region WallClimb Detection
        private bool canWallClimb;
        #endregion

		#region Ledge Detection
        private bool isDetectingLedge;
        private RaycastHit ledgePosition;
		#endregion

		#region Vault Detection
        private bool canVaultOver;
        private bool canVaultOn;
        private float vaultAnimHeight;
        #endregion
        
        #region Crouch Detection
        private bool canUncrouch;
        #endregion

        #region Public Getters

        public bool IsGrounded() => isGrounded;
        public bool IsOnSlope() => isOnSlope;
        public bool CanWallClimb() => canWallClimb;
        public bool CanClimbLedge() => isDetectingLedge;
        public bool CanVaultOver() => canVaultOver;
        public bool CanVaultOn() => canVaultOn;
        public float GetVaultHeight() => vaultAnimHeight;
        public bool CanUncrouch() => canUncrouch;
        public RaycastHit GetSlopeHit() => slopeHit;
        #endregion

        private void Update()
        {
            GroundDetection();
            SlopeDetection();
            CrouchDetection();

            CameraRaycastDetection();
            DelegateWallDetection();
            WallClimbCheck();
            LedgeCheck();
            VaultDetection();
            
            Debug.Log(isGrounded);
        }

        #region Ground Functions
        private void GroundDetection()
        {
            bool previouslyGrounded = isGrounded;
            isGrounded = Physics.SphereCast(groundDetectorTransform.position, groundDetectorRadius, -transform.up, out groundHit, 1f, groundLayer);
            if (!previouslyGrounded && isGrounded) OnPlayerLanded?.Invoke();
        }
        #endregion
        
        #region Slope Functions
        private void SlopeDetection()
        {
            bool wasOnSlope = isOnSlope;

            if (Physics.Raycast(groundDetectorTransform.position, (Quaternion.AngleAxis(65f, orientation.right) * orientation.forward).normalized, out slopeHit, 2f, groundLayer))
            {
                isOnSlope = Vector3.Angle(slopeHit.normal, orientation.up) is > 10f and <= 45f;
            }
            else isOnSlope = false;
            
            if (wasOnSlope && !isOnSlope) OnSlopeEnded?.Invoke();
        }
        #endregion

        #region WallRun Functions
        public void SetIsOnWall(Vector3 rightVec, Vector3 forwardVec)
        {
            isOnWall = true;
            enterWallRunRight = rightVec;
            enterWallRunForward = forwardVec;
        }

        public void SetIsOffWall()
        {
            isOnWall = false;
            enterWallRunRight = Vector3.zero;
            enterWallRunForward = Vector3.zero;
        }

        private void DelegateWallDetection()
        {
            if (isOnWall) OnWallDetection();
            else WallRunDetection();
        }

        private void WallRunDetection()
        {
            var position = wallDetectorTransform.position;
            var forward = orientation.forward.normalized;
            var right = orientation.right.normalized;
            
            rightWallDetected = Physics.Raycast(position, right, out rightWallHit, wallRunDetectionDistance, wallLayer);
            rightFrontWallDetected = Physics.Raycast(position, (forward + right).normalized, out rightFrontWallHit, wallRunDetectionDistance, wallLayer);
            rightBackWallDetected = Physics.Raycast(position, (-forward + right).normalized, out rightBackWallHit, wallRunDetectionDistance, wallLayer);

            leftWallDetected = Physics.Raycast(position, -right, out leftWallHit, wallRunDetectionDistance, wallLayer);
            leftFrontWallDetected = Physics.Raycast(position, (forward + -right).normalized, out leftFrontWallHit, wallRunDetectionDistance, wallLayer);
            leftBackWallDetected = Physics.Raycast(position, (-forward + -right).normalized, out leftBackWallHit, wallRunDetectionDistance, wallLayer);
        }

        private void OnWallDetection()
        {
            var position = wallDetectorTransform.position;

            rightWallDetected = Physics.Raycast(position, enterWallRunRight.normalized, out rightWallHit, wallRunDetectionDistance, wallLayer);
            rightFrontWallDetected = Physics.Raycast(position, (enterWallRunRight + enterWallRunForward).normalized, out rightFrontWallHit, wallRunDetectionDistance, wallLayer);
            rightBackWallDetected = Physics.Raycast(position, (enterWallRunRight + -enterWallRunForward).normalized, out rightBackWallHit, wallRunDetectionDistance, wallLayer);

            leftWallDetected = Physics.Raycast(position, -enterWallRunRight.normalized, out leftWallHit, wallRunDetectionDistance, wallLayer);
            leftFrontWallDetected = Physics.Raycast(position, (-enterWallRunRight + enterWallRunForward).normalized, out leftFrontWallHit, wallRunDetectionDistance, wallLayer);
            leftBackWallDetected = Physics.Raycast(position, (-enterWallRunRight + -enterWallRunForward).normalized, out leftBackWallHit, wallRunDetectionDistance, wallLayer);
        }

        private void CameraRaycastDetection()
        {
            if (Physics.Raycast(camera.ScreenPointToRay(new Vector3(width / 2f, height / 2f, 0)), out cameraHit, 10f, groundLayer))
            {
                cameraHitObject = cameraHit.collider.gameObject;
            }
            else
            {
                cameraHitObject = null;
            }
        }

        public bool IsLeftWallDetected()
        {
            return !canWallClimb && ((leftWallDetected && leftWallHit.collider.gameObject == cameraHitObject) ||
                                     (leftFrontWallDetected && leftFrontWallHit.collider.gameObject == cameraHitObject) ||
                                     (leftBackWallDetected && leftBackWallHit.collider.gameObject == cameraHitObject));
        }
        
        public bool IsRightWallDetected()
        {
            return !canWallClimb && ((rightWallDetected && rightWallHit.collider.gameObject == cameraHitObject) ||
                                     (rightFrontWallDetected && rightFrontWallHit.collider.gameObject == cameraHitObject) ||
                                     (rightBackWallDetected && rightBackWallHit.collider.gameObject == cameraHitObject));
        }
        public bool WallRunningLeftDetected() => leftWallDetected || leftFrontWallDetected || leftBackWallDetected;
        public bool WallRunningRightDetected() => rightWallDetected || rightFrontWallDetected || rightBackWallDetected;


        public Vector3 GetLeftWallNormal()
        {
            if (leftWallDetected) return leftWallHit.normal;
            if (leftFrontWallDetected) return leftFrontWallHit.normal;
            if (leftBackWallDetected) return leftBackWallHit.normal;
            return Vector3.zero;
        }

        public Vector3 GetRightWallNormal()
        {
            if (rightWallDetected) return rightWallHit.normal;
            if (rightFrontWallDetected) return rightFrontWallHit.normal;
            if (rightBackWallDetected) return rightBackWallHit.normal;
            return Vector3.zero;
        }

        public Transform GetLeftWall()
        {
            if (leftWallDetected) return leftWallHit.transform;
            if (leftFrontWallDetected) return leftFrontWallHit.transform;
            if (leftBackWallDetected) return leftBackWallHit.transform;
            return null;
        }

        public Transform GetRightWall()
        {
            if (rightWallDetected) return rightWallHit.transform;
            if (rightFrontWallDetected) return rightFrontWallHit.transform;
            if (rightBackWallDetected) return rightBackWallHit.transform;
            return null;
        }
		#endregion

		#region WallClimb Functions
		private void WallClimbCheck()
        {
            var position = wallDetectorTransform.position;
            var forward = orientation.forward;
            var up = transform.up;

            bool rightDetected = Physics.Raycast(position, (Quaternion.AngleAxis(15f, up) * forward).normalized, wallClimbDetectionDistance, wallLayer);
            bool leftDetected = Physics.Raycast(position, (Quaternion.AngleAxis(-15f, up) * forward).normalized, wallClimbDetectionDistance, wallLayer);   

            if (rightDetected && leftDetected) canWallClimb = true;
            else canWallClimb = false;
        }
        #endregion

        #region Ledge Functions
        private void LedgeCheck()
        {
            Vector3 up = orientation.up;
            Vector3 frontPoint = orientation.position + orientation.forward.normalized * ledgeDetectorForwardOffset + up.normalized * ledgeDetectorHeightOffset;
            bool ledgeDetected = Physics.Raycast(frontPoint, -up, out ledgePosition, 5f, wallLayer);

            isDetectingLedge = ledgeDetected && ledgePosition.distance < ledgeHeightDetectionDistance;
        }

        public Vector3 GetLeftHandPosition() => ledgePosition.point + -orientation.right.normalized * 0.5f;
        public Vector3 GetRightHandPosition() =>ledgePosition.point + orientation.right.normalized * 0.5f;
        #endregion

        #region Vault Functions
        private void VaultDetection()
        {
            if (canWallClimb || IsLeftWallDetected() || IsRightWallDetected()) return;

            float vaultOnHeight = 0f;
            float vaultHeight = 0f;
            Vector3 vaultPosition = vaultDetectorTransform.position;
            Vector3 normalizedFront = orientation.forward.normalized;
            Vector3 normalizedUp = orientation.up.normalized;
            
            if (Physics.Raycast(vaultPosition, normalizedFront, out RaycastHit frontVaultHit, vaultDetectionDistance * (1 + currentVelocity / 10f), wallLayer))
            {
                if (Vector3.Dot(orientation.forward, frontVaultHit.normal) < -0.8f)
                {
                    if (Physics.Raycast(vaultPosition + normalizedUp * 1.45f + normalizedFront * frontVaultHit.distance, -normalizedUp, out vaultHit, 2f, wallLayer))
                    {
                        vaultHeight = vaultHit.distance;
                    }
                    if (Physics.Raycast(vaultPosition + normalizedUp * 1.45f + normalizedFront * (frontVaultHit.distance + 1f), -normalizedUp, out RaycastHit vaultOnHit, 2f, wallLayer))
                    {
                        vaultOnHeight = vaultOnHit.distance;
                    }
                }
            }
            else
            {
                vaultHeight = 0f;
                vaultOnHeight = 0f;
            }

            vaultAnimHeight = 1.45f - vaultHeight;
            canVaultOn = vaultAnimHeight is > 1f and < 1.35f && Mathf.Abs(vaultHeight - vaultOnHeight) < 0.1f;
            canVaultOver = vaultAnimHeight is > 1f and < 1.35f && vaultOnHeight > 0.5f;
        }
        #endregion

        #region Crouch Detection

        private void CrouchDetection()
        {
            canUncrouch = !Physics.Raycast(crouchDetectorTransform.position, orientation.up, 1f, wallLayer);
        }
        #endregion
        
        public void SetCurrentVelocity(float velocity)
        {
            currentVelocity = velocity;
        }
        
		private void OnDrawGizmos()
        {
            Vector3 wallDetectorPosition = wallDetectorTransform.position;
            Vector3 wallDetectorUp = wallDetectorTransform.up;
            Vector3 orientationForward = orientation.forward;
            
            // Wall Climb
            Gizmos.color = canWallClimb ? Color.blue : Color.red;
            Gizmos.DrawLine(wallDetectorPosition, wallDetectorPosition + (Quaternion.AngleAxis(15f, wallDetectorUp) * orientationForward).normalized * wallClimbDetectionDistance);
            Gizmos.DrawLine(wallDetectorPosition, wallDetectorPosition + (Quaternion.AngleAxis(-15f, wallDetectorUp) * orientationForward).normalized * wallClimbDetectionDistance);

            // Wall Run Right
            Gizmos.color = rightWallDetected ? Color.blue : Color.red;
            Gizmos.DrawLine(wallDetectorTransform.position, orientation.position + orientation.right.normalized * wallRunDetectionDistance);
            Gizmos.color = rightFrontWallDetected ? Color.blue : Color.red;
            Gizmos.DrawLine(wallDetectorTransform.position, orientation.position + (orientation.right + orientation.forward).normalized * wallRunDetectionDistance);
            Gizmos.color = rightBackWallDetected? Color.blue: Color.red;
            Gizmos.DrawLine(wallDetectorTransform.position, orientation.position + (orientation.right + -orientation.forward).normalized * wallRunDetectionDistance);

            // Wall Run Left
            Gizmos.color = leftWallDetected ? Color.blue : Color.red;
            Gizmos.DrawLine(wallDetectorTransform.position, orientation.position + -orientation.right.normalized * wallRunDetectionDistance);
            Gizmos.color = leftFrontWallDetected? Color.blue: Color.red;
            Gizmos.DrawLine(wallDetectorTransform.position, orientation.position + (-orientation.right + orientation.forward).normalized * wallRunDetectionDistance);
            Gizmos.color = leftBackWallDetected? Color.blue: Color.red;
            Gizmos.DrawLine(wallDetectorTransform.position, orientation.position + (-orientation.right + -orientation.forward).normalized * wallRunDetectionDistance);

            // Ground Detection
            Gizmos.color = isGrounded ? Color.blue : Color.red;
            Gizmos.DrawLine(groundDetectorTransform.position, groundDetectorTransform.position + -transform.up);
            Gizmos.DrawWireSphere(groundHit.point, groundDetectorRadius);

            // Slope Detection
            Gizmos.DrawLine(groundDetectorTransform.position, slopeHit.point);
            
            // Ledge Grab
            Gizmos.color = isDetectingLedge ? Color.blue : Color.red;
            Vector3 ledgeRaySpawnPoint = orientation.position + orientation.forward.normalized * ledgeDetectorForwardOffset + orientation.up.normalized * ledgeDetectorHeightOffset;
            Gizmos.DrawLine(ledgeRaySpawnPoint, ledgePosition.point);

            // Vault
            Gizmos.DrawLine(vaultDetectorTransform.position, vaultDetectorTransform.position + orientationForward.normalized * vaultDetectionDistance * (1 + currentVelocity / 10f));
            Gizmos.DrawLine(vaultDetectorTransform.position + orientation.forward.normalized * (vaultDetectionDistance * 1 + (currentVelocity/10f)) + orientation.up.normalized * 1.45f,
                vaultDetectorTransform.position + orientation.forward.normalized * (vaultDetectionDistance * 1 + (currentVelocity/10f)) + orientation.up.normalized * 1.45f + -orientation.up * 2f);
            Gizmos.DrawLine(vaultDetectorTransform.position + orientation.forward.normalized * (vaultDetectionDistance * 2f + (currentVelocity/10f)) + orientation.up.normalized * 1.45f,
                vaultDetectorTransform.position + orientation.forward.normalized * (vaultDetectionDistance * 2f + (currentVelocity/10f)) + orientation.up.normalized * 1.45f + -orientation.up * 2f);
            
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(GetLeftHandPosition(), 0.1f);
            Gizmos.DrawWireSphere(GetRightHandPosition(), 0.1f);
        }
    }
}

