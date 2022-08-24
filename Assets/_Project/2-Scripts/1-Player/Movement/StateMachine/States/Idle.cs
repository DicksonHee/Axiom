using UnityEngine;

namespace Axiom.Player.Movement.StateMachine.States
{
    public class Idle : State
    {
        public Idle(MovementSystem movementSystem) : base(movementSystem)
        {
            stateName = StateName.Idle;
        }

        public override void EnterState()
        {
            base.EnterState();
            
            MovementSystem.SetTargetSpeed(MovementSystem.idleSpeed);
            MovementSystem.SetAnimatorBool("Idle", true);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            if(MovementSystem.inputDetection.movementInput.z > 0f) MovementSystem.ChangeState(MovementSystem.WalkingState);
            else if(MovementSystem.inputDetection.movementInput.z < 0f) MovementSystem.ChangeState(MovementSystem.BackRunningState);
            else if(Mathf.Abs(MovementSystem.inputDetection.movementInput.x) > 0f) MovementSystem.ChangeState(MovementSystem.StrafingState);
            else if(MovementSystem.inputDetection.crouchInput) MovementSystem.ChangeState(MovementSystem.CrouchingState);
        }
        
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

        public override void ExitState()
        {
            base.ExitState();
            
            MovementSystem.SetAnimatorBool("Idle", false);
        }
    }
}