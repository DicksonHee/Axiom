using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RespawnManager
{
	private static string currentAreaGUID;
	private static GameObject playerGO;

	public static event Action<string, Vector3, Vector3> OnRequestRespawn;

	public static void SetCurrentAreaGUID(string guid) => currentAreaGUID = guid;

	public static void RequestRespawnPlayer()
	{
		if (playerGO == null) playerGO = GameObject.FindGameObjectWithTag("Player");
		OnRequestRespawn?.Invoke(currentAreaGUID, Vector3.zero, Vector3.zero);
	}

	public static void RespawnPlayer(Vector3 position, Vector3 rotation)
	{
		playerGO.transform.SetPositionAndRotation(position, Quaternion.Euler(rotation));
	}
}
