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
        
        [Header("Drag")]
        public float groundedDrag = 1f;
        public float stoppingDrag = 5f;

        [Header("Gravity")]
        public float groundGravity = -10f;
        public float inAirGravity = -20f;
        public float wallrunGravity = -5f;
        
        [Header("Speed")]
        public float forwardSpeed = 20f;
        public float backwardSpeed = 15f;
        public float strafeSpeed = 15f;
        public float walkSpeed = 12f;
        public float inAirSpeed = 8f;
        
        [Header("Jump")]
        public float upJumpForce = 10f;
        #endregion
        
        #region Public Variables
        public Rigidbody _rb{ get; private set; }
        public CapsuleCollider _capsuleCollider { get; private set; }
        public Vector3 moveDirection { get; private set; }

        [HideInInspector] public float currentSpeed;
        [HideInInspector] public float currentTargetSpeed;
        [HideInInspector] public float currentTargetGravity;
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
            InvokeRepeating(nameof(DrawLine), 0f, 0.01f);
        }

        private void Update()
        {
            CurrentState.LogicUpdate();

            CheckIsTurning();
            CalculateMoveDirection();

            if(!rbInfo.isGrounded && CurrentState.stateName != StateName.InAir) ChangeState(_inAirState);
        }

        private void FixedUpdate()
        {
            CurrentState.PhysicsUpdate();

            ApplyMovement();
            ApplyGravity();
        }

        #region Update Functions
        // Calculate moveDirection based on the current input
        private void CalculateMoveDirection()
        {
            moveDirection = orientation.forward * inputDetection.movementInput.z + orientation.right * inputDetection.movementInput.x;
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
        #endregion
        
        #region FixedUpdate Functions
        // Apply movement to the character
        private void ApplyMovement()
        {
            Vector3 moveVel = moveDirection.normalized * (currentSpeed * _turnMultiplier);
            moveVel.y = _rb.velocity.y;
            _rb.velocity = moveVel;
        }

        // Apply constant downward force on the character
        private void ApplyGravity()
        {
            _gravityCounter += Time.fixedDeltaTime;
            _rb.AddForce(Vector3.down * (currentTargetGravity * gravityCurve.Evaluate(_gravityCounter)));
        }

        // Applies upwards force to the character
        private void Jump()
        {
            if (!rbInfo.isGrounded) return;
            Vector3 velocity = _rb.velocity;
            float jumpMultiplier = Mathf.Clamp(currentSpeed / forwardSpeed, 0.75f, 1f);

            _rb.velocity = new Vector3(velocity.x, 0f, velocity.z);
            _rb.AddForce(new Vector3(0f, upJumpForce * jumpMultiplier, 0f), ForceMode.VelocityChange);
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
        #endregion
        
        #region Set Functions
        // Sets Rigidbody drag
        public void SetDrag(float drag)
        {
            _rb.drag = drag;
        }

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
