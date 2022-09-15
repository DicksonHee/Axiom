using UnityEngine;

namespace Axiom.Player.Movement.StateMachine.States
{
    public class Running : State
    {
        public Running(MovementSystem movementSystem) : base(movementSystem)
        {
            stateName = StateName.Running;
        }

        public override void EnterState()
        {
            base.EnterState();
            
            MovementSystem.SetTargetSpeed(MovementSystem.forwardSpeed);
            MovementSystem.SetLastForwardState(this);
            
            MovementSystem.SetAnimatorBool("Running", true);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            if(MovementSystem.inputDetection.movementInput.z < 0 || MovementSystem.inputDetection.movementInput.magnitude <= 0) MovementSystem.ChangeState(MovementSystem.IdleState);
            else if(Mathf.Abs(MovementSystem.inputDetection.movementInput.x) > 0f) MovementSystem.ChangeState(MovementSystem.StrafingState);
            else if (MovementSystem.inputDetection.crouchInput) MovementSystem.ChangeState(MovementSystem.SlidingState);
        }

        public override void PhysicsUpdate()
        {
        }

        public override void ExitState()
        {
            base.ExitState();
            
            MovementSystem.SetAnimatorBool("Running", false);
        }
    }
}