using UnityEngine;

namespace Axiom.Player.Movement.StateMachine.States
{
	public class WallRunning : State
	{
		private Transform wallTransform;
		private Vector3 wallNormal;
		private Vector3 wallForward;
		private Vector3 exitVelocity;
		
		private float initialVertVel;
		private float initialHoriVel;

		private bool isRightWallEnter;
		private bool isJumpingOnExit;
		
		public WallRunning(MovementSystem movementSystem) : base(movementSystem)
		{
			stateName = StateName.WallRunning;
		}

		public override void EnterState()
		{
			base.EnterState();
			
			initialVertVel = Vector3.Dot(MovementSystem.upDirection, MovementSystem.rb.velocity);
			initialHoriVel = MovementSystem.GetCurrentSpeed();
			isRightWallEnter = MovementSystem.rbInfo.IsRightWallDetected();
			isJumpingOnExit = false;
			
			wallTransform = isRightWallEnter ? MovementSystem.rbInfo.GetRightWall() : MovementSystem.rbInfo.GetLeftWall();
			wallNormal = MovementSystem.rbInfo.IsRightWallDetected() ? MovementSystem.rbInfo.GetRightWallNormal() : MovementSystem.rbInfo.GetLeftWallNormal();
			wallForward = Vector3.Cross(wallNormal, MovementSystem.upDirection);
			if ((MovementSystem.forwardDirection - wallForward).magnitude > (MovementSystem.forwardDirection - -wallForward).magnitude) wallForward = -wallForward;

			if (isRightWallEnter) MovementSystem.cameraLook.StartRightWallRunCamera();
			else MovementSystem.cameraLook.StartLeftWallRunCamera();
			
			MovementSystem.DisableMovement();
			MovementSystem.rbInfo.SetIsOnWall(MovementSystem.rightDirection, MovementSystem.forwardDirection);
			MovementSystem.EnterWallRunState(wallTransform, wallNormal, isRightWallEnter);
			MovementSystem.SetGravity(0);
			
			MovementSystem.SetAnimatorBool("WallRunning", true);
			MovementSystem.playerAnimation.SetWallRunParam(isRightWallEnter ? 1 : -1);
			MovementSystem.playerAnimation.SetLandParam(isRightWallEnter ? 1 : -1);
		}

		public override void LogicUpdate()
		{
			base.LogicUpdate();
			
			if((isRightWallEnter && !MovementSystem.rbInfo.WallRunningRightDetected()) ||
			   (!isRightWallEnter && !MovementSystem.rbInfo.WallRunningLeftDetected()) ||
			   Vector3.Dot(MovementSystem.forwardDirection, wallNormal) >= 0.95f ||
			   Vector3.Dot(wallForward, MovementSystem.forwardDirection) <= -0.25f ||
			   Time.time - stateStartTime > MovementSystem.wallRunMaxDuration)
			{
				MovementSystem.ChangeState(MovementSystem._inAirState);
			}
			else if(MovementSystem.rbInfo.IsGrounded()) MovementSystem.ChangeState(MovementSystem._idleState);
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
				Vector3 moveVel = MovementSystem.forwardDirection.normalized * MovementSystem.wallRunSpeed;
				MovementSystem.rb.velocity = moveVel;
			}
			else
            {
				MovementSystem.rb.velocity = exitVelocity;
            }

			MovementSystem.rbInfo.SetIsOffWall();
			MovementSystem.cameraLook.ResetCamera();
			MovementSystem.EnableMovement();
			MovementSystem.ExitWallRunState();
			MovementSystem.SetAnimatorBool("WallRunning", false);
			
			base.ExitState();
		}

		private void WallRunningMovement()
		{
			float lerpMultiplier = 2f;
			if(initialVertVel < 0) lerpMultiplier = 3f;
			
			float verticalVel = Mathf.Lerp(initialVertVel, 0f, (Time.time - stateStartTime) * lerpMultiplier);
			float horizontalVel = Mathf.Lerp(initialHoriVel, MovementSystem.wallRunSpeed, (Time.time - stateStartTime) * lerpMultiplier);
			
			Vector3 moveVel = Vector3.ProjectOnPlane(wallForward, MovementSystem.upDirection) * horizontalVel;
			Vector3 stickToWallVel = Vector3.ProjectOnPlane(-wallNormal, MovementSystem.upDirection) * 20f;
			MovementSystem.rb.velocity = moveVel + stickToWallVel + MovementSystem.upDirection * verticalVel;
		}

		public void SetIsJumpingOnExit(bool val, Vector3 exitVel)
		{
			isJumpingOnExit = val;
			exitVelocity = exitVel;
			MovementSystem.ChangeState(MovementSystem._inAirState);
		}
	}
}

