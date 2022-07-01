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
		private float initialYVel;

		public WallRunning(MovementSystem movementSystem) : base(movementSystem)
		{
			stateName = StateName.WallRunning;
		}

		public override void EnterState(StateName prevState)
		{
			base.EnterState(prevState);

			MovementSystem.DisableMovement();
			MovementSystem.SetGravity(MovementSystem.inAirGravity);

			initialYVel = MovementSystem._rb.velocity.y;
			wallNormal = MovementSystem.rbInfo.rightWallDetected ? MovementSystem.rbInfo.rightWallHit.normal : MovementSystem.rbInfo.leftWallHit.normal;
			wallForward = Vector3.Cross(wallNormal, MovementSystem.transform.up);
			if ((MovementSystem.orientation.forward - wallForward).magnitude > (MovementSystem.orientation.forward - -wallForward).magnitude) wallForward = -wallForward;
		}

		public override void LogicUpdate()
		{
			base.LogicUpdate();

			if (MovementSystem.inputDetection.movementInput.z == 0 || 
			    MovementSystem.inputDetection.movementInput.x == 0 ||
			    MovementSystem.inputDetection.movementInput.x < 0 && MovementSystem.rbInfo.rightWallDetected ||
			    MovementSystem.inputDetection.movementInput.x > 0 && MovementSystem.rbInfo.leftWallDetected ||
			    !MovementSystem.rbInfo.rightWallDetected && !MovementSystem.rbInfo.leftWallDetected)
			{
				MovementSystem.ChangeState(MovementSystem._inAirState);
			}
			else if(MovementSystem.rbInfo.isGrounded) MovementSystem.ChangeState(MovementSystem._idleState);
		}

		public override void PhysicsUpdate()
		{
			base.PhysicsUpdate();

			WallRunningMovement();
		}
		
		public override void ExitState()
		{
			MovementSystem.EnableMovement();
			MovementSystem.ExitWallRunState(wallNormal);

			base.ExitState();
		}

		private void WallRunningMovement()
		{
			float climbVel = Mathf.Lerp(initialYVel, -5f,MovementSystem.wallRunCurve.Evaluate(Time.time - stateStartTime));
			Vector3 velocity = wallForward * MovementSystem.walkSpeed;
			velocity = new Vector3(velocity.x, climbVel, velocity.z);
			MovementSystem._rb.velocity = velocity;
		}
	}
}

