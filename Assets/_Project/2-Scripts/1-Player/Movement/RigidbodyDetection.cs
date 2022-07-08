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
        public float wallCheckDistance = 0.5f;
        public LayerMask wallLayer;

        private Rigidbody _rb;
        private CapsuleCollider _collider;

        public RaycastHit slopeHit;
        public RaycastHit rightWallHit;
        public RaycastHit leftWallHit;

        public bool isGrounded { get; private set; }
        public bool isOnSlope { get; private set; }
        public bool leftWallDetected {get; private set; }
        public bool rightWallDetected { get; private set; }

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
            rightWallDetected = Physics.Raycast(position, right, out rightWallHit, wallCheckDistance, wallLayer);
            leftWallDetected = Physics.Raycast(position, -right, out leftWallHit, wallCheckDistance, wallLayer);
            //Debug.Log(leftWallDetected);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(orientation.position, orientation.position + orientation.right * wallCheckDistance);
            Gizmos.DrawLine(orientation.position, orientation.position + -orientation.right * wallCheckDistance);
            Gizmos.DrawWireSphere(groundDetector.position, groundDetectorRadius);
        }
    }
}

