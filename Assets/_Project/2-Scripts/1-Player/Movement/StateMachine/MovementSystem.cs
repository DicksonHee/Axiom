using System;
using System.Numerics;
using Axiom.Player.StateMachine;
using Unity.VisualScripting;
using UnityEngine;
using Axiom.Player.Movement;
using DG.Tweening;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Axiom.Player.StateMachine
{
    [RequireComponent(typeof(RigidbodyDetection), typeof(InputDetection))]
    public class MovementSystem : StateMachine
    {
        #region Inspector Variables
        [Header("Detection")]
        public RigidbodyDetection rbInfo;
        public InputDetection inputDetection;
        public CameraLook cameraLook;
        public PlayerAnimation playerAnimation;
        public Transform orientation;
        public Transform cameraPosition;
        
        [Header("Capsule Colliders")]
        public CapsuleCollider _standCC;
        public CapsuleCollider _crouchCC;
        
        [Header("AnimationCurve")] 
        public AnimationCurve accelerationCurve;
        public AnimationCurve decelerationCurve;
        public AnimationCurve gravityCurve;
        public AnimationCurve inAirCurve;
        public AnimationCurve wallRunCurve;
        public AnimationCurve slideCurve;
        public AnimationCurve reverseSlideCurve;

        [Header("Gravity")]
        public float groundGravity = 10f;
        public float inAirGravity = 20f;

        [Header("Speed")]
        public float idleSpeed = 3f;
        public float forwardSpeed = 20f;
        public float backwardSpeed = 15f;
        public float strafeSpeed = 15f;
        public float walkSpeed = 12f;
        public float inAirSpeed = 8f;
        public float crouchSpeed = 8f;
        
        [Header("Jump")]
        public float upJumpForce = 10f;

        [Header("WallRun")]
        public float wallRunJumpUpForce = 10f;
        public float wallRunJumpSideForce = 10f;
        public float wallRunExitTime = 0.5f;
        public float wallRunJumpBufferTime = 0.5f;
        #endregion
        
        #region Public Variables
        public Rigidbody _rb{ get; private set; }
        public Vector3 moveDirection { get; private set; }
        public float currentSpeed{ get; private set; }
        public float currentTargetSpeed{ get; private set; }
        public float currentTargetGravity{ get; private set; }
        public float lrMultiplier{ get; private set; }
        public bool isExitingWallRun{ get; private set; }
        public bool isExitingSlide{ get; private set; }
        #endregion

        private bool _movementEnabled = true;
        
        #region Turning Variables
        private Vector3 _currentFacingTransform;
        private float _turnCheckCounter;
        private float _turnMultiplier;
        private float _turnCheckInterval = 0.5f;
        #endregion

        #region Gravity Variables
        private float _gravityCounter;
        private bool _isJumping;
        #endregion

        #region Wall Run Variables
        private float _wallRunExitCounter;
        private float _wallRunJumpBufferCounter;
        private Vector3 _wallRunNormal;
        private Vector3 _wallRunExitPosition;
        #endregion
        
        #region Crouch/Slide Variables
        private float _initialCameraHeight = 2.2f;
        private float _crouchCameraHeight = 1f;
        private float _slideExitCounter;
        #endregion
        
        #region States
        public Idle _idleState { get; private set; }
        public Walking _walkingState { get; private set; }
        public Running _runningState { get; private set; }
        public BackRunning _backRunningState { get; private set; }
        public Strafing _strafingState { get; private set; }
        public InAir _inAirState { get; private set; }
        public WallRunning _wallRunningState { get; private set; }
        public Climbing _climbingState { get; private set; }
        public Sliding _slidingState { get; private set; }
        public Crouching _crouchingState { get; private set; }
        public LedgeClimbing _ledgeClimbingState { get; private set; }
        public LedgeGrabbing _ledgeGrabbingState { get; private set; }
        public Vaulting _vaultingState { get; private set; }
        #endregion

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();

            _idleState = new Idle(this);
            _walkingState = new Walking(this);
            _runningState = new Running(this);
            _backRunningState = new BackRunning(this);
            _strafingState = new Strafing(this);
            _inAirState = new InAir(this);
            _wallRunningState = new WallRunning(this);
            _climbingState = new Climbing(this);
            _slidingState = new Sliding(this);
            _crouchingState = new Crouching(this);
            _ledgeClimbingState = new LedgeClimbing(this);
            _ledgeGrabbingState = new LedgeGrabbing(this);
            _vaultingState = new Vaulting(this);

            InitializeState(_idleState);

            inputDetection.OnJumpPressed += DelegateJump;
            rbInfo.OnPlayerLanded += Landed;
            //InvokeRepeating(nameof(DrawLine), 0f, 0.01f);
        }

        private void Update()
        {
            CheckChangeToAirState();
            CheckIsTurning();
            CheckWallRunTimers();
            CheckSlideTimers();

            CalculateMoveDirection();
            HandleAnimations();
            
            CurrentState.LogicUpdate();
        }

        private void FixedUpdate()
        {
            ApplyMovement();
            ApplyGravity();

            CurrentState.PhysicsUpdate();
        }

        #region Update Functions
        // Calculate moveDirection based on the current input
        private void CalculateMoveDirection()
        {
            float wallJumpMultiplier = _wallRunExitCounter > 0f ? 0f : 1f;
            moveDirection = orientation.forward * inputDetection.movementInput.z + orientation.right * (inputDetection.movementInput.x * wallJumpMultiplier * lrMultiplier);
            CheckSlopeMovementDirection();
        }
        
        private void CheckSlopeMovementDirection()
        {
            if (rbInfo.isOnSlope)
            {
                moveDirection = Vector3.ProjectOnPlane(moveDirection, rbInfo.slopeHit.normal);
            }
        }
        
        public Vector3 CheckSlopeMovementDirection(Vector3 direction)
        {
            if (rbInfo.isOnSlope)
            {
                var slopeRotation = Quaternion.FromToRotation(orientation.up, rbInfo.slopeHit.normal);
                var adjustedVel = slopeRotation * direction;
    
                if (adjustedVel.y <= -0.1f || adjustedVel.y >= 0.1f)
                {
                    return adjustedVel;
                }
            }

            return direction;
        }
        
        // Calculate the current movement speed by evaluating from the curve
        public void CalculateMovementSpeed(AnimationCurve curve, float prevSpeed, float time)
        {
            float velDiff = prevSpeed - currentTargetSpeed;
            currentSpeed = prevSpeed - velDiff * curve.Evaluate(time);
        }

        // Checks if the player is turning and sets the turn multiplier
        // If facing a certain direction for _turnCheckInterval amount of time
        // Set new _currentFacingTransform to forward vector
        // Set _turnMultiplier to the Dot product of the _currentVector and _currentFacingTransform, clamped from 0.5f, 1f
        private void CheckIsTurning()
        {
            _turnMultiplier = Mathf.Clamp(Vector3.Dot(_currentFacingTransform, orientation.TransformDirection(Vector3.forward)), 0.5f, 1f);
            if (Mathf.Abs(cameraLook.mouseX) < 1f) _turnCheckCounter += Time.deltaTime;
            if (_turnCheckCounter > _turnCheckInterval)
            {
                _turnCheckCounter = 0f;
                _currentFacingTransform = orientation.TransformDirection(Vector3.forward);
            }
        }
        
        // Decrements wall run timers
        private void CheckWallRunTimers()
        {
            _wallRunExitCounter -= Time.deltaTime;
            _wallRunJumpBufferCounter -= Time.deltaTime;
            if (_wallRunExitCounter <= 0) isExitingWallRun = false;
        }

        private void CheckSlideTimers()
        {
            _slideExitCounter -= Time.deltaTime;
            if (_slideExitCounter <= 0) isExitingSlide = false;
        }

        private void CheckChangeToAirState()
        {
            if(!rbInfo.isGrounded && 
               CurrentState.stateName != StateName.InAir && 
               CurrentState.stateName != StateName.WallRunning &&
               CurrentState.stateName != StateName.Crouching &&
               CurrentState.stateName != StateName.Sliding) ChangeState(_inAirState);
        }
        #endregion
        
        #region FixedUpdate Functions
        // Apply movement to the character
        private void ApplyMovement()
        {
            if (!_movementEnabled || _wallRunExitCounter > 0f) return;

            Vector3 moveVel = moveDirection.normalized * (currentSpeed * _turnMultiplier * Time.deltaTime * 50f);
            moveVel.y = _rb.velocity.y;
            _rb.velocity = moveVel;
        }

        // Apply constant downward force on the character
        private void ApplyGravity()
        {
            _gravityCounter += Time.fixedDeltaTime;
            if (rbInfo.isOnSlope && !_isJumping) currentTargetGravity = 100f;
            else currentTargetGravity = rbInfo.isGrounded ? groundGravity : inAirGravity;
            _rb.AddForce(-transform.up * (currentTargetGravity * gravityCurve.Evaluate(_gravityCounter)));
        }
        #endregion
        
        #region Jump Functions
        
        // Determines which jump to use
        private void DelegateJump()
        {
            if (CurrentState == _wallRunningState || _wallRunJumpBufferCounter > 0f) WallRunJump();
            else if (rbInfo.isGrounded) Jump();
        }
        
        // Applies upwards force to the character
        private void Jump()
        {
            _isJumping = true;
            if (rbInfo.leftWallDetected && inputDetection.movementInput.x < 0)
            {
                playerAnimation.SetJumpParam(-1f);
                ChangeState(_wallRunningState);
            }
            else if (rbInfo.rightWallDetected && inputDetection.movementInput.x > 0)
            {
                playerAnimation.SetJumpParam(1f);
                ChangeState(_wallRunningState);
            }
            else if (!rbInfo.isGrounded && CurrentState.stateName != StateName.InAir)
            {
                playerAnimation.SetJumpParam(0);
                ChangeState(_inAirState);
            }

            Vector3 velocity = _rb.velocity;
            float jumpMultiplier = Mathf.Clamp(velocity.magnitude / forwardSpeed, 0.75f, 1f);
            _rb.AddForce(new Vector3(0f, upJumpForce * jumpMultiplier, 0f), ForceMode.Impulse);

            playerAnimation.ResetTrigger("Landed");
            playerAnimation.SetTrigger("Jump");
        }
        
        // Applies upwards and sideways force to the character
        private void WallRunJump()
        {
            CurrentState.ExitState();
            playerAnimation.SetJumpParam(rbInfo.leftWallDetected ? -1 : 1);
            playerAnimation.ResetTrigger("Landed");
            playerAnimation.SetTrigger("WallJump");
            
            Vector3 jumpVector = transform.up * wallRunJumpUpForce + orientation.forward * wallRunJumpSideForce;
            _rb.AddForce(jumpVector, ForceMode.Impulse);
            
            _wallRunJumpBufferCounter = 0f;
        }
        
        private void Landed()
        {
            SetGravity(groundGravity);
            _isJumping = false;
            playerAnimation.ResetTrigger("WallJump");
            playerAnimation.ResetTrigger("Jump");
            playerAnimation.SetTrigger("Landed");
        }
        #endregion
        
        #region Crouch Functions
        public void StartCrouch()
        {
            _crouchCC.enabled = true;
            _standCC.enabled = false;
            //cameraPosition.DOLocalMoveY(_crouchCameraHeight, 0.5f);
        }

        public void EndCrouch()
        {
            _standCC.enabled = true;
            _crouchCC.enabled = false;
            //cameraPosition.DOLocalMoveY(_initialCameraHeight, 0.5f);
        }
        #endregion
        
        #region Set Functions
        // Sets the gravity amount
        public void SetGravity(float gravityVal)
        {
            _gravityCounter = 0f;
            currentTargetGravity = gravityVal;
        }
        
        // Called when exiting the wall run state
        public void ExitWallRunState(Vector3 normal)
        {
            _wallRunExitCounter = wallRunExitTime;
            _wallRunJumpBufferCounter = wallRunJumpBufferTime;
            isExitingWallRun = true;
            _wallRunNormal = normal;
        }

        public void ExitSlideState()
        {
            _slideExitCounter = 0.5f;
            isExitingSlide = true;
        }
        
        // Sets the target speed
        public void SetTargetSpeed(float speedVal) => currentTargetSpeed = speedVal;
        // Set left and right movement multiplier
        public void SetLRMultiplier(float multiplier) => lrMultiplier = multiplier;
        // Enables movement
        public void EnableMovement() => _movementEnabled = true;
        // Disables movement
        public void DisableMovement() => _movementEnabled = false;
        #endregion
        
        #region Animation Functions

        private void HandleAnimations()
        {
            playerAnimation.SetRotationDir(cameraLook.mouseX);
            playerAnimation.SetMovementDir(inputDetection.movementInput.normalized);
        }

        public void SetAnimatorBool(string param, bool val) => playerAnimation.SetBool(param, val);
        #endregion
        
        #region Debug Functions
        private void DrawLine()
        {
            Debug.DrawLine(rbInfo.groundDetector.position, rbInfo.groundDetector.position + new Vector3(0,5,0), Color.red, 99f);
        }
        public string GetCurrentStateName() => CurrentState.stateName.ToString();
        public string GetPreviousStatename() => PreviousState.ToString();
        public float GetCurrentSpeed() => _rb.velocity.magnitude;

        #endregion
    }
}
