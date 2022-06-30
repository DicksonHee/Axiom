using System;
using Axiom.Player.StateMachine;
using Unity.VisualScripting;
using UnityEngine;
using Axiom.Player.Movement;
using DG.Tweening;

namespace Axiom.Player.StateMachine
{
    [RequireComponent(typeof(RigidbodyDetection), typeof(InputDetection))]
    public class MovementSystem : StateMachine
    {
        #region Inspector Variables
        public RigidbodyDetection rbInfo;
        public InputDetection inputDetection;
        public CameraLook cameraLook;
        public Transform orientation;
        public Transform cameraPosition;
        
        [Header("AnimationCurve")] 
        public AnimationCurve accelerationCurve;
        public AnimationCurve decelerationCurve;
        public AnimationCurve gravityCurve;
        public AnimationCurve wallRunCurve;

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
        #endregion
        
        #region Public Variables
        public Rigidbody _rb{ get; private set; }
        public CapsuleCollider _capsuleCollider { get; private set; }
        public Vector3 moveDirection { get; private set; }

        [HideInInspector] public float currentSpeed;
        [HideInInspector] public float currentTargetSpeed;
        [HideInInspector] public float currentTargetGravity;
        [HideInInspector] public bool isExitingWallRun;
        #endregion
        
        #region Turning Variables
        private Vector3 _currentFacingTransform;
        private float _turnCheckCounter;
        private float _turnMultiplier;
        private float _turnCheckInterval = 0.5f;
        #endregion

        #region Gravity Variables
        private float _gravityCounter;
        #endregion

        private float _initialHeight;
        private float _crouchHeight;
        private float _initialCameraHeight = 4f;
        private float _crouchCameraHeight = 2f;

        private bool _movementEnabled = true;
        
        private float wallRunExitCounter;
        private float wallRunJumpBufferCounter;
        private Vector3 wallRunNormal;
        private Vector3 wallRunExitPosition;
        
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
            _capsuleCollider = GetComponent<CapsuleCollider>();
            _initialHeight = _capsuleCollider.height;
            _crouchHeight = _initialHeight * 0.5f;

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

            inputDetection.OnJumpPressed += Jump;
            rbInfo.OnPlayerLanded += Landed;
            InvokeRepeating(nameof(DrawLine), 0f, 0.01f);
        }

        private void Update()
        {
            CheckIsTurning();
            CalculateMoveDirection();
            CheckIsExitingWallRun();

            if(!rbInfo.isGrounded && CurrentState.stateName != StateName.InAir && CurrentState.stateName != StateName.WallRunning) ChangeState(_inAirState);

            CurrentState.LogicUpdate();
        }

        private void FixedUpdate()
        {
            WallRunJump();
            ApplyMovement();
            ApplyGravity();

            CurrentState.PhysicsUpdate();
        }

        #region Update Functions
        // Calculate moveDirection based on the current input
        private void CalculateMoveDirection()
        {
            float wallJumpMultiplier = wallRunExitCounter > 0f ? 0f : 1f;
            moveDirection = orientation.forward * inputDetection.movementInput.z + orientation.right * (inputDetection.movementInput.x * wallJumpMultiplier);
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

        private void CheckIsExitingWallRun()
        {
            wallRunExitCounter -= Time.deltaTime;
            wallRunJumpBufferCounter -= Time.deltaTime;
            if (wallRunExitCounter <= 0) isExitingWallRun = false;
        }
        #endregion
        
        #region FixedUpdate Functions
        // Apply movement to the character
        private void ApplyMovement()
        {
            if (!_movementEnabled) return;

            Vector3 moveVel = moveDirection.normalized * (currentSpeed * _turnMultiplier);
            moveVel.y = _rb.velocity.y;
            _rb.velocity = moveVel;
        }

        // Apply constant downward force on the character
        private void ApplyGravity()
        {
            _gravityCounter += Time.fixedDeltaTime;
            _rb.AddForce(-transform.up * (currentTargetGravity * gravityCurve.Evaluate(_gravityCounter)));
        }

        // Applies upwards force to the character
        public void Jump()
        {
            if (!rbInfo.isGrounded) return;
			
			Vector3 velocity = _rb.velocity;
			float jumpMultiplier = Mathf.Clamp(velocity.magnitude / forwardSpeed, 0.75f, 1f);
			_rb.AddForce(new Vector3(0f, upJumpForce * jumpMultiplier, 0f), ForceMode.VelocityChange);
			if (!rbInfo.isGrounded && CurrentState.stateName != StateName.InAir) ChangeState(_inAirState);
		}
        
        private void WallRunJump()
        {
            if (wallRunExitCounter <= 0f || rbInfo.isGrounded) return;

            SetIsExitingWallRun();
            Vector3 jumpVector = transform.up * wallRunJumpUpForce + wallRunNormal * wallRunJumpSideForce;
            Vector3 rbVel = _rb.velocity;

            _rb.velocity = Vector3.Lerp(wallRunExitPosition, jumpVector, accelerationCurve.Evaluate(wallRunExitCounter/wallRunExitTime));
            
            //_rb.velocity = new Vector3(rbVel.x, 0, rbVel.z);
            //_rb.AddForce(jumpVector, ForceMode.VelocityChange);
            wallRunJumpBufferCounter = 0f;
            ChangeState(_inAirState);
        }

		public void StartCrouch()
        {
            //_capsuleCollider.height = _crouchHeight;
            cameraPosition.DOLocalMoveY(_crouchCameraHeight, 0.5f);
        }

        public void EndCrouch()
        {
            //_capsuleCollider.height = _initialHeight;
            cameraPosition.DOLocalMoveY(_initialCameraHeight, 0.5f);
        }

        private void Landed()
        {
            SetGravity(groundGravity);
        }
        #endregion
        
        #region Set Functions
        // Sets the gravity amount
        public void SetGravity(float gravityVal)
        {
            _gravityCounter = 0f;
            currentTargetGravity = gravityVal;
        }

        // Sets the target speed
        public void SetTargetSpeed(float speedVal)
        {
            currentTargetSpeed = speedVal;
        }

        public void EnableMovement() => _movementEnabled = true;
        public void DisableMovement() => _movementEnabled = false;

        public void SetIsExitingWallRun()
        {
            wallRunExitCounter = wallRunExitTime;
            isExitingWallRun = true;
        }

        public void ExitWallRunState(Vector3 normal)
        {
            wallRunJumpBufferCounter = 0.5f;
            wallRunNormal = normal;
        }
        #endregion
        
        #region Debug Functions
        private void DrawLine()
        {
            Debug.DrawLine(rbInfo.groundDetector.position, rbInfo.groundDetector.position + new Vector3(0,5,0), Color.red, 99f);
        }
        public string GetCurrentStateName() => CurrentState.stateName.ToString();
        public string GetPreviousStatename() => PreviousState.ToString();
        public float GetCurrentSpeed() => _rb.velocity.magnitude * _turnMultiplier;
        #endregion
    }
}
