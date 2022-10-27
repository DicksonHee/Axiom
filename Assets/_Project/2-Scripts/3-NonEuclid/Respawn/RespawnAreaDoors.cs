using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnAreaDoors : MonoBehaviour
{
	public RespawnTrigger thisAreaTrigger;

	private void OnTriggerEnter(Collider other)
	{
		thisAreaTrigger.isPlayerInDoor = true;
		thisAreaTrigger.isEnabled = false;
	}

	private void OnTriggerExit(Collider other)
	{
		thisAreaTrigger.isPlayerInDoor = false;
	}
}
