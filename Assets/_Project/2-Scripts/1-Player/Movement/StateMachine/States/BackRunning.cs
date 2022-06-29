using System;
using System.Collections;
using System.Collections.Generic;
using Axiom.Player.Movement;
using Axiom.Player.StateMachine;
using UnityEngine;

namespace Axiom.Player.StateMachine
{
    public class BackRunning : State
    {
        public BackRunning(MovementSystem movementSystem) : base(movementSystem)
        {
            stateName = StateName.BackRunning;
        }

        public override void EnterState(StateName state)
        {
            base.EnterState(state);
            
            MovementSystem.SetDrag(MovementSystem.groundedDrag);
            MovementSystem.SetGravity(MovementSystem.groundGravity);
            MovementSystem.SetTargetSpeed(MovementSystem.backwardSpeed);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            if (MovementSystem.inputDetection.movementInput.magnitude <= 0 || MovementSystem.inputDetection.movementInput.z > 0f) MovementSystem.ChangeState(MovementSystem._idleState);
            else if(MovementSystem.inputDetection.crouchInput) MovementSystem.ChangeState(MovementSystem._crouchingState);
            
            CalculateMovementSpeed();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

        protected override void SelectMovementCurve()
        {
            base.SelectMovementCurve();
            
            switch (previousState)
            {
                case StateName.Idle:
                    movementCurve = MovementSystem.accelerationCurve;
                    break;
                case StateName.Walking:
                    movementCurve = MovementSystem.accelerationCurve;
                    break;
                case StateName.Running:
                    movementCurve = MovementSystem.decelerationCurve;
                    break;
                case StateName.Strafing:
                    movementCurve = MovementSystem.accelerationCurve;
                    break;
                case StateName.InAir:
                    movementCurve = MovementSystem.accelerationCurve;
                    break;
                case StateName.Climbing:
                    break;
                case StateName.Sliding:
                    break;
                case StateName.WallRunning:
                    break;
                case StateName.BackRunning:
                    break;
            }
        }
    }
}