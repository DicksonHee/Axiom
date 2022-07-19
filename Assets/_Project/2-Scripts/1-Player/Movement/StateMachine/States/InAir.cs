using System.Collections;
using System.Collections.Generic;
using Axiom.Player.Movement;
using Axiom.Player.StateMachine;
using UnityEngine;

namespace Axiom.Player.StateMachine
{
    public class InAir : State
    {
        private float initialHeight;

        private float wallClimbTimer;

        public InAir(MovementSystem movementSystem) : base(movementSystem)
        {
            stateName = StateName.InAir;
        }

        public override void EnterState()
        {
            base.EnterState();
            
            initialHeight = MovementSystem.transform.position.y;
            wallClimbTimer = 0f;

            MovementSystem.cameraLook.ApplyCameraXAxisMultiplier(0.5f);
            MovementSystem._rb.drag = 1f;
            MovementSystem.SetGravity(MovementSystem.inAirGravity);
            MovementSystem.SetTargetSpeed(MovementSystem.inAirSpeed);
            MovementSystem.SetLRMultiplier(0.25f);
            
            MovementSystem.SetAnimatorBool("InAir", true);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            if (MovementSystem.rbInfo.isGrounded) MovementSystem.ChangeState(MovementSystem._idleState);
            else if (MovementSystem.inputDetection.movementInput.z > 0f)
            {
                if (MovementSystem.rbInfo.canWallClimb && !MovementSystem.isExitingClimb) // Check for wall climb
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
            else if (MovementSystem.rbInfo.isDetectingLedge && !MovementSystem.isExitingLedgeGrab) MovementSystem.ChangeState(MovementSystem._ledgeGrabbingState);

            MovementSystem.SetMaxHeight(initialHeight - MovementSystem.transform.position.y);
            MovementSystem.playerAnimation.SetFloatParam("LandHeight", MovementSystem._maxHeight);
            
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

        private bool ShouldWallClimb()
        {
            return wallClimbTimer > 0.25f;
        }
    }
}

