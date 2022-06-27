using System;
using System.Collections;
using System.Collections.Generic;
using Axiom.Player.Movement;
using Axiom.Player.StateMachine;
using UnityEngine;

namespace Axiom.Player.StateMachine
{
    public class Idle : State
    {
        public Idle(MovementSystem movementSystem) : base(movementSystem)
        {
            stateName = StateName.Idle;
        }

        public override void EnterState(StateName state)
        {
            base.EnterState(state);
            
            MovementSystem.SetDrag(MovementSystem.stoppingDrag);
            MovementSystem.SetGravity(MovementSystem.groundGravity);
            MovementSystem.SetTargetSpeed(0f);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            if(MovementSystem.inputDetection.movementInput.z > 0f) MovementSystem.ChangeState(MovementSystem._walkingState);
            else if(MovementSystem.inputDetection.movementInput.z < 0f) MovementSystem.ChangeState(MovementSystem._backRunningState);
            else if(Mathf.Abs(MovementSystem.inputDetection.movementInput.x) > 0f) MovementSystem.ChangeState(MovementSystem._strafingState);
            
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
                    break;
                case StateName.Walking:
                    movementCurve = MovementSystem.decelerationCurve;
                    break;
                case StateName.Running:
                    movementCurve = MovementSystem.decelerationCurve;
                    break;
                case StateName.Strafing:
                    movementCurve = MovementSystem.decelerationCurve;
                    break;
                case StateName.InAir:
                    movementCurve = MovementSystem.decelerationCurve;
                    break;
                case StateName.Climbing:
                    break;
                case StateName.Sliding:
                    break;
                case StateName.WallRunning:
                    break;
                case StateName.BackRunning:
                    movementCurve = MovementSystem.decelerationCurve;
                    break;
            }
        }
    }
}