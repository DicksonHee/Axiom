using System;
using System.Collections;
using System.Collections.Generic;
using Axiom.Player.Movement.StateMachine;
using DG.Tweening;
using UnityEngine;

public class RespawnArea : MonoBehaviour
{
	public Transform spawnPosition;

	private Quaternion forwardLookDirection;
	private Vector3 gravityDirection;
	
	private void Awake()
	{
		forwardLookDirection = Quaternion.LookRotation(spawnPosition.forward, spawnPosition.up);
		gravityDirection = -spawnPosition.up.normalized * Physics.gravity.magnitude;
	}

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.R) && RespawnManager.GetCurrentRespawnArea() == this) RespawnManager.RequestRespawnPlayer();
	}

	public void RespawnPlayer(GameObject playerGO)
	{
		playerGO.GetComponent<MovementSystem>().TeleportPlayerRotateTo(spawnPosition.position, forwardLookDirection, gravityDirection);
	}
	
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			RespawnManager.SetCurrentRespawnArea(this);
		}
	}

	private void OnDrawGizmos()
	{
		SphereCollider sphereCollider = GetComponent<SphereCollider>();
		
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, sphereCollider.radius);
		
		DrawArrow.ForGizmo(spawnPosition.position, spawnPosition.forward, Color.red);
		DrawArrow.ForGizmo(spawnPosition.position, -spawnPosition.up, Color.blue);
	}
}
