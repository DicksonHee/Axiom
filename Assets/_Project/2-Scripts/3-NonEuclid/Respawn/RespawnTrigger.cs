using System.Collections;
using System.Collections.Generic;
using Axiom.Core;
using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			PostProcessingActions.current.RespawnAnimation(1f);
			Invoke(nameof(RespawnPlayer), 0.5f);
		}
	}

	private void RespawnPlayer()
	{
		RespawnManager.RequestRespawnPlayer();
	}
	
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(transform.position + GetComponent<BoxCollider>().center, GetComponent<BoxCollider>().size);
	}
}
