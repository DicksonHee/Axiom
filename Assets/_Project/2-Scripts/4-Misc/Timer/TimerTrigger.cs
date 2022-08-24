using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerTrigger : MonoBehaviour
{
	public bool isStart;
	public string sectionName;

	private Transform triggerPair;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			if (isStart) SpeedrunTimer.StartTimer();
			else SpeedrunTimer.EndTimer(sectionName);
		}
	}

	private void OnValidate()
	{
		TimerTrigger[] timers = FindObjectsOfType<TimerTrigger>();
		foreach (TimerTrigger timer in timers)
		{
			if (timer.sectionName == sectionName)
			{
				triggerPair = timer.transform;
				return;
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, GetComponent<SphereCollider>().radius);
		if(triggerPair != null) Gizmos.DrawLine(transform.position, triggerPair.position);
	}
}
