using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Axiom.Core
{
	public static class PlayerMovementDetails
	{
		public static bool movementInputEnabled = true;
		public static bool cameraLookEnabled = true;

		public static void DisableAllMovementInput()
		{
			movementInputEnabled = false;
			cameraLookEnabled = false;
		}

		public static void EnableAllMovementInput()
		{
            movementInputEnabled = true;
            cameraLookEnabled = true;
        }
	}
}