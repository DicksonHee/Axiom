using System.Collections;
using System.Collections.Generic;
using Axiom.Player.Movement;
using Axiom.Player.StateMachine;
using UnityEngine;

namespace Axiom.Player.StateMachine
{
    public class Grounded : State
    {
        public Grounded(MovementSystem movementSystem) : base(movementSystem)
        {
        }

        protected override void LogicUpdate()
        {
            base.LogicUpdate();
        }

        protected override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }
}