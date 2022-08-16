using Axiom.Player.Movement.StateMachine.States;
using Axiom.Core;
using UnityEngine;
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
        public MoveCamera moveCamera;
        public Transform orientation;

        [Header("VFX")] 
        public MovementVFX movementVFX;

        [Header("Capsule Colliders")]
        public CapsuleCollider _standCC;
        public CapsuleCollider _crouchCC;
        
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
        #endregion
        
        #region Public Variables
        public Rigidbody rb{ get; private set; }
        public Vector3 moveDirection { get; private set; }
        public float currentSpeed{ get; private set; }
        public float currentTargetSpeed{ get; private set; }
        public float currentTargetGravity { get; private set; }
        public float lrMultiplier{ get; private set; }
        public bool isExitingWallRun{ get; private set; }
        public bool isExitingClimb { get; private set; }
        public bool isExitingLedgeGrab { get; private set; }
        #endregion

        private bool _movementEnabled = true;
        #region Turning Variables
        private Vector3 _currentFacingTransform;
        private float _turnCheckCounter;
        private float _turnCheckInterval = 0.5f;
        #endregion

        #region Gravity Variables
        private float _gravityCounter;
        private bool _isJumping;
        #endregion

        #region Wall Run Variables
        private float _wallRunExitCounter;
        private bool _isExitingRightWall;
        private Vector3 _wallRunNormal;
        private Vector3 _wallRunExitPosition;
        private Transform previousWall;
        #endregion

        #region VFX Variables
        private bool isSpeedLinesShowing;
        #endregion
        
        private float _ledgeGrabExitCounter;

        public float totalAirTime;
        public Vector3 upDirection { get; private set; }
        public Vector3 forwardDirection { get; private set; }
        public Vector3 rightDirection { get; private set; }
        
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
        public Landing _landingState { get; private set; }
        #endregion

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();

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
            _landingState = new Landing(this);

            InitializeState(_idleState);

            lrMultiplier = 1;
            inputDetection.OnJumpPressed += DelegateJump;
            rbInfo.OnPlayerLanded += Landed;
            //InvokeRepeating(nameof(DrawLine), 0f, 0.01f);
        }

        private void Update()
        {
            upDirection = orientation.up;
            forwardDirection = orientation.forward;
            rightDirection = orientation.right;
            
            CheckChangeToAirState();
            CheckIsTurning();
            CheckWallRunTimers();
            CheckLedgeGrabTimers();

            LandedCheck();
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

            CurrentState.PhysicsUpdate();
        }

        #region Update Functions
        // Calculate moveDirection based on the current input
        private void CalculateMoveDirection()
        {
            moveDirection = forwardDirection * inputDetection.movementInput.z + rightDirection * (inputDetection.movementInput.x * lrMultiplier);
            CheckSlopeMovementDirection();
        }
        
        private void CheckSlopeMovementDirection()
        {
            if (!rbInfo.IsOnSlope()) return;
            moveDirection = Vector3.ProjectOnPlane(moveDirection, rbInfo.GetSlopeHit().normal);
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
            if (Mathf.Abs(cameraLook.mouseX) < 1f) _turnCheckCounter += Time.deltaTime;
            if (_turnCheckCounter > _turnCheckInterval)
            {
                _turnCheckCounter = 0f;
                _currentFacingTransform = forwardDirection;
            }
        }

        // Decrements wall run timers
        private void CheckWallRunTimers()
        {
            _wallRunExitCounter -= Time.deltaTime;
            isExitingWallRun = _wallRunExitCounter >= 0;
        }

        private void CheckLedgeGrabTimers()
        {
            _ledgeGrabExitCounter -= Time.deltaTime;
            isExitingLedgeGrab = _ledgeGrabExitCounter >= 0;
        }

        private void CheckChangeToAirState()
        {
            if(!rbInfo.IsGrounded() && 
               CurrentState != _inAirState && 
               CurrentState != _wallRunningState &&
               CurrentState != _ledgeGrabbingState &&
               CurrentState != _ledgeClimbingState &&
               CurrentState != _climbingState &&
               CurrentState != _crouchingState &&
               CurrentState != _slidingState) ChangeState(_inAirState);
        }
        #endregion
        
        #region FixedUpdate Functions
        private void ApplyMovement()
        {
            if (!_movementEnabled) return;
            
            rb.AddForce(moveDirection.normalized * currentTargetSpeed, ForceMode.Acceleration);
            ApplyCounterMovement();
        }

        private void ApplyCounterMovement()
        {
            Vector3 currentVel = rb.velocity;
            Vector3 rightVel = Vector3.Cross(upDirection, forwardDirection) * Vector3.Dot(currentVel, rightDirection);
            Vector3 forwardVel = Vector3.Cross(rightDirection, upDirection) * Vector3.Dot(currentVel, forwardDirection);
            
            if(moveDirection.x == 0 && rightVel.magnitude > 0) rb.AddForce(-rightVel * 5f, ForceMode.Acceleration);
            if(moveDirection.z == 0 && forwardVel.magnitude > 0) rb.AddForce(-forwardVel * 5f, ForceMode.Acceleration);
        }
        
        public Vector3 ProjectDirectionOnPlane(Vector3 direction, Vector3 normal)
        {
            return (direction - normal * Vector3.Dot(direction, normal)).normalized;
        }
        
        // Apply constant downward force on the character
        private void ApplyGravity()
        {
            if ((rbInfo.IsOnSlope() || rbInfo.IsGrounded()) && !_isJumping) return;
            rb.AddForce(-upDirection * currentTargetGravity, ForceMode.Force);
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

        // Sets the target speed
        public void SetTargetSpeed(float speedVal) => currentTargetSpeed = speedVal;
        // Set left and right movement multiplier
        public void SetLRMultiplier(float multiplier) => lrMultiplier = multiplier;
        // Enables movement
        public void EnableMovement() => _movementEnabled = true;
        // Disables movement
        public void DisableMovement() => _movementEnabled = false;
        #endregion
        
        #region Get Functions

        public Transform GetPreviousWall() => previousWall;
        #endregion
        
        #region Jump Functions
        
        // Determines which jump to use
        private void DelegateJump()
        {
            if (CurrentState == _wallRunningState && !_isJumping)
            {
                WallRunJump();
            }
            else if (CurrentState == _inAirState && !_isJumping)
            {
                InAirJump();
            }
            else if (rbInfo.IsGrounded() && CurrentState != _landingState)
            {
                if (rbInfo.CanVaultOn() || rbInfo.CanVaultOver()) ChangeState(_vaultingState);
                else if(!_isJumping) Jump();
            }
        }
        
        // Applies upwards force to the character
        private void Jump()
        {
            _isJumping = true;
            rb.AddForce(upDirection * upJumpForce, ForceMode.Impulse);

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
            else if (!rbInfo.IsGrounded() && CurrentState != _inAirState)
            {
                playerAnimation.SetJumpParam(0);
                ChangeState(_inAirState);
            }
            
            playerAnimation.SetLandParam(0f);
            playerAnimation.SetInAirParam(0f);
            playerAnimation.SetTrigger("Jump");
        }

        // Applies upwards and sideways force to the character
        private void WallRunJump()
        {
            Vector3 jumpVel = upDirection.normalized * wallRunJumpUpForce + forwardDirection * (Mathf.Clamp(Vector3.Dot(_wallRunNormal, forwardDirection), 0.75f, 1f) * wallRunJumpSideForce);
            _wallRunningState.SetIsJumpingOnExit(true, jumpVel);
            
            playerAnimation.SetInAirParam(_isExitingRightWall ? 1 : -1);
            playerAnimation.SetLandParam(_isExitingRightWall ? 1 : -1);
        }

        private void InAirJump()
        {
            if (PreviousState == _wallRunningState)
            {
                Vector3 jumpVel = upDirection.normalized * wallRunJumpUpForce + forwardDirection.normalized * (Mathf.Clamp(Vector3.Dot(_wallRunNormal, forwardDirection), 0.75f, 1f) * wallRunJumpSideForce);
                _inAirState.WallRunJump(jumpVel);
                playerAnimation.SetInAirParam(_isExitingRightWall ? 1 : -1);
                playerAnimation.SetLandParam(_isExitingRightWall ? 1 : -1);
            }
            else
            {
                Vector3 jumpVel = upDirection * upJumpForce;
                _inAirState.InAirJump(jumpVel);
                playerAnimation.SetInAirParam(0);
                playerAnimation.SetLandParam(0);
            }
            
            
        }

        private void Landed()
        {
            ChangeState(_landingState);
        }

        private void LandedCheck()
        {
            if (rbInfo.IsGrounded())
            {
                _isJumping = false;
                previousWall = null;
                _wallRunExitCounter = 0;
                isExitingClimb = false;
            }
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

        #region Animation Functions
        private void HandleAnimations()
        {
            playerAnimation.SetRotationDir(cameraLook.mouseX);
            playerAnimation.SetMovementDir(inputDetection.movementInput.normalized);
        }

        public void SetAnimatorBool(string param, bool val) => playerAnimation.SetBool(param, val);
        #endregion

        public void TeleportPlayer(Quaternion newRotation)
        {
            cameraLook.TransformForward(newRotation);
            playerAnimation.ForceRotate();
            TransformTargetVelocity();
        }
        
        private void TransformTargetVelocity()
        {
            Vector3 currentVel = rb.velocity;
            Vector3 newMoveDir = orientation.forward * inputDetection.movementInput.z + orientation.right * inputDetection.movementInput.x;
            newMoveDir.y = Vector3.Dot(currentVel, upDirection);
            rb.velocity = newMoveDir.normalized * currentVel.magnitude;
        }
        
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
            return new Vector3(Vector3.Dot(rb.velocity,forwardDirection), 0f, Vector3.Dot(rb.velocity, rightDirection)).magnitude;
        }
        #endregion
    }
}
