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

        [Range(0.1f, 2f)]public float groundDetectorRadius = 0.5f;
        public Transform groundDetector;
        public LayerMask groundLayer;
        
        private Rigidbody _rb;
        private CapsuleCollider _collider;

        private bool isGrounded;
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _collider = GetComponent<CapsuleCollider>();
        }

        private void Update()
        {
            GroundDetection();   
        }

        private void GroundDetection()
        {
            isGrounded = Physics.SphereCast(groundDetector.position, groundDetectorRadius, Vector3.down, out _, groundDetectorRadius, groundLayer);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(groundDetector.position, groundDetectorRadius);
        }
    }
}

