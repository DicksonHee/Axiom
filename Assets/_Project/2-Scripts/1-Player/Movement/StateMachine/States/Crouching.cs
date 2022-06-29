using System.Collections;
using System.Collections.Generic;
using Axiom.Player.Movement;
using Axiom.Player.StateMachine;
using UnityEngine;

namespace Axiom.Player.StateMachine
{
    public class Crouching : State
    {
        public Crouching(MovementSystem movementSystem) : base(movementSystem)
        {
            stateName = StateName.Crouching;
        }

        public override void EnterState(StateName prevState)
        {
            base.EnterState(prevState);
            
            MovementSystem.StartCrouch();
            MovementSystem.SetTargetSpeed(MovementSystem.crouchSpeed);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            if(!MovementSystem.inputDetection.crouchInput) MovementSystem.ChangeState(MovementSystem._idleState);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

        public override void ExitState()
        {
            base.ExitState();
            
            MovementSystem.EndCrouch();
        }
    }
}