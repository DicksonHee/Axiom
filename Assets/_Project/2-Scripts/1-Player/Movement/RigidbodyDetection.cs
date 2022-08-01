using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using static UnityEngine.Screen;

namespace Axiom.Player.Movement
{
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
    public class RigidbodyDetection : MonoBehaviour
    {
        public Transform orientation;
        public Camera camera;
        
        [Header("Ground Detection")]
        public float groundDetectorRadius = 0.5f;
        public Transform groundDetector;
        public LayerMask groundLayer;

        [Header("WallRun Detection")]
        public Transform wallDetector;
        public float wallCheckDistance = 0.5f;
        public LayerMask wallLayer;

        [Header("WallClimb Detection")] 
        public float wallClimbDetectionLength;

        [Header("Ledge Detection")]
        public Transform handPosition;
        public float ledgeDetector;
        public float ledgeHeightLength;
        public float ledgeForwardOffset; 
        public float ledgeDetectorHeightOffset;

        [Header("Vault Detection")]
        public Transform vaultDetector;
        public float maxVaultDepth;

        [HideInInspector] public RaycastHit slopeHit;

        private float currentVelocity;

        private RaycastHit cameraHit;
        private GameObject cameraHitObject;
        
        private RaycastHit rightWallHit;
        private RaycastHit rightFrontWallHit;
        private RaycastHit rightBackWallHit;

        private RaycastHit leftWallHit;
        private RaycastHit leftFrontWallHit;
        private RaycastHit leftBackWallHit;
        
        private RaycastHit vaultHit;
        private float vaultHeight;

        public bool isGrounded { get; private set; }
        public bool isOnSlope { get; private set; }

        private bool isOnWall;
        private Vector3 enterWallRunRight;
        private Vector3 enterWallRunForward;
        
        #region WallRun Detection
        public bool leftWallDetected {get; private set; }
        public bool leftFrontWallDetected {get; private set; }
        public bool leftBackWallDetected {get; private set; }
        public bool rightWallDetected { get; private set; }
        public bool rightFrontWallDetected { get; private set; }
        public bool rightBackWallDetected { get; private set; }
        #endregion

        #region WallClimb Detection
        public bool canWallClimb { get; private set; }
        public Vector3 wallClimbNormal { get; private set; }
		#endregion

		#region Ledge Detection
        public bool isDetectingLedge { get; private set; }
        private RaycastHit ledgePosition;
		#endregion

		#region Vault Detection
        public bool canVault { get; private set; }
        #endregion

		public event Action OnPlayerLanded;
        public event Action OnSlopeEnded;

        private void Update()
        {
            GroundDetection();
            SlopeDetection();

            CameraRaycastDetection();
            DelegateWallDetection();
            WallClimbCheck();
            LedgeCheck();
            VaultDetection();
            
            Debug.Log(IsLeftWallDetected());
        }

        #region Ground Functions
        private void GroundDetection()
        {
            bool previouslyGrounded = isGrounded;
            isGrounded = Physics.CheckSphere(groundDetector.position, groundDetectorRadius, groundLayer);
            if (!previouslyGrounded && isGrounded) OnPlayerLanded?.Invoke();
        }
        #endregion
        
        #region Slope Functions
        private void SlopeDetection()
        {
            bool wasOnSlope = isOnSlope;

            if (Physics.Raycast(groundDetector.position, -groundDetector.transform.up, out slopeHit, groundDetectorRadius, groundLayer))
            {
                isOnSlope = slopeHit.normal != transform.up;
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
            var position = wallDetector.position;
            var forward = orientation.forward.normalized;
            var right = orientation.right.normalized;
            
            rightWallDetected = Physics.Raycast(position, right, out rightWallHit, wallCheckDistance, wallLayer);
            rightFrontWallDetected = Physics.Raycast(position, (forward + right).normalized, out rightFrontWallHit, wallCheckDistance, wallLayer);
            rightBackWallDetected = Physics.Raycast(position, (-forward + right).normalized, out rightBackWallHit, wallCheckDistance, wallLayer);

            leftWallDetected = Physics.Raycast(position, -right, out leftWallHit, wallCheckDistance, wallLayer);
            leftFrontWallDetected = Physics.Raycast(position, (forward + -right).normalized, out leftFrontWallHit, wallCheckDistance, wallLayer);
            leftBackWallDetected = Physics.Raycast(position, (-forward + -right).normalized, out leftBackWallHit, wallCheckDistance, wallLayer);
        }

        private void OnWallDetection()
        {
            var position = wallDetector.position;

            rightWallDetected = Physics.Raycast(position, enterWallRunRight.normalized, out rightWallHit, wallCheckDistance, wallLayer);
            rightFrontWallDetected = Physics.Raycast(position, (enterWallRunRight + enterWallRunForward).normalized, out rightFrontWallHit, wallCheckDistance, wallLayer);
            rightBackWallDetected = Physics.Raycast(position, (enterWallRunRight + -enterWallRunForward).normalized, out rightBackWallHit, wallCheckDistance, wallLayer);

            leftWallDetected = Physics.Raycast(position, -enterWallRunRight.normalized, out leftWallHit, wallCheckDistance, wallLayer);
            leftFrontWallDetected = Physics.Raycast(position, (-enterWallRunRight + enterWallRunForward).normalized, out leftFrontWallHit, wallCheckDistance, wallLayer);
            leftBackWallDetected = Physics.Raycast(position, (-enterWallRunRight + -enterWallRunForward).normalized, out leftBackWallHit, wallCheckDistance, wallLayer);
        }

        private void CameraRaycastDetection()
        {
            if (Physics.Raycast(camera.ScreenPointToRay(new Vector3(width / 2, height / 2, 0)), out cameraHit, 10f, groundLayer))
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
            var position = wallDetector.position;
            var forward = orientation.forward;
            var up = transform.up;

            bool rightDetected = Physics.Raycast(position, (Quaternion.AngleAxis(15f, up) * forward).normalized, out RaycastHit rightWallClimbDetected, wallClimbDetectionLength, wallLayer);
            bool leftDetected = Physics.Raycast(position, (Quaternion.AngleAxis(-15f, up) * forward).normalized, out RaycastHit leftWallClimbDetected, wallClimbDetectionLength, wallLayer);   

            if (rightDetected && leftDetected) canWallClimb = true;
            else canWallClimb = false;

            wallClimbNormal = rightWallClimbDetected.normal;
        }
        #endregion

        #region Ledge Functions
        private void LedgeCheck()
        {
            Vector3 frontPoint = orientation.position + orientation.forward.normalized * ledgeForwardOffset + orientation.up.normalized * ledgeDetectorHeightOffset;
            bool ledgeDetected = Physics.Raycast(frontPoint, -transform.up, out ledgePosition, 99f, wallLayer);

            if (ledgeDetected && ledgePosition.distance < ledgeHeightLength) isDetectingLedge = true; 
            else isDetectingLedge = false;
        }

        public Vector3 GetLedgePosition() => ledgePosition.point;
        public Vector3 GetLedgeHandDifference() => GetRightHandPosition() - handPosition.position;
        public Vector3 GetLeftHandPosition() => ledgePosition.point + -orientation.right.normalized * 0.5f;
        public Vector3 GetRightHandPosition() =>ledgePosition.point + orientation.right.normalized * 0.5f;
        #endregion

        #region Vault Functions
        private void VaultDetection()
        {
            if (canWallClimb || IsLeftWallDetected() || IsRightWallDetected()) return;

            if (Physics.Raycast(vaultDetector.position, orientation.forward.normalized, out RaycastHit frontVaultHit, maxVaultDepth * (1 + currentVelocity / 10f), wallLayer))
            {
                if (Physics.Raycast(frontVaultHit.point + orientation.up.normalized * 0.1f, -orientation.up, out vaultHit, 2f, wallLayer))
                {
                    Debug.Log("ASD");
                    vaultHeight = 1.35f - vaultHit.distance;
                }
            }
            else
            {
                vaultHeight = 0f;
            }

            canVault = vaultHeight is > 0.5f and < 1.35f;
        }
        #endregion

        public void SetCurrentVelocity(float velocity)
        {
            currentVelocity = velocity;
        }
        
		private void OnDrawGizmos()
        {
            // Wall Climb
            Gizmos.color = canWallClimb ? Color.blue : Color.red;
            Gizmos.DrawLine(wallDetector.position, wallDetector.position + (Quaternion.AngleAxis(15f, wallDetector.up) * orientation.forward).normalized * wallClimbDetectionLength);
            Gizmos.DrawLine(wallDetector.position, wallDetector.position + (Quaternion.AngleAxis(-15f, wallDetector.up) * orientation.forward).normalized * wallClimbDetectionLength);

            // Wall Run Right
            Gizmos.color = rightWallDetected ? Color.blue : Color.red;
            Gizmos.DrawLine(wallDetector.position, orientation.position + orientation.right.normalized * wallCheckDistance);
            Gizmos.color = rightFrontWallDetected ? Color.blue : Color.red;
            Gizmos.DrawLine(wallDetector.position, orientation.position + (orientation.right + orientation.forward).normalized * wallCheckDistance);
            Gizmos.color = rightBackWallDetected? Color.blue: Color.red;
            Gizmos.DrawLine(wallDetector.position, orientation.position + (orientation.right + -orientation.forward).normalized * wallCheckDistance);

            // Wall Run Left
            Gizmos.color = leftWallDetected ? Color.blue : Color.red;
            Gizmos.DrawLine(wallDetector.position, orientation.position + -orientation.right.normalized * wallCheckDistance);
            Gizmos.color = leftFrontWallDetected? Color.blue: Color.red;
            Gizmos.DrawLine(wallDetector.position, orientation.position + (-orientation.right + orientation.forward).normalized * wallCheckDistance);
            Gizmos.color = leftBackWallDetected? Color.blue: Color.red;
            Gizmos.DrawLine(wallDetector.position, orientation.position + (-orientation.right + -orientation.forward).normalized * wallCheckDistance);

            // Ground Detection
            Gizmos.color = isGrounded ? Color.blue : Color.red;
            Gizmos.DrawWireSphere(groundDetector.position, groundDetectorRadius);

            // Ledge Grab
            Gizmos.color = isDetectingLedge ? Color.blue : Color.red;
            Vector3 ledgeRaySpawnPoint = orientation.position + orientation.forward.normalized * ledgeForwardOffset + orientation.up.normalized * ledgeDetectorHeightOffset;
            Gizmos.DrawLine(ledgeRaySpawnPoint, ledgePosition.point);

            // Vault
            Gizmos.DrawLine(vaultDetector.position + orientation.forward.normalized * (maxVaultDepth * 1 + (currentVelocity/10f)),
                vaultDetector.position + orientation.forward.normalized * (maxVaultDepth * 1 + (currentVelocity/10f)) + -orientation.up * 2f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(GetLeftHandPosition(), 0.1f);
            Gizmos.DrawWireSphere(GetRightHandPosition(), 0.1f);
        }
    }
}

