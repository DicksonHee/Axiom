using System.Collections;
using System.Collections.Generic;
using Axiom.Player.Movement;
using Axiom.Player.StateMachine;
using UnityEngine;

namespace Axiom.Player.StateMachine
{
    public class Vaulting : State
    {
		Vector3 initialPos;
		Vector3 initialDir;
		float initialVelocity;

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
			MovementSystem.playerAnimation.DisableRotation();
			MovementSystem.playerAnimation.SetFloatParam("VaultHeight", 1.1f);
			MovementSystem.SetAnimatorBool("Vaulting", true);

			MovementSystem.StartCoroutine(LerpPosition_CO(Mathf.Lerp(0.1f, 0.5f, 1 - initialVelocity / 15f)));
		}

		private IEnumerator LerpPosition_CO(float seconds)
		{
			while (Time.time - stateStartTime < seconds)
			{
				MovementSystem.transform.position = Vector3.Lerp(initialPos, initialPos + initialDir * 2f, (Time.time - stateStartTime) / seconds);
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
			MovementSystem.SetAnimatorBool("Vaulting", false);
		}
	}
}