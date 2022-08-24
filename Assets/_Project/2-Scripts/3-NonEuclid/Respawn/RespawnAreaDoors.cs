using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnAreaDoors : MonoBehaviour
{
	public RespawnArea respawnArea;
    public bool isEntranceDoor;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
		}
	}
}
