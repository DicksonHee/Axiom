using System.Collections;
using System.Collections.Generic;
using Axiom.Player.Movement;
using Axiom.Player.StateMachine;
using UnityEngine;

namespace Axiom.Player.StateMachine
{
    public class InAir : State
    {
        public InAir(MovementSystem movementSystem) : base(movementSystem)
        {
            stateName = StateName.InAir;
        }

        public override void EnterState(StateName prevState)
        {
            base.EnterState(prevState);
            
            MovementSystem.SetDrag(MovementSystem.stoppingDrag);
            MovementSystem.SetGravity(MovementSystem.inAirGravity);
            MovementSystem.SetTargetSpeed(MovementSystem.inAirSpeed);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (MovementSystem.rbInfo.isGrounded)
            {
                if (MovementSystem.inputDetection.movementInput.z > 0) MovementSystem.ChangeState(MovementSystem._walkingState);
                else if (MovementSystem.inputDetection.movementInput.z < 0 || MovementSystem.inputDetection.movementInput.magnitude <= 0) MovementSystem.ChangeState(MovementSystem._idleState);
            }

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
                    movementCurve = MovementSystem.slowToFast;
                    break;
                case StateName.Walking:
                    movementCurve = MovementSystem.fastToSlow;
                    break;
                case StateName.Running:
                    movementCurve = MovementSystem.fastToSlow;
                    break;
                case StateName.Strafing:
                    movementCurve = MovementSystem.fastToSlow;
                    break;
                case StateName.InAir:
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
                    movementCurve = MovementSystem.slowToFast;
                    break;
            }
        }
    }
}

