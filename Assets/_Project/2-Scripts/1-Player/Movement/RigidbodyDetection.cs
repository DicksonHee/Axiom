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
        public bool canRotate;

        [Range(0.1f, 5f)]public float groundDetectorRadius = 0.5f;
        public Transform groundDetector;
        public LayerMask groundLayer;
        
        private Rigidbody _rb;
        private CapsuleCollider _collider;

        public bool isGrounded { get; private set; }
        public bool isOnSlope { get; private set; }
        public RaycastHit slopeHit;
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _collider = GetComponent<CapsuleCollider>();
        }

        private void Update()
        {
            GroundDetection();
            SlopeDetection();
        }

        private void GroundDetection()
        {
            isGrounded = Physics.CheckSphere(groundDetector.position, groundDetectorRadius, groundLayer);
        }
        
        private void SlopeDetection()
        {
            if (Physics.Raycast(groundDetector.position, groundDetector.TransformDirection(Vector3.down), out slopeHit,  groundDetectorRadius));
            {
                isOnSlope = slopeHit.normal != Vector3.up;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundDetector.position, groundDetectorRadius);
        }
    }
}

