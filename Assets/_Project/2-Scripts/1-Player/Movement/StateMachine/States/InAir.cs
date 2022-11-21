using UnityEngine;

namespace Axiom.Player.Movement.StateMachine.States
{
    public class InAir : State
    {
        private float wallClimbTimer;
        
        public InAir(MovementSystem movementSystem) : base(movementSystem)
        {
            stateName = StateName.InAir;
        }

        public override void EnterState()
        {
            base.EnterState();
            wallClimbTimer = 0f;

            MovementSystem.cameraLook.ApplyCameraXAxisMultiplier(0.5f);
            MovementSystem.Rb.drag = 0.5f;
            MovementSystem.SetGravity(MovementSystem.inAirGravity);
            MovementSystem.SetTargetSpeed(MovementSystem.inAirSpeed);
            MovementSystem.SetLRMultiplier(0.25f);
            
            MovementSystem.SetAnimatorBool("InAir", true);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (MovementSystem.rbInfo.IsGrounded()) MovementSystem.ChangeState(MovementSystem.IdleState);
            else if (MovementSystem.rbInfo.CanClimbLedge() && !MovementSystem.IsExitingLedgeGrab)
            {
                MovementSystem.ChangeState(MovementSystem.LedgeGrabbingState);
            }
            else if (MovementSystem.inputDetection.movementInput.z > 0f)
            {
                if (MovementSystem.rbInfo.CanWallClimb() && !MovementSystem.IsExitingClimb) // Check for wall climb
                {
                    wallClimbTimer += Time.deltaTime;
                    if (ShouldWallClimb()) MovementSystem.ChangeState(MovementSystem.ClimbingState);
                }
                else if ((MovementSystem.rbInfo.IsLeftWallDetected() && MovementSystem.GetPreviousWall() != MovementSystem.rbInfo.GetLeftWall()) ||
                          (MovementSystem.rbInfo.IsRightWallDetected() && MovementSystem.GetPreviousWall() != MovementSystem.rbInfo.GetRightWall()) && 
                           !MovementSystem.IsExitingLedgeGrab && 
                           !MovementSystem.IsExitingWallRun) // Check for wall run
                {
                    MovementSystem.ChangeState(MovementSystem.WallRunningState);
                }
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

        public override void ExitState()
        {
            base.ExitState();

            MovementSystem.SetTotalAirTime(Time.time - stateStartTime);
            MovementSystem.cameraLook.ResetCameraXSens();
            MovementSystem.SetLRMultiplier(1f);
            MovementSystem.Rb.drag = 5f;

            MovementSystem.SetAnimatorBool("InAir", false);
        }

        private bool ShouldWallClimb()
        {
            return wallClimbTimer > 0.1f;
        }
    }
}

