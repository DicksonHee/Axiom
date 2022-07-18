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

            MovementSystem.EnterClimbState();
            MovementSystem.DisableMovement();
            MovementSystem.SetGravity(0f);
            
            MovementSystem.SetAnimatorBool("WallClimb", true);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (Time.time - stateStartTime > MovementSystem.wallClimbMaxDuration || !MovementSystem.rbInfo.canWallClimb) MovementSystem.ChangeState(MovementSystem._inAirState);
            else if (MovementSystem.rbInfo.isDetectingLedge && !MovementSystem.isExitingLedgeGrab) MovementSystem.ChangeState(MovementSystem._ledgeGrabbingState);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
        
        public override void ExitState()
        {
            base.ExitState();

            MovementSystem.ExitClimbState();
            MovementSystem.EnableMovement();
            MovementSystem.SetGravity(MovementSystem.inAirGravity);
            //Vector3 inputVel = MovementSystem.moveDirection;
            //MovementSystem._rb.velocity = new Vector3(inputVel.x, MovementSystem.wallClimbSpeed * 1.5f, inputVel.z);
            
            MovementSystem.SetAnimatorBool("WallClimb", false);
        }

        private void CalculateClimbingMovement()
        {
            Vector3 inputVel = MovementSystem.moveDirection * 0.1f;
            Vector3 moveVel = MovementSystem.ProjectDirectionOnPlane(inputVel, -MovementSystem.transform.forward);
            MovementSystem._rb.velocity = moveVel * MovementSystem.wallClimbSpeed;
        }
    }
}