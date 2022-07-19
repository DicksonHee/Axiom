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

        public override void EnterState()
        {
            base.EnterState();

            MovementSystem.SetAnimatorBool("Running", true);
            MovementSystem.SetTargetSpeed(MovementSystem.forwardSpeed);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            if(MovementSystem.inputDetection.movementInput.z < 0 || MovementSystem.inputDetection.movementInput.magnitude <= 0) MovementSystem.ChangeState(MovementSystem._idleState);
            else if(Mathf.Abs(MovementSystem.inputDetection.movementInput.x) > 0f) MovementSystem.ChangeState(MovementSystem._strafingState);
            else if (MovementSystem.inputDetection.crouchInput) MovementSystem.ChangeState(MovementSystem._slidingState);
            
            CalculateMovementSpeed();
        }

        public override void PhysicsUpdate()
        {
        }

        public override void ExitState()
        {
            base.ExitState();
            
            MovementSystem.SetAnimatorBool("Running", false);
        }
    }
}