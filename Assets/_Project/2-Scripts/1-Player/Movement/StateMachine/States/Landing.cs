using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Axiom.Player.Movement.StateMachine.States
{
    public class Landing : State
    {
		private float delayTime;
		private bool rollSuccess;
		
		public Landing(MovementSystem movementSystem) : base(movementSystem)
        {
            stateName = StateName.Landing;
        }

		public override void EnterState()
		{
			base.EnterState();

			MovementSystem.SetGravity(MovementSystem.groundGravity);
			MovementSystem.SetTargetSpeed(MovementSystem.walkSpeed);
			
			if (MovementSystem.TotalAirTime < 0.75f)
			{
				delayTime = 0.1f;
				MovementSystem.playerAnimation.SetFloatParam("LandIntensity", 0);
			}
			else if (MovementSystem.TotalAirTime < 1.5f)
			{
				delayTime = 0.1f;
				MovementSystem.playerAnimation.SetFloatParam("LandIntensity", 1);
			}
			else if (MovementSystem.TotalAirTime >= 1.5f)
			{ 
				rollSuccess = Mathf.Abs(stateStartTime - MovementSystem.inputDetection.crouchPressedTime) < 1f;
				delayTime = rollSuccess ? 0.3f : 1.5f;

				if (rollSuccess)
				{
					MovementSystem.playerAnimation.DisableRotation();
					//MovementSystem.playerAnimation.HideModel();
					MovementSystem.cameraLook.StartRollCamera();
				}
				else
				{
					MovementSystem.playerAnimation.DisableRotation();
					MovementSystem.DisableMovement();
					MovementSystem.cameraLook.StartHardLandingCamera();
					MovementSystem.Rb.velocity = Vector3.zero;
				}
				MovementSystem.playerAnimation.SetFloatParam("LandIntensity", 2);
				MovementSystem.playerAnimation.SetFloatParam("LandSuccess", rollSuccess ? 1 : 0);
			}

			MovementSystem.playerAnimation.ResetTrigger("WallJump");
			MovementSystem.playerAnimation.ResetTrigger("Jump");
			MovementSystem.playerAnimation.SetInAirParam(0);
			MovementSystem.SetAnimatorBool("Landing", true);
		}

		public override void LogicUpdate()
		{
			base.LogicUpdate();

			if (Time.time - stateStartTime >= delayTime)
			{
				if(MovementSystem.inputDetection.movementInput.z == 0 || delayTime > 0.5f) MovementSystem.ChangeState(MovementSystem.IdleState);
				else if (Mathf.Abs(MovementSystem.inputDetection.movementInput.z) > 0)
				{
					if(MovementSystem.LastForwardState == MovementSystem.RunningState) MovementSystem.ChangeState(MovementSystem.RunningState);
					else MovementSystem.ChangeState(MovementSystem.WalkingState);
				}
			}
		}

		public override void ExitState()
		{
			base.ExitState();

			MovementSystem.SetTotalAirTime(0f);
			MovementSystem.SetAnimatorBool("Landing", false);
			MovementSystem.playerAnimation.EnableRotation();
			
			if (rollSuccess)
			{
				MovementSystem.cameraLook.ResetCamera();
			}
			else
			{
				MovementSystem.EnableMovement();
			}
		}
	}
}
