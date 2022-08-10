using UnityEngine;

namespace Axiom.Player.Movement.StateMachine.States
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
            MovementSystem.rb.drag = 0.5f;
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
                MovementSystem.ChangeState(MovementSystem._ledgeGrabbingState);
            }
            else if (MovementSystem.inputDetection.movementInput.z > 0f)
            {
                if (MovementSystem.rbInfo.CanWallClimb() && !MovementSystem.isExitingClimb) // Check for wall climb
                {
                    wallClimbTimer += Time.deltaTime;
                    if (ShouldWallClimb()) MovementSystem.ChangeState(MovementSystem._climbingState);
                }
                else if (((MovementSystem.rbInfo.IsLeftWallDetected() && MovementSystem.GetPreviousWall() != MovementSystem.rbInfo.GetLeftWall()) ||
                          (MovementSystem.rbInfo.IsRightWallDetected() && MovementSystem.GetPreviousWall() != MovementSystem.rbInfo.GetRightWall())) && 
                           !MovementSystem.isExitingLedgeGrab && 
                           !MovementSystem.isExitingWallRun) // Check for wall run
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

            MovementSystem.totalAirTime = Time.time - stateStartTime;
            MovementSystem.cameraLook.ResetCameraXSens();
            MovementSystem.SetLRMultiplier(1f);
            MovementSystem.rb.drag = 5f;
            
            MovementSystem.SetAnimatorBool("InAir", false);
        }

        public void WallRunJump(Vector3 jumpVelocity)
        {
            if (hasWallJumped || Time.time - stateStartTime > MovementSystem.wallRunCoyoteTime) return;

            hasWallJumped = true;
            MovementSystem.rb.velocity = jumpVelocity;
        }

        public void InAirJump(Vector3 jumpVelocity)
        {
            if (hasAirJumped || Time.time - stateStartTime > MovementSystem.inAirCoyoteTime) return;

            hasAirJumped = true;
            MovementSystem.rb.AddForce(jumpVelocity);
        }

        private bool ShouldWallClimb()
        {
            return wallClimbTimer > 0.1f;
        }
    }
}

