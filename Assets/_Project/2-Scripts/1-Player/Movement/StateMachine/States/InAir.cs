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

            MovementSystem.SetGravity(MovementSystem.inAirGravity);
            MovementSystem.SetTargetSpeed(MovementSystem.inAirSpeed);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (MovementSystem.rbInfo.isGrounded) MovementSystem.ChangeState(MovementSystem._idleState);
            else if (MovementSystem._rb.velocity.y > 0 && !MovementSystem.isExitingWallRun)
            {
                if(MovementSystem.inputDetection.movementInput.x < 0 && MovementSystem.rbInfo.leftWallDetected) MovementSystem.ChangeState(MovementSystem._wallRunningState);
                else if(MovementSystem.inputDetection.movementInput.x > 0 && MovementSystem.rbInfo.rightWallDetected) MovementSystem.ChangeState(MovementSystem._wallRunningState);
            }

            CalculateMovementSpeed();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

        public override void ExitState()
        {
            base.ExitState();
        }
    }
}

