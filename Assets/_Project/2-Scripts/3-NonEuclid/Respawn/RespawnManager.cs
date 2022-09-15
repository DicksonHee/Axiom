using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RespawnManager
{
	private static GameObject playerGO;
	private static RespawnArea currentRespawnArea;

	public static void SetCurrentRespawnArea(RespawnArea newArea) => currentRespawnArea = newArea;

	public static void RequestRespawnPlayer()
	{
		if (playerGO == null) playerGO = GameObject.FindGameObjectWithTag("Player");
		
		currentRespawnArea.RespawnPlayer(playerGO);
	}
}