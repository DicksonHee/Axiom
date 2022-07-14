using System.Collections;
using System.Collections.Generic;
using Axiom.Player.Movement;
using Axiom.Player.StateMachine;
using UnityEngine;

namespace Axiom.Player.StateMachine
{
    public class Climbing : State
    {
        public Climbing(MovementSystem movementSystem) : base(movementSystem)
        {
            stateName = StateName.Climbing;
        }

        public override void EnterState()
        {
            base.EnterState();

            MovementSystem.DisableMovement();
            MovementSystem.SetGravity(0f);
            
            MovementSystem.SetAnimatorBool("WallClimb", true);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (Time.time - stateStartTime > MovementSystem.wallClimbMaxDuration ||
                !MovementSystem.rbInfo.canWallClimb)
                MovementSystem.ChangeState(MovementSystem._inAirState);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            CalculateClimbingMovement();
        }
        
        public override void ExitState()
        {
            base.ExitState();
            
            MovementSystem.isExitingClimb = true;
            MovementSystem.EnableMovement();
            Vector3 inputVel = MovementSystem.moveDirection;
            MovementSystem._rb.velocity = new Vector3(inputVel.x, MovementSystem.wallClimbSpeed * 1.5f, inputVel.z);
            
            MovementSystem.SetAnimatorBool("WallClimb", false);
        }

        private void CalculateClimbingMovement()
        {
            Vector3 inputVel = MovementSystem.moveDirection * 0.1f;
            MovementSystem._rb.velocity = new Vector3(inputVel.x, MovementSystem.wallClimbSpeed, inputVel.z);
        }
    }
}