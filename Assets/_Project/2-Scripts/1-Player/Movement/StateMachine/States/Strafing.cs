namespace Axiom.Player.Movement.StateMachine.States
{
    public class Strafing : State
    {
        public Strafing(MovementSystem movementSystem) : base(movementSystem)
        {
            stateName = StateName.Strafing;
        }

        public override void EnterState()
        {
            base.EnterState();

            MovementSystem.SetAnimatorBool("Strafing", true);
            MovementSystem.SetTargetSpeed(MovementSystem.strafeSpeed);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            if (MovementSystem.inputDetection.movementInput.x == 0f && MovementSystem.inputDetection.movementInput.z > 0) MovementSystem.ChangeState(MovementSystem._walkingState);
            else if (MovementSystem.inputDetection.movementInput.x == 0f && MovementSystem.inputDetection.movementInput.z <= 0) MovementSystem.ChangeState(MovementSystem._idleState);
            else if(MovementSystem.inputDetection.crouchInput) MovementSystem.ChangeState(MovementSystem._crouchingState);

            CalculateMovementSpeed(startSpeed: 0);
        }

        public override void PhysicsUpdate()
        {
        }

        public override void ExitState()
        {
            base.ExitState();
            
            MovementSystem.SetAnimatorBool("Strafing", false);
        }
    }
}