namespace Axiom.Player.Movement.StateMachine.States
{
    public class LedgeClimbing : State
    {
        public LedgeClimbing(MovementSystem movementSystem) : base(movementSystem)
        {
            stateName = StateName.LedgeClimbing;
        }
    }
}