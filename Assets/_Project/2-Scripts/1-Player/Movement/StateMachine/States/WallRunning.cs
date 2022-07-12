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
		private bool isRightWallEnter;
		
		public WallRunning(MovementSystem movementSystem) : base(movementSystem)
		{
			stateName = StateName.WallRunning;
		}

		public override void EnterState()
		{
			base.EnterState();

			initialYVel = MovementSystem._rb.velocity.y;
			isRightWallEnter = MovementSystem.rbInfo.rightWallDetected;
			wallNormal = MovementSystem.rbInfo.rightWallDetected ? MovementSystem.rbInfo.rightWallHit.normal : MovementSystem.rbInfo.leftWallHit.normal;
			wallForward = Vector3.Cross(wallNormal, MovementSystem.transform.up);
			if ((MovementSystem.orientation.forward - wallForward).magnitude > (MovementSystem.orientation.forward - -wallForward).magnitude) wallForward = -wallForward;

			MovementSystem.cameraLook.ChangeFov(110f);
			MovementSystem.cameraLook.ChangeTilt(isRightWallEnter ? 10f : -10f);
			
			MovementSystem.DisableMovement();
			MovementSystem.EnterWallRunState(wallNormal, isRightWallEnter);
			MovementSystem.SetAnimatorBool("WallRunning", true);
			MovementSystem.SetAnimatorFloat("WallRunType", isRightWallEnter ? 1 : -1);
			MovementSystem.SetGravity(MovementSystem.inAirGravity);
		}

		public override void LogicUpdate()
		{
			base.LogicUpdate();

			if (MovementSystem.inputDetection.movementInput.z == 0 ||
			    !isRightWallEnter && MovementSystem.rbInfo.rightWallDetected ||
			    isRightWallEnter && MovementSystem.rbInfo.leftWallDetected ||
			    !MovementSystem.rbInfo.rightWallDetected && !MovementSystem.rbInfo.leftWallDetected ||
			    Vector3.Dot(MovementSystem.orientation.forward, wallNormal) > 0.5f ||
			    Time.time - stateStartTime > 1f)
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
			MovementSystem.cameraLook.ResetFov();
			MovementSystem.cameraLook.ResetTilt();
			
			MovementSystem.EnableMovement();
			MovementSystem.ExitWallRunState();
			MovementSystem.SetAnimatorBool("WallRunning", false);
			
			base.ExitState();
		}

		private void WallRunningMovement()
		{
			Vector3 velocity = wallForward * MovementSystem.wallRunSpeed;
			velocity = new Vector3(velocity.x, 0f, velocity.z);
			MovementSystem._rb.velocity = velocity;
		}
	}
}

