using System;
using System.Collections;
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
        public float wallRunSpeed = 25f;
        public float wallClimbSpeed = 12f;
        
        [Header("Jump")]
        public float upJumpForce = 10f;
        public float inAirCoyoteTime = 0.15f;

        [Header("WallRun")]
        public float wallRunJumpUpForce = 10f;
        public float wallRunJumpSideForce = 10f;
        public float wallRunExitTime = 0.5f;
        public float wallRunCoyoteTime = 0.5f;
        public float wallRunMaxDuration = 1f;

        [Header("WallClimb")] 
        public float wallClimbMaxDuration = 1f;
        public float exitWallClimbDuration = 0.2f;
        #endregion
        
        #region Public Variables
        public Rigidbody _rb{ get; private set; }
        public Vector3 moveDirection { get; private set; }
        public float currentSpeed{ get; private set; }
        public float currentTargetSpeed{ get; private set; }
        public float currentTargetGravity { get; private set; }
        public float lrMultiplier{ get; private set; }
        public bool isExitingWallRun{ get; private set; }
        public bool isExitingSlide{ get; private set; }
        public bool isExitingClimb { get; private set; }
        public bool isExitingLedgeGrab { get; private set; }
        #endregion

        private bool _movementEnabled = true;
        public float _maxHeight;

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
        private bool _isExitingRightWall;
        public Transform previousWall;
        #endregion
        
        #region Crouch/Slide Variables
        private float _slideExitCounter;
        #endregion

        private float _ledgeGrabExitCounter;
        
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
            CheckLedgeGrabTimers();

            CalculateMoveDirection();
            HandleAnimations();
            
            rbInfo.SetCurrentVelocity(GetCurrentSpeed());

            CurrentState.LogicUpdate();
        }

        private void FixedUpdate()
        {
            ApplyGravity();
            ApplyMovement();

            CurrentState.PhysicsUpdate();
        }

        #region Update Functions
        // Calculate moveDirection based on the current input
        private void CalculateMoveDirection()
        {
            moveDirection = orientation.forward * inputDetection.movementInput.z + orientation.right * (inputDetection.movementInput.x * lrMultiplier);
            CheckSlopeMovementDirection();
        }
        
        private void CheckSlopeMovementDirection()
        {
            if (!rbInfo.isOnSlope) return;
            moveDirection = Vector3.ProjectOnPlane(moveDirection, rbInfo.slopeHit.normal);
        }
        
        public Vector3 CheckSlopeMovementDirection(Vector3 direction)
        {
            if (!rbInfo.isOnSlope) return direction;
            
            var slopeRotation = Quaternion.FromToRotation(orientation.up, rbInfo.slopeHit.normal);
            var adjustedVel = slopeRotation * direction;
            if (adjustedVel.y <= -0.1f || adjustedVel.y >= 0.1f) return adjustedVel;

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
            _turnMultiplier = Mathf.Clamp(Vector3.Dot(_currentFacingTransform, orientation.forward), 0.5f, 1f);
            if (Mathf.Abs(cameraLook.mouseX) < 1f) _turnCheckCounter += Time.deltaTime;
            if (_turnCheckCounter > _turnCheckInterval)
            {
                _turnCheckCounter = 0f;
                _currentFacingTransform = orientation.forward;
            }
        }
        
        // Decrements wall run timers
        private void CheckWallRunTimers()
        {
            _wallRunExitCounter -= Time.deltaTime;
            _wallRunJumpBufferCounter -= Time.deltaTime;
            isExitingWallRun = _wallRunExitCounter >= 0;
        }

        private void CheckSlideTimers()
        {
            _slideExitCounter -= Time.deltaTime;
            isExitingSlide = _slideExitCounter >= 0;
        }

        private void CheckLedgeGrabTimers()
        {
            _ledgeGrabExitCounter -= Time.deltaTime;
            isExitingLedgeGrab = _ledgeGrabExitCounter >= 0;
        }

        private void CheckChangeToAirState()
        {
            if(!rbInfo.isGrounded && 
               CurrentState.stateName != StateName.InAir && 
               CurrentState.stateName != StateName.WallRunning &&
               CurrentState.stateName != StateName.LedgeGrabbing &&
               CurrentState.stateName != StateName.LedgeClimbing &&
               CurrentState.stateName != StateName.Climbing &&
               CurrentState.stateName != StateName.Crouching &&
               CurrentState.stateName != StateName.Sliding) ChangeState(_inAirState);
        }
        #endregion
        
        #region FixedUpdate Functions
        private void ApplyMovement()
        {
            _rb.AddForce(moveDirection * currentTargetSpeed, ForceMode.Acceleration);
        }

        public Vector3 ProjectDirectionOnPlane(Vector3 direction, Vector3 normal)
        {
            return (direction - normal * Vector3.Dot(direction, normal)).normalized;
        }
        
        // Apply constant downward force on the character
        private void ApplyGravity()
        {
            _rb.AddForce(-transform.up * currentTargetGravity, ForceMode.Force);
        }
        #endregion
        
        #region Jump Functions
        
        // Determines which jump to use
        private void DelegateJump()
        {
            if (CurrentState == _wallRunningState)
            {
                WallRunJump();
            }
            else if (CurrentState == _ledgeGrabbingState)
            {
                LedgeGrabJump();
            }
            else if (CurrentState == _inAirState)
            {
                InAirJump();
            }
            else if (rbInfo.isGrounded)
            {
                if (!rbInfo.canVaultOver && !rbInfo.canVaultOn) Jump();
                else ChangeState(_vaultingState);
            }
        }
        
        // Applies upwards force to the character
        private void Jump()
        {
            _isJumping = true;
            _rb.AddForce(transform.up * upJumpForce, ForceMode.Impulse);

            if (rbInfo.IsLeftWallDetected() && inputDetection.movementInput.x < 0)
            {
                playerAnimation.SetJumpParam(-1f);
                ChangeState(_wallRunningState);
            }
            else if (rbInfo.IsRightWallDetected() && inputDetection.movementInput.x > 0)
            {
                playerAnimation.SetJumpParam(1f);
                ChangeState(_wallRunningState);
            }
            else if (!rbInfo.isGrounded && CurrentState.stateName != StateName.InAir)
            {
                playerAnimation.SetJumpParam(0);
                ChangeState(_inAirState);
            }

            playerAnimation.SetLandParam(0f);
            playerAnimation.SetInAirParam(0f);
            playerAnimation.ResetTrigger("Landed");
            playerAnimation.SetTrigger("Jump");
        }

        // Applies upwards and sideways force to the character
        private void WallRunJump()
        {
            Vector3 forward = orientation.forward;
            Vector3 jumpVel = transform.up.normalized * wallRunJumpUpForce + forward.normalized * (Mathf.Clamp(Vector3.Dot(_wallRunNormal, forward), 0.75f, 1f) * wallRunJumpSideForce);
            _wallRunningState.SetIsJumpingOnExit(true, jumpVel);
            
            playerAnimation.ResetTrigger("Landed");
            playerAnimation.SetInAirParam(_isExitingRightWall ? 1 : -1);
            playerAnimation.SetLandParam(_isExitingRightWall ? 1 : -1);
            _wallRunJumpBufferCounter = 0f;
        }

        private void InAirJump()
        {
            Vector3 forward = orientation.forward;
            
            if (_wallRunExitCounter > 0f)
            {
                Vector3 jumpVel = transform.up.normalized * wallRunJumpUpForce + forward.normalized * (Mathf.Clamp(Vector3.Dot(_wallRunNormal, forward), 0.75f, 1f) * wallRunJumpSideForce);
                _inAirState.WallRunJump(jumpVel);
            }
            else
            {
                _rb.AddForce(transform.up * upJumpForce, ForceMode.Impulse);
            }

            playerAnimation.ResetTrigger("Landed");
            playerAnimation.SetInAirParam(_isExitingRightWall ? 1 : -1);
            playerAnimation.SetLandParam(_isExitingRightWall ? 1 : -1);
            _wallRunJumpBufferCounter = 0f;
        }

        private void LedgeGrabJump()
        {
            Vector3 jumpVel = transform.up * wallRunJumpUpForce + orientation.forward.normalized * wallRunJumpSideForce;
            _ledgeGrabbingState.SetIsJumpingOnExit(true, jumpVel);

            playerAnimation.ResetTrigger("Landed");
            playerAnimation.SetInAirParam(0f);
            playerAnimation.SetLandParam(0f);
            _wallRunJumpBufferCounter = 0f;
        }
        private void Landed()
        {
            SetGravity(groundGravity);
            
            _isJumping = false;
            previousWall = null;
            _wallRunExitCounter = 0;
            isExitingClimb = false;

            playerAnimation.ResetTrigger("WallJump");
            playerAnimation.ResetTrigger("Jump");

            playerAnimation.SetInAirParam(0);
            playerAnimation.SetTrigger("Landed");
            
            ResetMaxHeight();
        }
        #endregion
        
        #region Crouch Functions
        public void StartCrouch()
        {
            EnableCollider(_crouchCC);
        }

        public void EndCrouch()
        {
            EnableCollider(_standCC);
        }
        #endregion
        
        #region WallRun Functions
        public void EnterWallRunState(Transform wallTransform, Vector3 normal, bool isWallOnRight)
        {
            _wallRunNormal = normal;
            _isExitingRightWall = isWallOnRight;
            previousWall = wallTransform;
        }
        
        // Called when exiting the wall run state
        public void ExitWallRunState()
        {
            _wallRunExitCounter = wallRunExitTime;
            _wallRunJumpBufferCounter = wallRunCoyoteTime;
            isExitingWallRun = true;
        }
        #endregion
        
        #region Capsule Collider Functions

        private void EnableCollider(CapsuleCollider col)
        {
            _standCC.enabled = _standCC == col;
            _crouchCC.enabled = _crouchCC == col;
        }
        #endregion
        
        #region Set Functions
        // Sets the gravity amount
        public void SetGravity(float gravityVal)
        {
            _gravityCounter = 0f;
            currentTargetGravity = gravityVal;
        }

        public void EnterClimbState()
        {
            isExitingClimb = false;
        }

        public void ExitClimbState()
        {
            isExitingClimb = true;
        }

        public void ExitLedgeGrabState()
        {
            _ledgeGrabExitCounter = 0.3f;
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

        public void ResetMaxHeight() => _maxHeight = 0f;

        public void SetMaxHeight(float val)
        {
            if (_maxHeight < val) _maxHeight = val;
        }
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

        public float GetCurrentSpeed()
        {
            return new Vector3(Vector3.Dot(_rb.velocity,orientation.forward), 0f, Vector3.Dot(_rb.velocity, orientation.right)).magnitude;
        }
        #endregion
    }
}
