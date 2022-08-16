using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Axiom.Player.Movement.StateMachine.States
{
    public class Landing : State
    {
		private float delayTime;
		private float enterSpeed;

        public Landing(MovementSystem movementSystem) : base(movementSystem)
        {
            stateName = StateName.Landing;
        }

		public override void EnterState()
		{
			base.EnterState();

			enterSpeed = MovementSystem.GetCurrentSpeed();
			
			MovementSystem.SetGravity(MovementSystem.groundGravity);
			MovementSystem.SetTargetSpeed(MovementSystem.walkSpeed);
			
			if (MovementSystem.totalAirTime < 0.75f)
			{
				delayTime = 0.1f;
				MovementSystem.playerAnimation.SetFloatParam("LandIntensity", 0);
			}
			else if (MovementSystem.totalAirTime < 1.5f)
			{
				delayTime = 0.25f;
				MovementSystem.playerAnimation.SetFloatParam("LandIntensity", 1);
			}
			else if (MovementSystem.totalAirTime >= 1.5f)
			{ 
				bool rollSuccess = Mathf.Abs(stateStartTime - MovementSystem.inputDetection.crouchPressedTime) < 1f;
				delayTime = rollSuccess ? 0.6f : 1.5f;

				if (rollSuccess)
				{
					MovementSystem.playerAnimation.DisableRotation();
					MovementSystem.cameraLook.StartRollCamera();
					enterSpeed = 16f;
				}
				else
				{
					MovementSystem.DisableMovement();
					MovementSystem.cameraLook.StartHardLandingCamera(30f);
					MovementSystem.rb.velocity = Vector3.zero;
					enterSpeed = 0f;
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
				if(MovementSystem.inputDetection.movementInput.z == 0) MovementSystem.ChangeState(MovementSystem._idleState);
				else if (Mathf.Abs(MovementSystem.inputDetection.movementInput.z) > 0)
				{
					if(enterSpeed > 15f) MovementSystem.ChangeState(MovementSystem._runningState);
					else MovementSystem.ChangeState(MovementSystem._walkingState);
				}
			}
			else
			{
				CalculateMovementSpeed();
			}
		}

		public override void ExitState()
		{
			base.ExitState();

			MovementSystem.totalAirTime = 0f;
			MovementSystem.EnableMovement();
			MovementSystem.playerAnimation.EnableRotation();
			MovementSystem.cameraLook.ResetCamera();
			MovementSystem.SetAnimatorBool("Landing", false);
		}
	}
}
