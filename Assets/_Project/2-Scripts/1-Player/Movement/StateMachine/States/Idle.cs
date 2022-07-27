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

        public override void EnterState()
        {
            base.EnterState();
            
            MovementSystem.SetTargetSpeed(MovementSystem.idleSpeed);
            MovementSystem.SetAnimatorBool("Idle", true);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            if(MovementSystem.inputDetection.movementInput.z > 0f && !MovementSystem.isExitingSlide) MovementSystem.ChangeState(MovementSystem._walkingState);
            else if(MovementSystem.inputDetection.movementInput.z < 0f) MovementSystem.ChangeState(MovementSystem._backRunningState);
            else if(Mathf.Abs(MovementSystem.inputDetection.movementInput.x) > 0f) MovementSystem.ChangeState(MovementSystem._strafingState);
            else if(MovementSystem.inputDetection.crouchInput) MovementSystem.ChangeState(MovementSystem._crouchingState);

            CalculateMovementSpeed();
        }
        
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

        public override void ExitState()
        {
            base.ExitState();
            
            MovementSystem.SetAnimatorBool("Idle", false);
        }
    }
}