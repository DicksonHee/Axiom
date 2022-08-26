using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			RespawnManager.RequestRespawnPlayer();
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(transform.position + GetComponent<BoxCollider>().center, GetComponent<BoxCollider>().size);
	}
}
