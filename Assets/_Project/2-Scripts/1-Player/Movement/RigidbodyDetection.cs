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

        public bool isGrounded { get; private set; }
        
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
            isGrounded = Physics.Raycast(groundDetector.position, Vector3.down, groundDetectorRadius, groundLayer);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(groundDetector.position, groundDetector.position + new Vector3(0,-groundDetectorRadius,0));
        }
    }
}

