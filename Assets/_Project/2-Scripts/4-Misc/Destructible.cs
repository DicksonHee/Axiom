using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Destructible : MonoBehaviour
{
	//[SerializeField] private GameObject brokenAlt;
	//[SerializeField] private Vector3 scaleSize;
	//private void OnCollisionEnter(Collision collision)
	//{
	//	Instantiate(brokenAlt, transform.position, transform.rotation);
	//	brokenAlt.transform.localScale = new Vector3(scaleSize.x, scaleSize.y, scaleSize.z);
	//	Destroy(gameObject);
	//}

    private Rigidbody Rigidbody;
    private AudioSource AudioSource;
    [SerializeField]
    private GameObject BrokenPrefab;
    [SerializeField]
    private AudioClip DestructionClip;
    [SerializeField]
    private float ExplosiveForceMin = 50;
    [SerializeField]
    private float ExplosiveForceMax = 100;
    [SerializeField]
    private float ExplosiveRadius = 5;
    //[SerializeField]
    //private float PieceFadeSpeed = 0.25f;
    //[SerializeField]
    //private float PieceDestroyDelay = 5f;
    //[SerializeField]
    //private float PieceSleepCheckDelay = 0.1f;
    [SerializeField]
    private float shardScaleFactor = 0.01f;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        AudioSource = GetComponent<AudioSource>();
    }

	private void OnCollisionEnter(Collision collision)
	{
		if(collision.collider.CompareTag("Destructible"))
		{
            Explode();
		}
	}

    public void Explode()
    {
        var instanceScale = gameObject.transform.localScale;

        if (DestructionClip != null)
        {
            AudioSource.PlayOneShot(DestructionClip);
        }

        GameObject brokenInstance = Instantiate(BrokenPrefab, transform.position, transform.rotation);
        brokenInstance.GetComponent<DestructiblePieces>().SetDefaultValues(instanceScale, Rigidbody.velocity, ExplosiveForceMin, ExplosiveForceMax, ExplosiveRadius, shardScaleFactor);
        gameObject.SetActive(false);
    }
}