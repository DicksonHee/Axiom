using System.Collections;
using System.Collections.Generic;
using Axiom.Player.Movement;
using Axiom.Player.StateMachine;
using UnityEngine;

namespace Axiom.Player.StateMachine
{
	public class WallRunning : State
	{
		private Transform wallTransform;
		private Vector3 wallNormal;
		private Vector3 wallForward;
		private Vector3 exitVelocity;
		private Vector3 initialYVel;
		private bool isRightWallEnter;
		private bool isJumpingOnExit;
		
		public WallRunning(MovementSystem movementSystem) : base(movementSystem)
		{
			stateName = StateName.WallRunning;
		}

		public override void EnterState()
		{
			base.EnterState();

			initialYVel = MovementSystem.GetCurrentUpForce();
			isRightWallEnter = MovementSystem.rbInfo.IsRightWallDetected();
			isJumpingOnExit = false;
			
			wallTransform = isRightWallEnter ? MovementSystem.rbInfo.GetRightWall() : MovementSystem.rbInfo.GetLeftWall();
			wallNormal = MovementSystem.rbInfo.IsRightWallDetected() ? MovementSystem.rbInfo.GetRightWallNormal() : MovementSystem.rbInfo.GetLeftWallNormal();
			wallForward = Vector3.Cross(wallNormal, MovementSystem.transform.up);
			if ((MovementSystem.orientation.forward - wallForward).magnitude > (MovementSystem.orientation.forward - -wallForward).magnitude) wallForward = -wallForward;

			if (isRightWallEnter) MovementSystem.cameraLook.StartRightWallRunCamera();
			else MovementSystem.cameraLook.StartLeftWallRunCamera();

			MovementSystem.DisableVerticalMovement();
			MovementSystem.DisableMovement();
			MovementSystem.rbInfo.SetIsOnWall(MovementSystem.orientation.right, MovementSystem.orientation.forward);
			MovementSystem.EnterWallRunState(wallTransform, wallNormal, isRightWallEnter);
			MovementSystem.SetGravity(MovementSystem.inAirGravity);
			
			MovementSystem.SetAnimatorBool("WallRunning", true);
			MovementSystem.playerAnimation.SetWallRunParam(isRightWallEnter ? 1 : -1);
			MovementSystem.playerAnimation.SetLandParam(isRightWallEnter ? 1 : -1);
		}

		public override void LogicUpdate()
		{
			base.LogicUpdate();

			if((isRightWallEnter && !MovementSystem.rbInfo.WallRunningRightDetected()) ||
			   (!isRightWallEnter && !MovementSystem.rbInfo.WallRunningLeftDetected()) ||
			   Vector3.Dot(MovementSystem.orientation.forward, wallNormal) >= 0.95f ||
			   Time.time - stateStartTime > MovementSystem.wallRunMaxDuration)
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
			if (!isJumpingOnExit)
			{
				Vector3 moveVel = MovementSystem.ProjectDirectionOnPlane(MovementSystem.moveDirection.normalized, MovementSystem.transform.up) * MovementSystem.wallRunSpeed;
				MovementSystem._rb.velocity = moveVel;
			}
			else
            {
				MovementSystem._rb.velocity = exitVelocity;
            }

			MovementSystem.rbInfo.SetIsOffWall();
			MovementSystem.cameraLook.ResetCamera();
			MovementSystem.EnableVerticalMovement();
			MovementSystem.EnableMovement();
			MovementSystem.ExitWallRunState();
			MovementSystem.SetAnimatorBool("WallRunning", false);
			
			base.ExitState();
		}

		private void WallRunningMovement()
		{
			//float yVel = Mathf.Lerp(initialYVel, 0f, (Time.time - stateStartTime) * 2);
			//Vector3 velocity = (wallForward + -wallNormal) * MovementSystem.wallRunSpeed;
			Vector3 moveVel = MovementSystem.ProjectDirectionOnPlane((wallForward + -wallNormal).normalized, MovementSystem.transform.up) * MovementSystem.wallRunSpeed;
			
			//velocity = new Vector3(velocity.x, yVel, velocity.z);
			MovementSystem._rb.velocity = moveVel;
		}

		public void SetIsJumpingOnExit(bool val, Vector3 exitVel)
		{
			isJumpingOnExit = val;
			exitVelocity = exitVel;
			MovementSystem.ChangeState(MovementSystem._inAirState);
		}
	}
}

