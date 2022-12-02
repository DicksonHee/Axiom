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
            
            initialDir = MovementSystem.MoveDirection;

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

            CalculateInAirTime();
             
            if ((MovementSystem.GetCurrentSpeed() < 0.5f && Time.time - stateStartTime > 0.5f) ||
                Vector3.Dot(MovementSystem.ForwardDirection, initialDir) < 0.8f) // If too slow or looking away from slide direction
            {
                CheckShouldCrouchOnExit();
            }
            else if (Vector3.Dot(MovementSystem.Rb.velocity, MovementSystem.UpDirection) > 0.1f) // If sliding up
            {
                CheckShouldCrouchOnExit();
            }
            else if (!MovementSystem.inputDetection.crouchInput)
            {
                if (MovementSystem.rbInfo.CanUncrouch()) MovementSystem.ChangeState(MovementSystem.IdleState);
                else MovementSystem.ChangeState(MovementSystem.CrouchingState);
            }
            else if (MovementSystem.inputDetection.movementInput.z <= 0) CheckShouldCrouchOnExit();// If letting go of crouch key
            else if (inAirCounter > 0.2f) MovementSystem.ChangeState(MovementSystem.InAirState); // If in air
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
                Vector3.Dot(MovementSystem.Rb.velocity, MovementSystem.orientation.up) < 0.1f)
            {
                targetSpeed = MovementSystem.forwardSpeed * 2;
            }
            float currentSpeed = Mathf.Lerp(MovementSystem.forwardSpeed, targetSpeed, (Time.time - stateStartTime) * 0.5f);
            MovementSystem.Rb.AddForce(MovementSystem.MoveDirection * currentSpeed, ForceMode.Acceleration);
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
            if(MovementSystem.inputDetection.crouchInput) MovementSystem.ChangeState(MovementSystem.CrouchingState);
            else
            {
                if(MovementSystem.rbInfo.CanUncrouch()) MovementSystem.ChangeState(MovementSystem.IdleState);
                else MovementSystem.ChangeState(MovementSystem.CrouchingState);
            }
        }
    }
}