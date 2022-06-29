using System.Collections;
using System.Collections.Generic;
using Axiom.Player.Movement;
using Axiom.Player.StateMachine;
using UnityEngine;

namespace Axiom.Player.StateMachine
{
	public class WallRunning : State
	{
		private Vector3 wallNormal;
		private Vector3 wallForward;

		public WallRunning(MovementSystem movementSystem) : base(movementSystem)
		{
			stateName = StateName.WallRunning;
		}

		public override void EnterState(StateName prevState)
		{
			base.EnterState(prevState);

			MovementSystem.DisableMovement();
			MovementSystem.SetGravity(MovementSystem.inAirGravity);

			wallNormal = MovementSystem.rbInfo.rightWallDetected ? MovementSystem.rbInfo.rightWallHit.normal : MovementSystem.rbInfo.leftWallHit.normal;
			wallForward = Vector3.Cross(wallNormal, MovementSystem.transform.up);
			if ((MovementSystem.orientation.forward - wallForward).magnitude > (MovementSystem.orientation.forward - -wallForward).magnitude) wallForward = -wallForward;
		}

		public override void ExitState()
		{
			base.ExitState();

			MovementSystem.EnableMovement();
		}

		public override void LogicUpdate()
		{
			base.LogicUpdate();

			if (MovementSystem.inputDetection.movementInput.magnitude == 0 || MovementSystem.rbInfo.isGrounded) MovementSystem.ChangeState(MovementSystem._idleState);
			else if (MovementSystem.inputDetection.movementInput.x == 0) MovementSystem.ChangeState(MovementSystem._inAirState);
		}

		public override void PhysicsUpdate()
		{
			base.PhysicsUpdate();

			WallRunningMovement();
		}

		private void WallRunningMovement()
		{
			float climbVel = Time.time - stateStartTime < 0.5f ? 6f : -3f;
			MovementSystem._rb.velocity = wallForward * MovementSystem.walkSpeed;
			MovementSystem._rb.velocity = new Vector3(MovementSystem._rb.velocity.x, climbVel, MovementSystem._rb.velocity.z);
		}
	}
}

