using System;
using UnityEngine;
using Axiom.Core;
using static UnityEngine.Screen;

namespace Axiom.Player.Movement
{
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
    public class RigidbodyDetection : MonoBehaviour
    {
        #region Inspector Variables
        public Transform orientation;
        public Camera cam;

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
        private bool isLookingAtWall;
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

        private RaycastHit rightWallClimbHit;
        private RaycastHit leftWallClimbHit;

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
        private bool leftWallClimbDetected;
        private bool rightWallClimbDetected;
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

        private Vector3 upDirection;
        private Vector3 rightDirection;
        private Vector3 forwardDirection;
        private readonly Vector3 screenMiddle = new(width / 2f, height / 2f, 0);

        #region Public Getters

        public bool IsGrounded() => isGrounded;
        public bool IsOnSlope() => isOnSlope;
        public bool CanWallClimb() => canWallClimb;
        public bool CanClimbLedge() => isDetectingLedge;
        public bool CanVaultOver() => canVaultOver;
        public bool IsLookingAtWall(GameObject currentWall) => isLookingAtWall && cameraHitObject == currentWall;
        public bool CanVaultOn() => canVaultOn;
        public float GetVaultHeight() => vaultAnimHeight;
        public bool CanUncrouch() => canUncrouch;
        public RaycastHit GetSlopeHit() => slopeHit;

        public string GetCurrentFloorMaterial()
        {
            string retString = "";
            if (isGrounded)
            {
                if (!groundHit.collider.TryGetComponent(out MeshRenderer floorRenderer)) return "";
                retString = floorRenderer.material.name.Replace("(Instance)", "").Replace(" ", "");
            }
            else if(isOnWall)
            {
                if (!cameraHitObject.TryGetComponent(out MeshRenderer wallRenderer)) return "";
                retString = wallRenderer.material.name.Replace("(Instance)", "").Replace(" ", "");
            }
            else if (canWallClimb)
            {
                if (!rightWallClimbHit.collider.TryGetComponent(out MeshRenderer frontWallRenderer)) return "";
                retString = frontWallRenderer.material.name.Replace("(Instance)", "").Replace(" ", "");
            }

            return retString;
        }
		#endregion

        private void Update()
        {
            upDirection = orientation.up;
            rightDirection = orientation.right;
            forwardDirection = orientation.forward;

            GroundDetection();
            SlopeDetection();
            CrouchDetection();

            CameraRaycastDetection();
            DelegateWallDetection();
            WallClimbCheck();
            LedgeCheck();
            VaultDetection();
        }

        #region Ground Functions
        private void GroundDetection()
        {
            bool previouslyGrounded = isGrounded;
            isGrounded = Physics.SphereCast(groundDetectorTransform.position, groundDetectorRadius, -upDirection, out groundHit, 1f, groundLayer);
            if (!previouslyGrounded && isGrounded)
            {
                //ValidGroundDetection();
                OnPlayerLanded?.Invoke();
                if (groundHit.collider.TryGetComponent(out IPlayerLand playerLand))
                    playerLand.OnPlayerLand();
            }
        }
        #endregion
        
        #region Slope Functions
        private void SlopeDetection()
        {
            bool wasOnSlope = isOnSlope;

            if (Physics.Raycast(groundDetectorTransform.position, (Quaternion.AngleAxis(60f, rightDirection) * forwardDirection).normalized, out slopeHit, 2f, groundLayer))
            {
                isOnSlope = Vector3.Angle(slopeHit.normal, upDirection) is > 5f and <= 45f or >= -45f and < -5f;
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

            rightWallDetected = Physics.Raycast(position, rightDirection, out rightWallHit, wallRunDetectionDistance, wallLayer);
            rightFrontWallDetected = Physics.Raycast(position, (forwardDirection + rightDirection).normalized, out rightFrontWallHit, wallRunDetectionDistance, wallLayer);
            rightBackWallDetected = Physics.Raycast(position, (-forwardDirection + rightDirection).normalized, out rightBackWallHit, wallRunDetectionDistance, wallLayer);

            leftWallDetected = Physics.Raycast(position, -rightDirection, out leftWallHit, wallRunDetectionDistance, wallLayer);
            leftFrontWallDetected = Physics.Raycast(position, (forwardDirection + -rightDirection).normalized, out leftFrontWallHit, wallRunDetectionDistance, wallLayer);
            leftBackWallDetected = Physics.Raycast(position, (-forwardDirection + -rightDirection).normalized, out leftBackWallHit, wallRunDetectionDistance, wallLayer);
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
            isLookingAtWall = Physics.SphereCast(cam.ScreenPointToRay(screenMiddle), 0.1f, out cameraHit, 10f, groundLayer);
            
            if (isLookingAtWall)
            {
                cameraHitObject = cameraHit.collider.gameObject;
            }
        }

        public bool IsLeftWallDetected()
        {
            return !canWallClimb && ((leftWallDetected && leftWallHit.collider.gameObject == cameraHitObject) ||
                                     (leftFrontWallDetected && leftFrontWallHit.collider.gameObject == cameraHitObject) ||
                                     (leftBackWallDetected && leftBackWallHit.collider.gameObject == cameraHitObject) ||
                                     (leftWallClimbDetected && leftWallClimbHit.collider.gameObject == cameraHitObject));
        }
        
        public bool IsRightWallDetected()
        {
            return !canWallClimb && ((rightWallDetected && rightWallHit.collider.gameObject == cameraHitObject) ||
                                     (rightFrontWallDetected && rightFrontWallHit.collider.gameObject == cameraHitObject) ||
                                     (rightBackWallDetected && rightBackWallHit.collider.gameObject == cameraHitObject) ||
                                     (rightWallClimbDetected && rightWallClimbHit.collider.gameObject == cameraHitObject));
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

            rightWallClimbDetected = Physics.Raycast(position, (Quaternion.AngleAxis(20f, upDirection) * forwardDirection).normalized, out rightWallClimbHit, wallClimbDetectionDistance, wallLayer);
            leftWallClimbDetected = Physics.Raycast(position, (Quaternion.AngleAxis(-20f, upDirection) * forwardDirection).normalized, out leftWallClimbHit, wallClimbDetectionDistance, wallLayer);

            if (rightWallClimbDetected && leftWallClimbDetected) canWallClimb = true;
            else canWallClimb = false;
        }
        #endregion

        #region Ledge Functions
        private void LedgeCheck()
        {
            Vector3 frontPoint = orientation.position + forwardDirection * ledgeDetectorForwardOffset + upDirection * ledgeDetectorHeightOffset;
            bool ledgeDetected = Physics.SphereCast(frontPoint, 0.3f, -upDirection, out ledgePosition, 1f, groundLayer);
            isDetectingLedge = ledgeDetected && ledgePosition.distance < ledgeHeightDetectionDistance;
        }

        public Vector3 GetLeftHandPosition() => ledgePosition.point + -rightDirection * 0.5f;
        public Vector3 GetRightHandPosition() => ledgePosition.point + rightDirection * 0.5f;
        #endregion

        #region Vault Functions
        private void VaultDetection()
        {
            if (canWallClimb || IsLeftWallDetected() || IsRightWallDetected()) return;

            Vector3 vaultPosition = vaultDetectorTransform.position;
            float vaultOnHeight = 0f;
            float vaultHeight = 0f;

            if (Physics.Raycast(vaultPosition, forwardDirection, out RaycastHit frontVaultHit, vaultDetectionDistance * (1 + currentVelocity / 10f), wallLayer))
            {
                if (Vector3.Dot(orientation.forward, frontVaultHit.normal) < -0.8f)
                {
                    if (Physics.Raycast(vaultPosition + upDirection * 1.45f + forwardDirection * frontVaultHit.distance, -upDirection, out vaultHit, 2f, wallLayer))
                    {
                        vaultHeight = vaultHit.distance;
                    }
                    if (Physics.Raycast(vaultPosition + upDirection * 1.45f + forwardDirection * (frontVaultHit.distance + 1f), -upDirection, out RaycastHit vaultOnHit, 2f, wallLayer))
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
            canUncrouch = !Physics.Raycast(crouchDetectorTransform.position, upDirection, 1f, wallLayer);
        }
        #endregion

        #region ValidGround Functions

        // private void ValidGroundDetection()
        // {
        //     bool isValidRespawnPoint = true;
        //     int sectionDegrees = 360 / 6;
        //     Vector3 currentPosition = transform.position;
        //     for (int ii = 0; ii < 6; ii++)
        //     {
        //         if (!Physics.Raycast((currentPosition + Quaternion.AngleAxis(ii * sectionDegrees, upDirection) * forwardDirection).normalized * 1f, -upDirection, 3f, wallLayer))
        //         {
        //             isValidRespawnPoint = false;
        //             break;
        //         }
        //     }
        //
        //     if (isValidRespawnPoint)
        //     {
        //         PlayerMovementDetails.lastGroundedPosition = currentPosition;
        //         PlayerMovementDetails.lastGroundedRotation = transform.rotation.eulerAngles;
        //     }
        // }

		#endregion

		public void SetCurrentVelocity(float velocity)
        {
            currentVelocity = velocity;
        }
        
		private void OnDrawGizmos()
        {
            upDirection = orientation.up;
            rightDirection = orientation.right;
            forwardDirection = orientation.forward;
            Vector3 wallDetectorPosition = wallDetectorTransform.position;
            Vector3 wallDetectorUp = wallDetectorTransform.up;

			// Wall Climb
			Gizmos.color = canWallClimb ? Color.blue : Color.red;
            Gizmos.DrawLine(wallDetectorPosition, wallDetectorPosition + (Quaternion.AngleAxis(15f, wallDetectorUp) * forwardDirection).normalized * wallClimbDetectionDistance);
            Gizmos.DrawLine(wallDetectorPosition, wallDetectorPosition + (Quaternion.AngleAxis(-15f, wallDetectorUp) * forwardDirection).normalized * wallClimbDetectionDistance);

            // Wall Run Right
            Gizmos.color = rightWallDetected ? Color.blue : Color.red;
            Gizmos.DrawLine(wallDetectorTransform.position, orientation.position + rightDirection * wallRunDetectionDistance);
            Gizmos.color = rightFrontWallDetected ? Color.blue : Color.red;
            Gizmos.DrawLine(wallDetectorTransform.position, orientation.position + (rightDirection + forwardDirection).normalized * wallRunDetectionDistance);
            Gizmos.color = rightBackWallDetected? Color.blue: Color.red;
            Gizmos.DrawLine(wallDetectorTransform.position, orientation.position + (rightDirection + -forwardDirection).normalized * wallRunDetectionDistance);

            // Wall Run Left
            Gizmos.color = leftWallDetected ? Color.blue : Color.red;
            Gizmos.DrawLine(wallDetectorTransform.position, orientation.position + -rightDirection * wallRunDetectionDistance);
            Gizmos.color = leftFrontWallDetected? Color.blue: Color.red;
            Gizmos.DrawLine(wallDetectorTransform.position, orientation.position + (-rightDirection + forwardDirection).normalized * wallRunDetectionDistance);
            Gizmos.color = leftBackWallDetected? Color.blue: Color.red;
            Gizmos.DrawLine(wallDetectorTransform.position, orientation.position + (-rightDirection + -forwardDirection).normalized * wallRunDetectionDistance);

            // Ground Detection
            Gizmos.color = isGrounded ? Color.blue : Color.red;
            Gizmos.DrawLine(groundDetectorTransform.position, groundDetectorTransform.position + -upDirection);
            Gizmos.DrawWireSphere(groundHit.point, groundDetectorRadius);

            // Slope Detection
            if(IsOnSlope()) Gizmos.DrawLine(groundDetectorTransform.position, slopeHit.point);
            else Gizmos.DrawLine(groundDetectorTransform.position, groundDetectorTransform.position + (Quaternion.AngleAxis(60f, rightDirection) * forwardDirection).normalized * 2f);
            
            // Ledge Grab
            Gizmos.color = isDetectingLedge ? Color.blue : Color.red;
            Vector3 ledgeRaySpawnPoint = orientation.position + forwardDirection * ledgeDetectorForwardOffset + upDirection * ledgeDetectorHeightOffset;
            if (CanClimbLedge())
            {
                Gizmos.DrawLine(ledgeRaySpawnPoint, ledgePosition.point);
                Gizmos.DrawWireSphere(ledgePosition.point, 0.3f);
            }
            else
            {
                Gizmos.DrawLine(ledgeRaySpawnPoint, ledgeRaySpawnPoint + -upDirection);
                Gizmos.DrawWireSphere(ledgeRaySpawnPoint + -orientation.up, 0.3f);
            }

            // Vault
            Gizmos.DrawLine(vaultDetectorTransform.position, vaultDetectorTransform.position + forwardDirection * vaultDetectionDistance * (1 + currentVelocity / 10f));
            Gizmos.DrawLine(vaultDetectorTransform.position + forwardDirection * (vaultDetectionDistance * 1 + (currentVelocity/10f)) + upDirection * 1.45f,
                vaultDetectorTransform.position + forwardDirection * (vaultDetectionDistance * 1 + (currentVelocity/10f)) + upDirection * 1.45f + -upDirection * 2f);
            Gizmos.DrawLine(vaultDetectorTransform.position + forwardDirection * (vaultDetectionDistance * 2f + (currentVelocity/10f)) + upDirection * 1.45f,
                vaultDetectorTransform.position + forwardDirection * (vaultDetectionDistance * 2f + (currentVelocity/10f)) + upDirection * 1.45f + -upDirection * 2f);
            
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(GetLeftHandPosition(), 0.1f);
            Gizmos.DrawWireSphere(GetRightHandPosition(), 0.1f);
        }
    }
}

