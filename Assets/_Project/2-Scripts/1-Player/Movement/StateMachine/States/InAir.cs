using System.Collections;
using System.Collections.Generic;
using Axiom.Player.Movement;
using Axiom.Player.StateMachine;
using UnityEngine;

namespace Axiom.Player.StateMachine
{
    public class InAir : State
    {
        private bool hasAirJumped;
        private bool hasWallJumped;
        private float wallClimbTimer;
        

        public InAir(MovementSystem movementSystem) : base(movementSystem)
        {
            stateName = StateName.InAir;
        }

        public override void EnterState()
        {
            base.EnterState();

            hasAirJumped = false;
            hasWallJumped = false;
            wallClimbTimer = 0f;

            MovementSystem.cameraLook.ApplyCameraXAxisMultiplier(0.5f);
            MovementSystem._rb.drag = 0.5f;
            MovementSystem.SetGravity(MovementSystem.inAirGravity);
            MovementSystem.SetTargetSpeed(MovementSystem.inAirSpeed);
            MovementSystem.SetLRMultiplier(0.25f);
            
            MovementSystem.SetAnimatorBool("InAir", true);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            if (MovementSystem.rbInfo.IsGrounded()) MovementSystem.ChangeState(MovementSystem._idleState);
            else if (MovementSystem.rbInfo.CanClimbLedge() && !MovementSystem.isExitingLedgeGrab)
            {
                Debug.Log("ASD");
                MovementSystem.ChangeState(MovementSystem._ledgeGrabbingState);
            }
            else if (MovementSystem.inputDetection.movementInput.z > 0f)
            {
                if (MovementSystem.rbInfo.CanWallClimb() && !MovementSystem.isExitingClimb) // Check for wall climb
                {
                    wallClimbTimer += Time.deltaTime;
                    if (ShouldWallClimb()) MovementSystem.ChangeState(MovementSystem._climbingState);
                }
                else if (((MovementSystem.rbInfo.IsLeftWallDetected() && MovementSystem.previousWall != MovementSystem.rbInfo.GetLeftWall()) ||
                          (MovementSystem.rbInfo.IsRightWallDetected() && MovementSystem.previousWall != MovementSystem.rbInfo.GetRightWall()))
                          && !MovementSystem.isExitingLedgeGrab) // Check for wall run
                {
                    MovementSystem.ChangeState(MovementSystem._wallRunningState);
                }
            }
            
            MovementSystem.playerAnimation.SetFloatParam("LandHeight", Time.time - stateStartTime);
            
            CalculateMovementSpeed();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

        public override void ExitState()
        {
            base.ExitState();
            
            MovementSystem.cameraLook.ResetCameraXSens();
            MovementSystem.SetLRMultiplier(1f);
            MovementSystem._rb.drag = 5f;
            
            MovementSystem.SetAnimatorBool("InAir", false);
        }

        public void WallRunJump(Vector3 jumpVelocity)
        {
            if (hasWallJumped) return;

            hasWallJumped = true;
            MovementSystem._rb.velocity = jumpVelocity;
        }

        public void InAirJump(Vector3 jumpVelocity)
        {
            if (hasAirJumped) return;

            hasAirJumped = true;
            MovementSystem._rb.AddForce(jumpVelocity);
        }

        private bool ShouldWallClimb()
        {
            return wallClimbTimer > 0.18f;
        }
    }
}

