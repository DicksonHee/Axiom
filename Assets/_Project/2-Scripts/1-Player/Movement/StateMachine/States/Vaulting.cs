using System.Collections;
using System.Collections.Generic;
using Axiom.Player.Movement;
using Axiom.Player.StateMachine;
using UnityEngine;

namespace Axiom.Player.StateMachine
{
    public class Vaulting : State
    {
		private Vector3 initialPos;
		private Vector3 initialDir;
		private float initialVelocity;

		public Vaulting(MovementSystem movementSystem) : base(movementSystem)
        {
            stateName = StateName.Vaulting;
        }

		public override void EnterState()
		{
			base.EnterState();

			initialPos = MovementSystem.transform.position;
			initialDir = MovementSystem.orientation.forward;
			initialVelocity = MovementSystem.GetCurrentSpeed();

			MovementSystem.DisableMovement();
			MovementSystem._rb.isKinematic = true;

			MovementSystem.cameraLook.StartVaultCamera();
			MovementSystem.playerAnimation.SetVaultHandPositions();
			MovementSystem.playerAnimation.DisableRotation();
			MovementSystem.playerAnimation.SetFloatParam("VaultHeight", MovementSystem.rbInfo.GetVaultHeight());
			
			if (MovementSystem.rbInfo.CanVaultOver())
			{
				MovementSystem.SetAnimatorBool("VaultingOver", true);

				MovementSystem.StartCoroutine(LerpForwardPosition_CO(Mathf.Lerp(0.1f, 0.5f, 1 - initialVelocity / 15f)));
			}
			else
			{
				MovementSystem.SetAnimatorBool("VaultingOn", true);

				MovementSystem.StartCoroutine(LerpUpwardPosition_CO(Mathf.Lerp(0.1f, 0.5f, 1 - initialVelocity / 15f)));
			}
		}

		private IEnumerator LerpForwardPosition_CO(float seconds)
		{
			float startTime = Time.time;
			while (Time.time - startTime < seconds)
			{
				MovementSystem.transform.position = Vector3.Lerp(initialPos, initialPos + initialDir * 2f, (Time.time - startTime) / seconds);
				yield return null;
			}

			MovementSystem.ChangeState(MovementSystem._idleState);
		}

		private IEnumerator LerpUpwardPosition_CO(float seconds)
		{
			float startTime = Time.time;
			while (Time.time - startTime < seconds)
			{
				MovementSystem.transform.position = Vector3.Lerp(initialPos, initialPos + (initialDir + MovementSystem.orientation.up).normalized * 2f, (Time.time - startTime) / seconds);
				yield return null;
			}

			MovementSystem.ChangeState(MovementSystem._idleState);
		}

		public override void ExitState()
		{
			base.ExitState();

			MovementSystem.EnableMovement();
			MovementSystem._rb.isKinematic = false;

			MovementSystem.cameraLook.ResetCamera();
			MovementSystem.playerAnimation.EnableRotation();
			MovementSystem.SetAnimatorBool("VaultingOn", false);
			MovementSystem.SetAnimatorBool("VaultingOver", false);
		}
	}
}