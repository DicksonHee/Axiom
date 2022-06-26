using System.Collections;
using System.Collections.Generic;
using Axiom.Player.Movement;
using Axiom.Player.StateMachine;
using UnityEngine;

namespace Axiom.Player.StateMachine
{
    public class Running : State
    {
        public Running(MovementSystem movementSystem) : base(movementSystem)
        {
            stateName = StateName.Running;
        }

        public override void EnterState(StateName state)
        {
            base.EnterState(state);
            
            MovementSystem.SetDrag(MovementSystem.groundedDrag);
            MovementSystem.SetGravity(MovementSystem.groundGravity);
            MovementSystem.SetTargetSpeed(MovementSystem.forwardSpeed);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            if(MovementSystem.inputDetection.movementInput.z < 0 || MovementSystem.inputDetection.movementInput.magnitude <= 0) MovementSystem.ChangeState(MovementSystem._idleState);
            else if(Mathf.Abs(MovementSystem.inputDetection.movementInput.x) > 0f) MovementSystem.ChangeState(MovementSystem._strafingState);
            
            CalculateMovementSpeed();
        }

        public override void PhysicsUpdate()
        {
        }

        protected override void SelectMovementCurve()
        {
            base.SelectMovementCurve();
            
            switch (previousState)
            {
                case StateName.Idle:
                    movementCurve = MovementSystem.slowToFast;
                    break;
                case StateName.Walking:
                    movementCurve = MovementSystem.slowToFast;
                    break;
                case StateName.Running:
                    break;
                case StateName.Strafing:
                    movementCurve = MovementSystem.slowToFast;
                    break;
                case StateName.InAir:
                    movementCurve = MovementSystem.slowToFast;
                    break;
                case StateName.Climbing:
                    break;
                case StateName.Sliding:
                    break;
                case StateName.Turning:
                    movementCurve = MovementSystem.turnToMove;
                    break;
                case StateName.WallRunning:
                    break;
                case StateName.BackRunning:
                    movementCurve = MovementSystem.fastToSlow;
                    break;
            }
        }
    }
}