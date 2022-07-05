using System.Collections;
using System.Collections.Generic;
using Axiom.Player.Movement;
using Axiom.Player.StateMachine;
using UnityEngine;

namespace Axiom.Player.StateMachine
{
    public class Sliding : State
    {
        private Vector3 initialDir;
        private float initialSpeed;
        private float slopeSpeed;
        private float inAirCounter;
        private float distanceMultiplier;

        public Sliding(MovementSystem movementSystem) : base(movementSystem)
        {
            stateName = StateName.Sliding;
        }

        public override void EnterState()
        {
            base.EnterState();

            MovementSystem.SetAnimatorBool("Sliding", true);

            slopeSpeed = 0f;
            initialDir = MovementSystem.moveDirection;
            initialSpeed = MovementSystem._rb.velocity.magnitude;
            distanceMultiplier = Mathf.Clamp(1 - initialSpeed / MovementSystem.forwardSpeed, 0.5f, 1f);
            MovementSystem.StartCrouch();
            MovementSystem.SetTargetSpeed(0f);
            MovementSystem.SetLRMultiplier(0.1f);
            MovementSystem.DisableMovement();
            
            Debug.Log(distanceMultiplier);
            
            MovementSystem.rbInfo.OnSlopeEnded += ResetStateTimer;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (MovementSystem._rb.velocity.magnitude < 0.5f && Time.time - stateStartTime > 0.5f)
            {
                if (!MovementSystem.inputDetection.crouchInput) MovementSystem.ChangeState(MovementSystem._idleState);
                else if (MovementSystem.inputDetection.crouchInput) MovementSystem.ChangeState(MovementSystem._crouchingState);
            }
            else if (!MovementSystem.inputDetection.crouchInput) MovementSystem.ChangeState(MovementSystem._idleState);
            else if (inAirCounter > 0.8f) MovementSystem.ChangeState(MovementSystem._inAirState);
            
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
            
            MovementSystem.EnableMovement();
            MovementSystem.EndCrouch();
            MovementSystem.SetLRMultiplier(1f);
            MovementSystem.ExitSlideState();
            MovementSystem.SetAnimatorBool("Sliding", false);
            
            MovementSystem.rbInfo.OnSlopeEnded -= ResetStateTimer;
        }

        private void CalculateSlideSpeed()
        {
            float currentSpeed = 0f;
            if (MovementSystem.rbInfo.isOnSlope)
            {
                float velDiff = initialSpeed - MovementSystem.forwardSpeed * 2;
                currentSpeed = Mathf.Clamp(initialSpeed + velDiff * MovementSystem.reverseSlideCurve.Evaluate((Time.time - stateStartTime) * distanceMultiplier), 0, float.MaxValue);
                
                if (MovementSystem._rb.velocity.magnitude > slopeSpeed) slopeSpeed = MovementSystem._rb.velocity.magnitude;
            }
            else if (!MovementSystem.rbInfo.isOnSlope)
            {
                float velDiff = initialSpeed - 0;
                currentSpeed = Mathf.Clamp(initialSpeed - velDiff * MovementSystem.slideCurve.Evaluate((Time.time - stateStartTime) * distanceMultiplier), 0, float.MaxValue);
            }

            Vector3 moveVel = initialDir.normalized * currentSpeed;
            moveVel = MovementSystem.CheckSlopeMovementDirection(moveVel);
            moveVel.y += MovementSystem._rb.velocity.y;
            MovementSystem._rb.velocity = moveVel;
        }

        private void ResetStateTimer()
        {
            stateStartTime = Time.time;
            initialSpeed = slopeSpeed;
            distanceMultiplier = Mathf.Clamp(1 - initialSpeed / MovementSystem.forwardSpeed, 0.5f, 1f);
        }
        
        private void CalculateInAirTime()
        {
            if (!MovementSystem.rbInfo.isGrounded) inAirCounter += Time.deltaTime;
            else inAirCounter = 0f;
        }
    }
}