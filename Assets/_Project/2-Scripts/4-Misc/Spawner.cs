using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject spawnObject;
    [SerializeField] private Transform spawnPos;
    [SerializeField] private Transform spawnDest;
    [SerializeField] private float speed;

	public int spawnerID;
	public GameObject destructiblePlatform;

	private bool hasActivated;

    private void OnEnable()
    {
        RespawnManager.OnRespawn += SetHasNotActivated;
    }

    private void OnDisable()
    {
        RespawnManager.OnRespawn -= SetHasNotActivated;
    }

    public void Reset()
	{
		destructiblePlatform.SetActive(true);
	}

	private void SetHasActivated() => hasActivated = true;
	private void SetHasNotActivated() => hasActivated = false;

	public void SpawnObject()
	{
		if (hasActivated) return;

        //var clone = Instantiate(spawnObject, spawnPos.position, Quaternion.identity);
        spawnObject.transform.position = spawnPos.position;
        spawnObject.SetActive(true);

        Vector3 newSpawnPos = spawnPos.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)) * 10f;
        Vector3 targetPos = spawnDest.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)) * 10f;

        Rigidbody rb = spawnObject.GetComponent<Rigidbody>();
        rb.velocity = (targetPos - newSpawnPos).normalized * speed;
        rb.AddTorque(Vector3.one * 200f, ForceMode.Impulse);
        //Destroy(clone, 2);
        hasActivated = true;
        //spawnObject.SetActive(false);
    }

	private void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
			SpawnObject();
		}
	}
}
