using System;
using Axiom.Player.StateMachine;
using Unity.VisualScripting;
using UnityEngine;
using Axiom.Player.Movement;

namespace Axiom.Player.StateMachine
{
    [RequireComponent(typeof(RigidbodyDetection), typeof(InputDetection))]
    public class MovementSystem : StateMachine
    {
        public RigidbodyDetection rbInfo;
        public InputDetection inputDetection;
        public CameraLook cameraLook;
        public Transform orientation;
        
        [Header("AnimationCurve")]
        public AnimationCurve idleToRun;

        [Header("Drag")]
        public float groundedDrag = 1f;
        public float stoppingDrag = 5f;

        [Header("Gravity")]
        public float groundGravity = -10f;
        public float inAirGravity = -20f;
        public float wallrunGravity = -5f;
        
        [Header("Speed")]
        public float forwardSpeed = 10f;
        public float backwardSpeed = 5f;
        public float turningSpeed = 3f;
        public float strafeSpeed = 5f;
        public float inAirSpeed = 8f;
        
        [Header("Jump")]
        public float jumpForce = 10f;
        
        public Rigidbody _rb{ get; private set; }

        [HideInInspector] public float currentTargetSpeed;
        
        public Vector3 _currentVelocity { get; private set; }
        
        public Idle _idleState { get; private set; }
        public Walking _walkingState { get; private set; }
        public Running _runningState { get; private set; }
        public InAir _inAirState { get; private set; }
        public WallRunning _wallRunningState { get; private set; }
        public Climbing _climbingState { get; private set; }
        public Sliding _slidingState { get; private set; }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();

            _idleState = new Idle(this);
            _walkingState = new Walking(this);
            _runningState = new Running(this);
            _inAirState = new InAir(this);
            _wallRunningState = new WallRunning(this);
            _climbingState = new Climbing(this);
            _slidingState = new Sliding(this);

            InitializeState(_idleState);
        }

        private void Update()
        {
            CurrentState.LogicUpdate();

            if(!rbInfo.isGrounded) ChangeState(_inAirState);
            
            UpdateCurrentTargetSpeed();

            Debug.Log(MoveDirection());
        }

        private void FixedUpdate()
        {
            CurrentState.PhysicsUpdate();

            if(rbInfo.isGrounded) ApplyGravity(groundGravity);
        }

        private void UpdateCurrentTargetSpeed()
        {
            Vector3 currentInput = inputDetection.movementInput;
            if (currentInput == Vector3.zero) return;

            if (Mathf.Abs(cameraLook.mouseX) > 0.1f) currentTargetSpeed = 3f;
            else if (!rbInfo.isGrounded) currentTargetSpeed = inAirSpeed;
            else if (currentInput.x > 0 || currentInput.x < 0) currentTargetSpeed = strafeSpeed;
            else if (currentInput.z < 0) currentTargetSpeed = backwardSpeed;
            else if (currentInput.z > 0) currentTargetSpeed = forwardSpeed;
        }

        public Vector3 MoveDirection()
        {
            return orientation.forward * inputDetection.movementInput.z + orientation.right * inputDetection.movementInput.x;
        }
        
        public void MovePlayer(Vector3 vel)
        {
            _rb.AddForce(vel.normalized * currentTargetSpeed);
        }

        public void ApplyGravity(float force)
        {
            _rb.AddRelativeForce(Vector3.down * force);
        }

        public void Jump()
        {
            var velocity = _rb.velocity;
            _rb.velocity = new Vector3(velocity.x, 0f, velocity.z);
            _rb.AddForce(new Vector3(0f, jumpForce, 0f), ForceMode.VelocityChange);
        }

        public void SetDrag(float drag)
        {
            _rb.drag = drag;
        }
        
    }
}
