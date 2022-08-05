using UnityEngine;

namespace Axiom.Player.Movement.StateMachine.States
{
    public class Walking : State
    {
        private float toRunCounter;
        
        public Walking(MovementSystem movementSystem) : base(movementSystem)
        {
            stateName = StateName.Walking;
        }

        public override void EnterState()
        {
            base.EnterState();

            MovementSystem.SetAnimatorBool("Walking", true);
            MovementSystem.SetTargetSpeed(MovementSystem.walkSpeed);

            if (MovementSystem.GetCurrentSpeed() >= MovementSystem.currentTargetSpeed) MovementSystem.ChangeState(MovementSystem._runningState);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (MovementSystem.inputDetection.movementInput.magnitude <= 0 || MovementSystem.inputDetection.movementInput.z < 0) MovementSystem.ChangeState(MovementSystem._idleState);
            else if (Mathf.Abs(MovementSystem.inputDetection.movementInput.x) > 0f) MovementSystem.ChangeState(MovementSystem._strafingState);
            else if (MovementSystem.inputDetection.movementInput.z > 0 && Time.time - stateStartTime > 0.75f) MovementSystem.ChangeState(MovementSystem._runningState);
            else if (MovementSystem.inputDetection.crouchInput)
            {
                if((MovementSystem.rbInfo.IsOnSlope() && MovementSystem.rb.velocity.y < 0f) ||
                   (MovementSystem.GetCurrentSpeed() > 5f)) MovementSystem.ChangeState(MovementSystem._slidingState);
                else MovementSystem.ChangeState(MovementSystem._crouchingState);
            }

            CalculateMovementSpeed();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

        public override void ExitState()
        {
            base.ExitState();
            
            MovementSystem.SetAnimatorBool("Walking", false);
        }
    }
}