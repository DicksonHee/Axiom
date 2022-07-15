using System.Collections;
using System.Collections.Generic;
using Axiom.Player.Movement;
using Axiom.Player.StateMachine;
using UnityEngine;

namespace Axiom.Player.StateMachine
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

			isJumpingOnExit = false;
			exitVelocity = Vector3.zero;

			MovementSystem.DisableMovement();
			MovementSystem.SetGravity(0f);

			MovementSystem.SetAnimatorBool("LedgeGrabbing", true);
		}

		public override void LogicUpdate()
		{
			base.LogicUpdate();

			if (Time.time - stateStartTime > 1.25f)
			{
				if (MovementSystem.inputDetection.movementInput.z > 0f && MovementSystem.rbInfo.canWallClimb) MovementSystem.ChangeState(MovementSystem._ledgeClimbingState);
			}
		}

		public override void PhysicsUpdate()
		{
			base.PhysicsUpdate();

			MovementSystem._rb.velocity = Vector3.zero;
		}

		public override void ExitState()
		{
			base.ExitState();

			if (isJumpingOnExit)
			{
				MovementSystem._rb.velocity = exitVelocity;
			}

			MovementSystem.EnableMovement();
			MovementSystem.ExitLedgeGrabState();

			MovementSystem.SetAnimatorBool("LedgeGrabbing", false);
		}

		public void SetIsJumpingOnExit(bool val, Vector3 exitVel)
		{
			isJumpingOnExit = val;
			exitVelocity = exitVel;
			MovementSystem.ChangeState(MovementSystem._inAirState);
		}
	}
}