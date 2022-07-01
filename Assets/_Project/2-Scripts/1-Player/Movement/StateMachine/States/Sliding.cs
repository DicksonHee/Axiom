using System.Collections;
using System.Collections.Generic;
using Axiom.Player.Movement;
using Axiom.Player.StateMachine;
using UnityEngine;

namespace Axiom.Player.StateMachine
{
    public class Sliding : State
    {
        private float initialSpeed;
        private float slopeSpeed;
        private float inAirCounter;
        
        public Sliding(MovementSystem movementSystem) : base(movementSystem)
        {
            stateName = StateName.Sliding;
        }

        public override void EnterState(StateName prevState)
        {
            base.EnterState(prevState);

            initialSpeed = MovementSystem._rb.velocity.magnitude;
            MovementSystem.StartCrouch();
            MovementSystem.SetTargetSpeed(0f);
            MovementSystem.lrMultiplier = 0.1f;
            
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
            CalculateSlideSpeed();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

        public override void ExitState()
        {
            MovementSystem.EndCrouch();
            MovementSystem.lrMultiplier = 1f;
            MovementSystem.rbInfo.OnSlopeEnded -= ResetStateTimer;
            
            base.ExitState();
        }

        private void CalculateSlideSpeed()
        {
            if (MovementSystem.rbInfo.isOnSlope)
            {
                CalculateMovementSpeed(MovementSystem.reverseSlideCurve, initialSpeed);
                if (MovementSystem._rb.velocity.magnitude > slopeSpeed) slopeSpeed = MovementSystem._rb.velocity.magnitude;
            }
            else if (!MovementSystem.rbInfo.isOnSlope) CalculateMovementSpeed(MovementSystem.slideCurve, slopeSpeed);
        }

        private void ResetStateTimer()
        {
            stateStartTime = Time.time;
        }
        
        private void CalculateInAirTime()
        {
            if (!MovementSystem.rbInfo.isGrounded) inAirCounter += Time.deltaTime;
            else inAirCounter = 0f;
        }
    }
}