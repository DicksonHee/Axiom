using UnityEngine;

namespace Axiom.Player.Movement.StateMachine.States
{
    public class LedgeGrabbing : State
    {
		private bool isJumpingOnExit;
		private Vector3 exitVelocity;

        public LedgeGrabbing(MovementSystem movementSystem) : base(movementSystem)
        {
            stateName = StateName.LedgeGrabbing;
        }

		public override void EnterState()
		{
			base.EnterState();

			MovementSystem.rb.velocity = Vector3.zero;
			MovementSystem.ChangeState(MovementSystem._inAirState);
		}

		public override void ExitState()
		{
			base.ExitState();
			MovementSystem.rb.velocity = MovementSystem.transform.up * (MovementSystem.wallRunJumpUpForce * 1.5f) + MovementSystem.forwardDirection.normalized * MovementSystem.wallRunJumpSideForce;
		}
    }
}