using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RespawnManager
{
	private static GameObject playerGO;
	private static RespawnArea currentRespawnArea;
	public static RespawnTrigger currentRespawnTrigger { get; set; }

	public delegate void RespawnEvent();
	public static event RespawnEvent OnRespawn;
	public static event RespawnEvent OnPlayRespawnSound;

	public static void SetCurrentRespawnArea(RespawnArea newArea) => currentRespawnArea = newArea;
	public static RespawnArea GetCurrentRespawnArea() => currentRespawnArea;

	public static void InvokeOnPlayRespawnSound() => OnPlayRespawnSound?.Invoke();
	
	public static void RequestRespawnPlayer()
	{
		if (playerGO == null) playerGO = GameObject.FindGameObjectWithTag("Player");
		
		currentRespawnArea.RespawnPlayer(playerGO);
		OnRespawn?.Invoke();
	}
}