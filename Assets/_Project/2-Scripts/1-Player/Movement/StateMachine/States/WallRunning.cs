using UnityEngine;

namespace Axiom.Player.Movement.StateMachine.States
{
	public class WallRunning : State
	{
		private Transform wallTransform;
		private Vector3 wallNormal;
		private Vector3 wallForward;
		private Vector3 exitVelocity;
		private Vector3 moveVel;
		private Vector3 stickToWallVel;
		
		private float initialVertVel;
		private float initialHoriVel;
		private float exitCounter;
		private float stickToWallMultiplier;
		
		private bool isRightWallEnter;
		private bool isJumpingOnExit;
		
		public WallRunning(MovementSystem movementSystem) : base(movementSystem)
		{
			stateName = StateName.WallRunning;
		}

		public override void EnterState()
		{
			base.EnterState();

			// Get initial movement values right before the player enters wall run
			initialVertVel = Vector3.Dot(MovementSystem.UpDirection, MovementSystem.Rb.velocity);
			initialHoriVel = MovementSystem.GetCurrentSpeed();
			isRightWallEnter = MovementSystem.rbInfo.IsRightWallDetected();
			isJumpingOnExit = false;
			
			// Set the wall values
			wallTransform = isRightWallEnter ? MovementSystem.rbInfo.GetRightWall() : MovementSystem.rbInfo.GetLeftWall();
			wallNormal = MovementSystem.rbInfo.IsRightWallDetected() ? MovementSystem.rbInfo.GetRightWallNormal() : MovementSystem.rbInfo.GetLeftWallNormal();
			wallForward = Vector3.Cross(wallNormal, MovementSystem.UpDirection);
			if ((MovementSystem.ForwardDirection - wallForward).magnitude > (MovementSystem.ForwardDirection - -wallForward).magnitude) wallForward = -wallForward;

			// Set movement direction
			moveVel = Vector3.ProjectOnPlane(wallForward, MovementSystem.UpDirection);
			stickToWallVel = Vector3.ProjectOnPlane(-wallNormal, MovementSystem.UpDirection) * 20f;
			
			// Set camera values to move camera to the left or right depending on wall
			if (isRightWallEnter) MovementSystem.cameraLook.StartRightWallRunCamera(wallTransform.gameObject);
			else MovementSystem.cameraLook.StartLeftWallRunCamera(wallTransform.gameObject);

			MovementSystem.DisableMovement();
			MovementSystem.rbInfo.SetIsOnWall(MovementSystem.RightDirection, MovementSystem.ForwardDirection);
			MovementSystem.EnterWallRunState(wallTransform, wallNormal, isRightWallEnter);
			MovementSystem.SetGravity(0);
			
			MovementSystem.SetAnimatorBool("WallRunning", true);
			MovementSystem.playerAnimation.SetWallRunParam(isRightWallEnter ? 1 : -1);
		}

		public override void LogicUpdate()
		{
			base.LogicUpdate();

			if ((isRightWallEnter && !MovementSystem.rbInfo.WallRunningRightDetected()) ||
			    (!isRightWallEnter && !MovementSystem.rbInfo.WallRunningLeftDetected()) ||
			    Vector3.Dot(MovementSystem.ForwardDirection, wallNormal) >= 0.96f ||
			    Vector3.Dot(wallForward, MovementSystem.ForwardDirection) <= -0.25f ||
			    Time.time - stateStartTime > MovementSystem.wallRunMaxDuration)
			{
				Debug.Log("EnterWall: " + isRightWallEnter + "\n" + 
				          "Left: " + MovementSystem.rbInfo.WallRunningLeftDetected() + "\n" +
				          "Right: " + MovementSystem.rbInfo.WallRunningRightDetected() + "\n" +
				          "WallNormal: " + Vector3.Dot(MovementSystem.ForwardDirection, wallNormal) + "\n" +
				          "ForwardDir: " + Vector3.Dot(wallForward, MovementSystem.ForwardDirection));
				stickToWallMultiplier = 0f;
				exitCounter -= Time.deltaTime;
			}
			else if(Vector3.Dot(wallNormal, MovementSystem.ForwardDirection) < -0.9f)
			{
				MovementSystem.ChangeState(MovementSystem.ClimbingState);
			}
			else if (MovementSystem.rbInfo.IsGrounded()) MovementSystem.ChangeState(MovementSystem.IdleState);
			else
			{
				exitCounter = MovementSystem.wallRunCoyoteTime;
				stickToWallMultiplier = 1f;
			}
			
			if (exitCounter <= 0f) MovementSystem.ChangeState(MovementSystem.InAirState);
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
				Vector3 moveVel = MovementSystem.ForwardDirection.normalized * MovementSystem.wallRunSpeed;
				MovementSystem.Rb.velocity = moveVel;
			}
			else
            {
				MovementSystem.Rb.velocity = exitVelocity;
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
			float lerpMultiplier = 1.5f;
			if(initialVertVel < 0) lerpMultiplier = 2f;
			
			float verticalVel = Mathf.Lerp(initialVertVel, 0f, (Time.time - stateStartTime) * lerpMultiplier);
			float horizontalVel = Mathf.Lerp(initialHoriVel, MovementSystem.wallRunSpeed, (Time.time - stateStartTime) * lerpMultiplier);

			MovementSystem.Rb.velocity = moveVel * horizontalVel + stickToWallVel * stickToWallMultiplier + MovementSystem.UpDirection * verticalVel;
		}

		public void SetIsJumpingOnExit(bool val, Vector3 exitVel)
		{
			isJumpingOnExit = val;
			exitVelocity = exitVel;
			MovementSystem.ChangeState(MovementSystem.InAirState);
		}
	}
}

