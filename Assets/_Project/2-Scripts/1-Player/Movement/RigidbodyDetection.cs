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
            if (Physics.Raycast(groundDetector.position, groundDetector.TransformDirection(Vector3.down), out slopeHit,  groundDetectorRadius));
            {
                isOnSlope = slopeHit.normal != Vector3.up;
            }
        }

        private void WallRunDetection()
        {
            rightWallDetected = Physics.Raycast(orientation.position, orientation.right, out rightWallHit, wallCheckDistance, wallLayer);
            leftWallDetected = Physics.Raycast(orientation.position, -orientation.right, out leftWallHit, wallCheckDistance, wallLayer);
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

