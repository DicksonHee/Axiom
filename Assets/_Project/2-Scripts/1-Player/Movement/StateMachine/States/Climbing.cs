using UnityEngine;

namespace Axiom.Player.Movement.StateMachine.States
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

            MovementSystem.cameraLook.StartClimbCamera();
            MovementSystem.SetAnimatorBool("WallClimb", true);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (Time.time - stateStartTime > MovementSystem.wallClimbMaxDuration || !MovementSystem.rbInfo.CanWallClimb()) MovementSystem.ChangeState(MovementSystem.InAirState);
            else if (MovementSystem.rbInfo.CanClimbLedge() && !MovementSystem.IsExitingLedgeGrab) MovementSystem.ChangeState(MovementSystem.LedgeGrabbingState);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            CalculateClimbingMovement();
        }
        
        public override void ExitState()
        {
            base.ExitState();

            MovementSystem.ExitClimbState();
            MovementSystem.EnableMovement();
            MovementSystem.SetGravity(MovementSystem.inAirGravity);

            MovementSystem.cameraLook.ResetCamera();
            MovementSystem.SetAnimatorBool("WallClimb", false);
        }

        private void CalculateClimbingMovement()
        {
            Vector3 up = MovementSystem.transform.up;
            Vector3 inputVel = MovementSystem.MoveDirection * 0.1f;
            Vector3 moveVel = Vector3.ProjectOnPlane(inputVel, up);
            MovementSystem.Rb.velocity = (moveVel + up)  * MovementSystem.wallClimbSpeed;
        }
    }
}