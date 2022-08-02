using System.Collections;
using System.Collections.Generic;
using Axiom.Player.Movement;
using Axiom.Player.StateMachine;
using UnityEngine;

namespace Axiom.Player.StateMachine
{
    public class Crouching : State
    {
        private float inAirCounter;

        public Crouching(MovementSystem movementSystem) : base(movementSystem)
        {
            stateName = StateName.Crouching;
        }

        public override void EnterState()
        {
            base.EnterState();

            MovementSystem.StartCrouch();
            MovementSystem.SetTargetSpeed(MovementSystem.crouchSpeed);
            MovementSystem.SetAnimatorBool("Crouching", true);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            CalculateInAirTime();

            if (!MovementSystem.inputDetection.crouchInput && MovementSystem.rbInfo.CanUncrouch()) MovementSystem.ChangeState(MovementSystem._idleState);
            if (inAirCounter > 0.8f) MovementSystem.ChangeState(MovementSystem._inAirState);
            
            CalculateMovementSpeed();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

        public override void ExitState()
        {
            base.ExitState();

            MovementSystem.EndCrouch();
            MovementSystem.SetAnimatorBool("Crouching", false);
        }

        private void CalculateInAirTime()
        {
            if (!MovementSystem.rbInfo.IsGrounded()) inAirCounter += Time.deltaTime;
            else inAirCounter = 0f;
        }
    }
}