using UnityEngine;

namespace Axiom.Player.Movement.StateMachine.States
{
    public class BackRunning : State
    {
        public BackRunning(MovementSystem movementSystem) : base(movementSystem)
        {
            stateName = StateName.BackRunning;
        }

        public override void EnterState()
        {
            base.EnterState();
            
            MovementSystem.SetTargetSpeed(MovementSystem.backwardSpeed);
            MovementSystem.SetAnimatorBool("Backwards", true);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            if (MovementSystem.inputDetection.movementInput.magnitude <= 0 || MovementSystem.inputDetection.movementInput.z > 0f) MovementSystem.ChangeState(MovementSystem.IdleState);
            else if(MovementSystem.inputDetection.crouchInput) MovementSystem.ChangeState(MovementSystem.CrouchingState);
            else if(Mathf.Abs(MovementSystem.inputDetection.movementInput.x) > 0) MovementSystem.ChangeState(MovementSystem.StrafingState);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

        public override void ExitState()
        {
            base.ExitState();
            
            MovementSystem.SetAnimatorBool("Backwards", false);
        }
    }
}