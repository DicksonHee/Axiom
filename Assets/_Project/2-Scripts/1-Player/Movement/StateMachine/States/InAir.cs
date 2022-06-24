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
        }

        public override void LogicUpdate()
        {
            if (MovementSystem.rbInfo.isGrounded) MovementSystem.ChangeState(MovementSystem._idleState);
            MovementSystem.ApplyGravity(MovementSystem.inAirGravity);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            MovementSystem.MovePlayer(MovementSystem.MoveDirection());
        }
    }
}

