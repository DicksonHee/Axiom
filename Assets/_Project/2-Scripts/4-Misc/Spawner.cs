using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject spawnObject;
    [SerializeField] private Transform spawnPos;
    [SerializeField] private Transform spawnDest;
    [SerializeField] private float speed;
	
	private void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
			var clone = Instantiate(spawnObject, spawnPos);
			clone.GetComponent<Rigidbody>().AddForce(spawnDest.position - spawnPos.position * speed, ForceMode.Impulse);
			Destroy(clone, 10);
			gameObject.GetComponent<Collider>().enabled = false;
		}
	}
}
