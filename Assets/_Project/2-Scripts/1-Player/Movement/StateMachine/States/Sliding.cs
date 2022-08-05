using UnityEngine;

namespace Axiom.Player.Movement.StateMachine.States
{
    public class Sliding : State
    {
        private Vector3 initialDir;
        private float inAirCounter;

        public Sliding(MovementSystem movementSystem) : base(movementSystem)
        {
            stateName = StateName.Sliding;
        }

        public override void EnterState()
        {
            base.EnterState();
            
            initialDir = MovementSystem.moveDirection;

            MovementSystem.rbInfo.OnSlopeEnded += ResetStateTimer;

            MovementSystem.DisableMovement();
            MovementSystem.SetTargetSpeed(0f);
            MovementSystem.SetLRMultiplier(0.1f);
            MovementSystem.StartCrouch();

            MovementSystem.cameraLook.StartSlideCamera();
            MovementSystem.SetAnimatorBool("Sliding", true);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if ((MovementSystem.GetCurrentSpeed() < 0.5f && Time.time - stateStartTime > 0.5f) ||
                Vector3.Dot(MovementSystem.forwardDirection, initialDir) < 0.8f) // If too slow or looking away from slide direction
            {
                CheckShouldCrouchOnExit();
            }
            else if (Vector3.Dot(MovementSystem.rb.velocity, MovementSystem.upDirection) > 0.1f) // If sliding up
            {
                CheckShouldCrouchOnExit();
            }
            else if (!MovementSystem.inputDetection.crouchInput) CheckShouldCrouchOnExit();
            else if (MovementSystem.inputDetection.movementInput.z <= 0) CheckShouldCrouchOnExit();// If letting go of crouch key
            else if (inAirCounter > 0.8f) MovementSystem.ChangeState(MovementSystem._inAirState); // If in air
 
            CalculateInAirTime();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            
            CalculateSlideSpeed();
        }

        public override void ExitState()
        {
            base.ExitState();

            MovementSystem.rbInfo.OnSlopeEnded -= ResetStateTimer;

            MovementSystem.EndCrouch();
            MovementSystem.SetLRMultiplier(1f);
            MovementSystem.EnableMovement();

            MovementSystem.cameraLook.ResetCamera();
            MovementSystem.playerAnimation.ResetRotation();
            MovementSystem.SetAnimatorBool("Sliding", false);
        }

        private void CalculateSlideSpeed()
        {
            float targetSpeed = 0;
            if (MovementSystem.rbInfo.IsOnSlope() && 
                Vector3.Dot(MovementSystem.rb.velocity, MovementSystem.orientation.up) < 0.1f)
            {
                targetSpeed = MovementSystem.forwardSpeed * 2;
            }
            float currentSpeed = Mathf.Lerp(MovementSystem.forwardSpeed, targetSpeed, (Time.time - stateStartTime) * 0.5f);
            MovementSystem.rb.AddForce(initialDir.normalized * currentSpeed, ForceMode.Acceleration);
        }

        private void ResetStateTimer()
        {
            stateStartTime = Time.time;
        }
        
        private void CalculateInAirTime()
        {
            if (!MovementSystem.rbInfo.IsGrounded()) inAirCounter += Time.deltaTime;
            else inAirCounter = 0f;
        }
        
        private void CheckShouldCrouchOnExit()
        {
            if(MovementSystem.inputDetection.crouchInput) MovementSystem.ChangeState(MovementSystem._crouchingState);
            else
            {
                if(MovementSystem.rbInfo.CanUncrouch()) MovementSystem.ChangeState(MovementSystem._idleState);
                else MovementSystem.ChangeState(MovementSystem._crouchingState);
            }
        }
    }
}