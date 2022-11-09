using System;
using Axiom.Player.Movement.StateMachine.States;
using Axiom.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vector3 = UnityEngine.Vector3;

namespace Axiom.Player.Movement.StateMachine
{
    [RequireComponent(typeof(RigidbodyDetection), typeof(InputDetection), typeof(CameraLook))]
    [RequireComponent(typeof(MovementVFX))]
    public class MovementSystem : StateMachine
    {
        #region Inspector Variables
        [Header("Detection")]
        public RigidbodyDetection rbInfo;
        public InputDetection inputDetection;
        public CameraLook cameraLook;
        public PlayerAnimation playerAnimation;
        public Transform orientation;
        
        [Header("Awake Bools")]
        public bool enableMovementOnAwake;
        public bool enableCameraOnAwake;

        [Header("VFX")] 
        public MovementVFX movementVFX;

        [Header("Capsule Colliders")]
        public CapsuleCollider standingCollider;
        public CapsuleCollider crouchingCollider;
        
        [Header("AnimationCurve")] 
        public AnimationCurve accelerationCurve;
        public AnimationCurve decelerationCurve;

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
        public float decelerationSpeed = 50f;
        
        [Header("Jump")]
        public float upJumpForce = 10f;
        public float inAirCoyoteTime = 0.15f;
        public float maxJumpBufferTime = 0.2f;

        [Header("WallRun")]
        public float wallRunJumpUpForce = 10f;
        public float wallRunJumpSideForce = 10f;
        public float wallRunExitTime = 0.5f;
        public float wallRunCoyoteTime = 0.5f;
        public float wallRunMaxDuration = 1f;

        [Header("WallClimb")] 
        public float wallClimbMaxDuration = 1f;
        #endregion
        
        #region Public Variables
        public Rigidbody Rb{ get; private set; }
        public Vector3 MoveDirection { get; private set; }
        public float CurrentTargetSpeed{ get; private set; }
        public bool IsExitingWallRun{ get; private set; }
        public bool IsExitingClimb { get; private set; }
        public bool IsExitingLedgeGrab { get; private set; }
        public float TotalAirTime { get; private set; }
        public Vector3 UpDirection { get; private set; }
        public Vector3 ForwardDirection { get; private set; }
        public Vector3 RightDirection { get; private set; }
        public State LastForwardState { get; private set; }
        #endregion

        #region Movement Variables
        private float lrMultiplier;
        private bool movementEnabled = true;
        private bool counterMovementEnabled = true;
        #endregion
        
        #region Gravity Variables
        private float currentTargetGravity;
        #endregion

        #region Jump Variables
        private float coyoteTimeCounter;
        private float jumpBufferCounter;
        #endregion
        
        #region Wall Run/Climb Variables
        private float ledgeGrabExitCounter;
        
        private float wallRunExitCounter;
        private bool isExitingRightWall;
        private Vector3 wallRunNormal;
        private Vector3 wallRunExitPosition;
        private Transform previousWall;
        #endregion

        #region VFX Variables
        private bool isSpeedLinesShowing;
        #endregion

        #region States
        public Idle IdleState { get; private set; }
        public Walking WalkingState { get; private set; }
        public Running RunningState { get; private set; }
        public BackRunning BackRunningState { get; private set; }
        public Strafing StrafingState { get; private set; }
        public InAir InAirState { get; private set; }
        public WallRunning WallRunningState { get; private set; }
        public Climbing ClimbingState { get; private set; }
        public Sliding SlidingState { get; private set; }
        public Crouching CrouchingState { get; private set; }
        public LedgeClimbing LedgeClimbingState { get; private set; }
        public LedgeGrabbing LedgeGrabbingState { get; private set; }
        public Vaulting VaultingState { get; private set; }
        public Landing LandingState { get; private set; }
        #endregion

        #region Events
        public event Action<string> OnStateChanged;
        #endregion

        private void Awake()
        {
            IdleState = new Idle(this);
            WalkingState = new Walking(this);
            RunningState = new Running(this);
            BackRunningState = new BackRunning(this);
            StrafingState = new Strafing(this);
            InAirState = new InAir(this);
            WallRunningState = new WallRunning(this);
            ClimbingState = new Climbing(this);
            SlidingState = new Sliding(this);
            CrouchingState = new Crouching(this);
            LedgeClimbingState = new LedgeClimbing(this);
            LedgeGrabbingState = new LedgeGrabbing(this);
            VaultingState = new Vaulting(this);
            LandingState = new Landing(this);

            Rb = GetComponent<Rigidbody>();
            lrMultiplier = 1;
            LastForwardState = WalkingState;
            
            PlayerMovementDetails.movementInputEnabled = enableMovementOnAwake;
            PlayerMovementDetails.cameraLookEnabled = enableCameraOnAwake;
            
            InitializeState(IdleState);
        }

        public override void ChangeState(State state)
        {
            base.ChangeState(state);

            OnStateChanged?.Invoke(state.stateName.ToString());
        }

        private void OnEnable()
        {
            inputDetection.OnJumpPressed += DelegateJump;
            rbInfo.OnPlayerLanded += Landed;
        }

        private void OnDisable()
        {
            inputDetection.OnJumpPressed -= DelegateJump;
            rbInfo.OnPlayerLanded -= Landed;
        }

        private void Update()
        {


            UpDirection = orientation.up;
            ForwardDirection = orientation.forward;
            RightDirection = orientation.right;
            
            CheckChangeToAirState();
            CheckCoyoteTimer();
            CheckJumpBufferTimer();
            CheckWallRunTimers();
            CheckLedgeGrabTimers();
            
            CalculateMoveDirection();
            HandleAnimations();
            HandleVFX();
            
            rbInfo.SetCurrentVelocity(GetCurrentSpeed());

            CurrentState.LogicUpdate();
        }

        private void FixedUpdate()
        {
            ApplyGravity();
            ApplyMovement();

            CheckIfShouldJump();

            CurrentState.PhysicsUpdate();
        }

        #region Update Functions
        // Calculate moveDirection based on the current input
        private void CalculateMoveDirection()
        {
            MoveDirection = ForwardDirection * inputDetection.movementInput.z + RightDirection * (inputDetection.movementInput.x * lrMultiplier);
            CheckSlopeMovementDirection();
        }
        
        // Calculate new moveDirection if player is on slope
        private void CheckSlopeMovementDirection()
        {
            if (!rbInfo.IsOnSlope()) return;
            MoveDirection = Vector3.ProjectOnPlane(MoveDirection, rbInfo.GetSlopeHit().normal);
        }

        // Check if player is on ground and decrements jump counter if not on ground
        private void CheckCoyoteTimer()
        {
            float timeDelta = rbInfo.IsGrounded() || CurrentState == WallRunningState ? Time.deltaTime : -Time.deltaTime;
            coyoteTimeCounter = Mathf.Clamp(coyoteTimeCounter + timeDelta, 0, inAirCoyoteTime);
        }
        
        private void CheckJumpBufferTimer()
        {
            jumpBufferCounter = Mathf.Clamp(jumpBufferCounter - Time.deltaTime, 0, maxJumpBufferTime);
        }

        // Decrements wall run timers
        private void CheckWallRunTimers()
        {
            wallRunExitCounter -= Time.deltaTime;
            IsExitingWallRun = wallRunExitCounter >= 0;
        }

        // Decrements ledge grab timers
        private void CheckLedgeGrabTimers()
        {
            ledgeGrabExitCounter -= Time.deltaTime;
            IsExitingLedgeGrab = ledgeGrabExitCounter >= 0;
        }

        // Check if should change to in air state
        private void CheckChangeToAirState()
        {
            if(!rbInfo.IsGrounded() && 
               coyoteTimeCounter <= 0 &&
               CurrentState != InAirState && 
               CurrentState != WallRunningState &&
               CurrentState != LedgeGrabbingState &&
               CurrentState != LedgeClimbingState &&
               CurrentState != ClimbingState &&
               CurrentState != CrouchingState &&
               CurrentState != SlidingState) ChangeState(InAirState);
        }

        private void CheckIfShouldJump()
        {
            if (CurrentState != LandingState && CurrentState != InAirState && rbInfo.CanUncrouch() && 
                jumpBufferCounter > 0f)
            {
                coyoteTimeCounter = -1f;
                jumpBufferCounter = -1f;
                Jump();
            }
        }
        #endregion

        #region FixedUpdate Functions

        // Apply movement to the player using AddForce(Acceleration)
        private void ApplyMovement()
        {
            if (!movementEnabled) return;
            
            Rb.AddForce(MoveDirection.normalized * CurrentTargetSpeed, ForceMode.Acceleration);
            ApplyCounterMovement();
        }

        // Applies counter movement to reduce slippery movement
        private void ApplyCounterMovement()
        {
            if (!counterMovementEnabled) return;
            
            Vector3 currentVel = Rb.velocity;
            Vector3 rightVel = Vector3.Cross(UpDirection, ForwardDirection) * Vector3.Dot(currentVel, RightDirection);
            Vector3 forwardVel = Vector3.Cross(RightDirection, UpDirection) * Vector3.Dot(currentVel, ForwardDirection);

            if (inputDetection.movementInput.x == 0f && rightVel.magnitude > 0.1f) Rb.AddForce(-rightVel * decelerationSpeed, ForceMode.Acceleration);
            if (inputDetection.movementInput.z == 0f && forwardVel.magnitude > 0.1f) Rb.AddForce(-forwardVel * decelerationSpeed, ForceMode.Acceleration);
        }

        // Apply constant downward force on the character
        private void ApplyGravity()
        {
            if (rbInfo.IsGrounded()) return;
            Rb.AddForce(/*-UpDirection*/Physics.gravity * currentTargetGravity, ForceMode.Force);
        }
        #endregion
        
        #region Set Functions
        // Sets the gravity amount
        public void SetGravity(float gravityVal) => currentTargetGravity = gravityVal;
        // Sets the target speed
        public void SetTargetSpeed(float speedVal) => CurrentTargetSpeed = speedVal;
        // Set left and right movement multiplier
        public void SetLRMultiplier(float multiplier) => lrMultiplier = multiplier;
        public void SetTotalAirTime(float duration) => TotalAirTime = duration;
        public void SetLastForwardState(State state) => LastForwardState = state;
        public void EnterClimbState() => IsExitingClimb = false;
        public void ExitClimbState() => IsExitingClimb = true;
        // Enables movement
        public void EnableMovement() => movementEnabled = true;
        // Disables movement
        public void DisableMovement() => movementEnabled = false;
        public void EnableCounterMovement() => counterMovementEnabled = true;
        public void DisableCounterMovement() => counterMovementEnabled = false;
        #endregion
        
        #region Get Functions
        public Transform GetPreviousWall() => previousWall;
        public bool GetIsOnRightWall() => isExitingRightWall;
        #endregion
        
        #region Jump Functions
        
        // Determines which jump to use
        private void DelegateJump()
        {
            jumpBufferCounter = maxJumpBufferTime;

            if (CurrentState == WallRunningState)
            {
                coyoteTimeCounter = -1f;
                WallRunJump();
            }
            else if (rbInfo.CanVaultOn() || rbInfo.CanVaultOver())
            {
                ChangeState(VaultingState);
            }
        }
        
        // Applies upwards force to the character
        private void Jump()
        {
            float forwardForce = 0f;
            if (GetCurrentSpeed() > 16f) forwardForce = (GetCurrentSpeed() - 16f);
            Rb.AddForce(UpDirection * (upJumpForce + forwardForce) + MoveDirection * forwardForce, ForceMode.Impulse);

            if (rbInfo.IsLeftWallDetected() && inputDetection.movementInput.x < 0)
            {
                playerAnimation.SetJumpParam(-1f);
                ChangeState(WallRunningState);
            }
            else if (rbInfo.IsRightWallDetected() && inputDetection.movementInput.x > 0)
            {
                playerAnimation.SetJumpParam(1f);
                ChangeState(WallRunningState);
            }
            else if (!rbInfo.IsGrounded() && CurrentState != InAirState)
            {
                playerAnimation.SetJumpParam(0);
                ChangeState(InAirState);
            }
            
            playerAnimation.SetLandParam(0f);
            playerAnimation.SetInAirParam(0f);
            playerAnimation.SetTrigger("Jump");
        }

        // Applies upwards and sideways force to the character
        private void WallRunJump()
        {
            Vector3 jumpVel = UpDirection.normalized * wallRunJumpUpForce + ForwardDirection * (Mathf.Clamp(Vector3.Dot(wallRunNormal, ForwardDirection), 0.75f, 1f) * wallRunJumpSideForce);
            WallRunningState.SetIsJumpingOnExit(true, jumpVel);
            
            playerAnimation.SetInAirParam(isExitingRightWall ? 1 : -1);
        }
        
        private void Landed()
        {
            previousWall = null;
            wallRunExitCounter = 0;
            IsExitingClimb = false;
            ChangeState(LandingState);
        }

        #endregion
        
        #region Crouch Functions
        public void StartCrouch()
        {
            EnableCollider(crouchingCollider);
        }

        public void EndCrouch()
        {
            EnableCollider(standingCollider);
        }
        #endregion
        
        #region WallRun Functions
        public void EnterWallRunState(Transform wallTransform, Vector3 normal, bool isWallOnRight)
        {
            wallRunNormal = normal;
            isExitingRightWall = isWallOnRight;
            previousWall = wallTransform;
        }
        
        // Called when exiting the wall run state
        public void ExitWallRunState()
        {
            wallRunExitCounter = wallRunExitTime;
            IsExitingWallRun = true;
        }
        #endregion

        #region Capsule Collider Functions

        private void EnableCollider(CapsuleCollider col)
        {
            standingCollider.enabled = standingCollider == col;
            crouchingCollider.enabled = crouchingCollider == col;
        }
        #endregion

        #region Animation Functions
        private void HandleAnimations()
        {
            playerAnimation.SetRotationDir(cameraLook.mouseX);
            playerAnimation.SetMovementDir(inputDetection.movementInput.normalized);
        }

        public void SetAnimatorBool(string param, bool val) => playerAnimation.SetBool(param, val);
        #endregion

        #region Teleport Functions

        public void TeleportPlayer(Vector3 teleportPosition)
        {
            DisableCounterMovement();

            transform.position = teleportPosition;

            Invoke(nameof(EnableCounterMovement), 0.1f);
        }

        // Force rotation
        public void TeleportPlayerRotateTo(Vector3 teleportPosition, Quaternion? forwardRotation, Vector3? gravityDirection)
        {
            DisableCounterMovement();
            Vector3 currentVel = Rb.velocity;
            
            if (forwardRotation != null)
            {
                transform.position = teleportPosition;
                cameraLook.TransformForwardRotateTo(forwardRotation.Value);
                playerAnimation.ForceRotate();
            }

            if (gravityDirection != null)
            {
                Physics.gravity = gravityDirection.Value;
            }
            
            //TransformTargetVelocity(currentVel);
            Invoke(nameof(EnableCounterMovement), 0.1f);
        }

        // Add rotation
		public void TeleportPlayerRotateBy(Vector3 teleportPosition, Quaternion? rotationDiff, Vector3? gravityDirection)
		{
            DisableCounterMovement();
			Vector3 currentVel = Rb.velocity;

            if (gravityDirection != null)
            {
                Physics.gravity = gravityDirection.Value;
                print(gravityDirection.Value);
            }
            
            if (rotationDiff != null)
            {
                transform.position = teleportPosition;
                //cameraLook.TransformForwardRotateBy(rotationDiff.Value);
                print($"before: {transform.rotation.eulerAngles}");
                transform.rotation = transform.rotation * rotationDiff.Value;
                print($"after: {transform.rotation.eulerAngles}");

                playerAnimation.ForceRotate();
                TransformTargetVelocity(currentVel, rotationDiff.Value);
            }

            Invoke(nameof(EnableCounterMovement), 0.1f);
		}

        public void TeleportButForRealYo(Vector3 position, Quaternion rotation, Quaternion rotationDifference)
        {
            transform.position = position;
            transform.rotation = rotation;
            Rb.velocity = rotationDifference * Rb.velocity;
        }

        private void TransformTargetVelocity(Vector3 vel, Quaternion rotationDiff)
        {
            Vector3 newMoveDir = rotationDiff * vel;
            Rb.velocity = newMoveDir;
            //Vector3 newMoveDir = orientation.forward * inputDetection.movementInput.z + orientation.right * inputDetection.movementInput.x;

            ////Rb.velocity = (vel + newMoveDir).normalized * vel.magnitude;
            //Rb.velocity = newMoveDir.normalized * vel.magnitude;
        }
		#endregion

		#region VFX Functions
		private void HandleVFX()
        {
            if (isSpeedLinesShowing && (GetCurrentSpeed() < 5f || inputDetection.movementInput.x != 0))
            {
                isSpeedLinesShowing = false;
                DisableSpeedLines();
                SetVignetteIntensity(0.1f);
            }
            else if (!isSpeedLinesShowing && GetCurrentSpeed() > 15f)
            {
                isSpeedLinesShowing = true;
                EnableSpeedLines();
                SetVignetteIntensity(0.2f);
            }
        }
        
        public void EnableSpeedLines() => movementVFX.SetSpeedLineSpawnRate(100f);
        public void DisableSpeedLines() => movementVFX.SetSpeedLineSpawnRate(0f);

        private void SetVignetteIntensity(float intensity) => PostProcessingActions.current.SetVignetteIntensity(intensity);
        #endregion

        #region Debug Functions
        private void DrawLine()
        {
            Debug.DrawLine(rbInfo.groundDetectorTransform.position, rbInfo.groundDetectorTransform.position + new Vector3(0,5,0), Color.red, 99f);
        }
        public string GetCurrentStateName() => CurrentState.stateName.ToString();
        public string GetPreviousStatename() => PreviousState.stateName.ToString();

        public float GetCurrentSpeed()
        {
            return new Vector3(Vector3.Dot(Rb.velocity,ForwardDirection), 0f, Vector3.Dot(Rb.velocity, RightDirection)).magnitude;
        }
        #endregion
    }
}