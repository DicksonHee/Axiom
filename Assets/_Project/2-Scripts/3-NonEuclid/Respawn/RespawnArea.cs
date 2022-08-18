using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnArea : MonoBehaviour
{
	public Transform defaultRespawnPoint;
	public Vector3 defaultPlayerRotation;

	private Bounds colliderBounds;
	private GameObject playerGO;

	private bool isActive;
	private string respawnAreaGUID;

	private void Awake()
	{
		colliderBounds = GetComponent<BoxCollider>().bounds;
		respawnAreaGUID = System.Guid.NewGuid().ToString();

		RespawnManager.OnRequestRespawn += CheckRespawnPoint;
	}

	private void CheckRespawnPoint(string guid, Vector3 lastGroundedPosition, Vector3 lastGroundedRotation)
	{
		if (respawnAreaGUID == guid && IsWithinBounds(lastGroundedPosition))
		{
			RespawnManager.RespawnPlayer(lastGroundedPosition, lastGroundedRotation);
		}
	}

	private bool IsWithinBounds(Vector3 position) => colliderBounds.Contains(position);

	public void SetIsActive(bool val)
	{
		isActive = val;
		if (isActive)
		{
			RespawnManager.SetCurrentAreaGUID(respawnAreaGUID);
		}
		else
		{
			RespawnManager.SetCurrentAreaGUID(null);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			if (playerGO == null) playerGO = GameObject.FindGameObjectWithTag("Player");
			//playerGO.transform.SetPositionAndRotation(position, Quaternion.Euler(rotation));
		}
	}
}
