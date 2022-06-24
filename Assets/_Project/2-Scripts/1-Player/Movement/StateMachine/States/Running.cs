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
        }

        public override void EnterState()
        {
            base.EnterState();
            
            MovementSystem.SetDrag(MovementSystem.groundedDrag);
        }

        public override void LogicUpdate()
        {
            if (MovementSystem.inputDetection.movementInput.magnitude <= 0)
            {
                MovementSystem.ChangeState(MovementSystem._idleState);
            }
        }

        public override void PhysicsUpdate()
        {
            float velocityMultiplier = MovementSystem.idleToRun.Evaluate(Time.time - stateStartTime);
            MovementSystem.MovePlayer(MovementSystem.MoveDirection() * velocityMultiplier);
            
            if (MovementSystem.inputDetection.jumpInput && MovementSystem.rbInfo.isGrounded) MovementSystem.Jump();
        }
    }
}