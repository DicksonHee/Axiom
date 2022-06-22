using System.Collections;
using System.Collections.Generic;
using Axiom.Player.Movement;
using UnityEngine;

namespace Axiom.Player.StateMachine
{
    public abstract class State
    {
        protected MovementSystem MovementSystem;

        protected float stateStartTime;
        
        public State(MovementSystem movementSystem)
        {
            MovementSystem = movementSystem;
        }

        public virtual void EnterState()
        {
            stateStartTime = Time.time;
        }

        public virtual void ExitState()
        {
        }

        protected virtual void LogicUpdate()
        {
        }

        protected virtual void PhysicsUpdate()
        {
        }
    }
}
