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

            MovementSystem.SetTargetSpeed(MovementSystem.walkSpeed);
            MovementSystem.SetLastForwardState(this);
            
            MovementSystem.SetAnimatorBool("Walking", true);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (MovementSystem.inputDetection.movementInput.magnitude <= 0 || MovementSystem.inputDetection.movementInput.z < 0) MovementSystem.ChangeState(MovementSystem.IdleState);
            else if (Mathf.Abs(MovementSystem.inputDetection.movementInput.x) > 0f) MovementSystem.ChangeState(MovementSystem.StrafingState);
            else if (MovementSystem.inputDetection.movementInput.z > 0 && Time.time - stateStartTime > 0.5f) MovementSystem.ChangeState(MovementSystem.RunningState);
            else if (MovementSystem.inputDetection.crouchInput)
            {
                if((MovementSystem.rbInfo.IsOnSlope() && MovementSystem.Rb.velocity.y < 0f) ||
                   MovementSystem.GetCurrentSpeed() > 5f) MovementSystem.ChangeState(MovementSystem.SlidingState);
                else MovementSystem.ChangeState(MovementSystem.CrouchingState);
            }
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