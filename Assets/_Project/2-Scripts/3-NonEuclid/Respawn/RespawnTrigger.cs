using System;
using System.Collections;
using System.Collections.Generic;
using Axiom.Core;
using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
	public bool triggerOnEnter;
	public bool triggerOnExit;
	public GameObject thisAreaPortal;
	
	[HideInInspector]public bool isEnabled { get; set; }
	[HideInInspector]public bool isPlayerInDoor { get; set; }

	private void Start()
	{
		InvokeRepeating(nameof(CheckCurrentActiveArea), 1f, 1f);
	}

	private void CheckCurrentActiveArea()
	{
		if (isPlayerInDoor) return;
		
		if (this == RespawnManager.currentRespawnTrigger)
		{
			isEnabled = true;
			if(thisAreaPortal != null) thisAreaPortal.SetActive(true);
		}
		else
		{
			isEnabled = false;
			if(thisAreaPortal != null) thisAreaPortal.SetActive(false);
		}
	}
	
	private void OnTriggerEnter(Collider other)
	{
		RespawnManager.currentRespawnTrigger = this;
		if (triggerOnExit || !isEnabled) return;
		
		if (other.CompareTag("Player"))
		{
            Debug.Log("Enter");
            PostProcessingActions.current.RespawnAnimation(1f);
			Invoke(nameof(RespawnPlayer), 0.5f);
		}
	}
	
	private void OnTriggerExit(Collider other)
	{
		if (triggerOnEnter || !isEnabled) return;
		
		if (other.CompareTag("Player"))
		{
			Debug.Log("Exit");
			PostProcessingActions.current.RespawnAnimation(1f);
			Invoke(nameof(RespawnPlayer), 0.5f);
		}
	}

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.R)) RespawnPlayer();
	}

	private void RespawnPlayer()
	{
		RespawnManager.RequestRespawnPlayer();
	}
}
