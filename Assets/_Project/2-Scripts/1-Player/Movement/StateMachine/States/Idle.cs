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
        }

        public override void EnterState()
        {
            base.EnterState();
            
            MovementSystem.SetDrag(MovementSystem.stoppingDrag);
        }

        public override void LogicUpdate()
        {
            if (MovementSystem.inputDetection.movementInput.magnitude > 0f)
            {
                MovementSystem.ChangeState(MovementSystem._runningState);
            }
        }
        
        public override void PhysicsUpdate()
        {
            if (MovementSystem.inputDetection.jumpInput && MovementSystem.rbInfo.isGrounded) MovementSystem.Jump();
        }
    }
}