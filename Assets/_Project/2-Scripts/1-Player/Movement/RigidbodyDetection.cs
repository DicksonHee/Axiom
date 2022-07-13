using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Axiom.Player.Movement
{
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
    public class RigidbodyDetection : MonoBehaviour
    {
        public Transform orientation;

        [Header("Ground Detection")]
        public float groundDetectorRadius = 0.5f;
        public Transform groundDetector;
        public LayerMask groundLayer;

        [Header("WallRun Detection")]
        public Transform wallDetector;
        public float wallCheckDistance = 0.5f;
        public LayerMask wallLayer;

        private Rigidbody _rb;
        private CapsuleCollider _collider;

        public RaycastHit slopeHit;
        public RaycastHit rightWallHit;
        public RaycastHit rightFrontWallHit;
        public RaycastHit rightBackWallHit;
        public RaycastHit leftWallHit;
        public RaycastHit leftFrontWallHit;
        public RaycastHit leftBackWallHit;

        public bool isGrounded { get; private set; }
        public bool isOnSlope { get; private set; }
        public bool leftWallDetected {get; private set; }
        public bool leftFrontWallDetected {get; private set; }
        public bool leftBackWallDetected {get; private set; }
        public bool rightWallDetected { get; private set; }
        public bool rightFrontWallDetected { get; private set; }
        public bool rightBackWallDetected { get; private set; }

        public event Action OnPlayerLanded;
        public event Action OnSlopeEnded;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _collider = GetComponent<CapsuleCollider>();
        }

        private void Update()
        {
            GroundDetection();
            SlopeDetection();
            WallRunDetection();

            Debug.Log("L: " + IsLeftWallDetected());
            Debug.Log("R: " + IsRightWallDetected());
        }

        private void GroundDetection()
        {
            bool previouslyGrounded = isGrounded;
            isGrounded = Physics.CheckSphere(groundDetector.position, groundDetectorRadius, groundLayer);
            if (!previouslyGrounded && isGrounded) OnPlayerLanded?.Invoke();
        }
        
        private void SlopeDetection()
        {
            bool wasOnSlope = isOnSlope;

            if (Physics.Raycast(groundDetector.position, groundDetector.TransformDirection(Vector3.down), out slopeHit,
                    groundDetectorRadius, groundLayer))
            {
                isOnSlope = slopeHit.normal != Vector3.up;
            }
            else isOnSlope = false;

            if (wasOnSlope && !isOnSlope) OnSlopeEnded?.Invoke();
        }

        private void WallRunDetection()
        {
            var position = orientation.position;
            var right = orientation.right;
            var back = -orientation.forward;

            rightWallDetected = Physics.Raycast(position, right.normalized, out rightWallHit, wallCheckDistance, wallLayer);
            leftWallDetected = Physics.Raycast(position, -right.normalized, out leftWallHit, wallCheckDistance, wallLayer);
            rightFrontWallDetected = Physics.Raycast(position, (right + -back).normalized, out rightFrontWallHit, wallCheckDistance * 1.5f, wallLayer);
            leftFrontWallDetected = Physics.Raycast(position, (-right + -back).normalized, out leftFrontWallHit, wallCheckDistance * 1.5f, wallLayer);           
            rightBackWallDetected = Physics.Raycast(position, (right + back).normalized, out rightBackWallHit, wallCheckDistance * 1.5f, wallLayer);
            leftBackWallDetected = Physics.Raycast(position, (-right + back).normalized, out leftBackWallHit, wallCheckDistance * 1.5f, wallLayer);
        }

        public bool IsLeftWallDetected() => leftWallDetected || leftFrontWallDetected || leftBackWallDetected;
        public bool IsRightWallDetected() => rightWallDetected || rightFrontWallDetected || rightBackWallDetected;
        
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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(orientation.position, orientation.position + orientation.right.normalized * wallCheckDistance);
            Gizmos.DrawLine(orientation.position, orientation.position + -orientation.right.normalized * wallCheckDistance);
            Gizmos.DrawLine(orientation.position, orientation.position + (orientation.right + orientation.forward).normalized * wallCheckDistance * 1.5f);
            Gizmos.DrawLine(orientation.position, orientation.position + (-orientation.right + orientation.forward).normalized * wallCheckDistance * 1.5f);
            Gizmos.DrawLine(orientation.position, orientation.position + (orientation.right + -orientation.forward).normalized * wallCheckDistance * 1.5f);
            Gizmos.DrawLine(orientation.position, orientation.position + (-orientation.right + -orientation.forward).normalized * wallCheckDistance * 1.5f);
            Gizmos.DrawWireSphere(groundDetector.position, groundDetectorRadius);
        }
    }
}

