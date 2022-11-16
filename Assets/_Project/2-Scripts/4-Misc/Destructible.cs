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
    private float ExplosiveForceMin = 10;
    [SerializeField]
    private float ExplosiveForceMax = 50;
    [SerializeField]
    private float ExplosiveRadius = 2;
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
		if(collision.gameObject.tag == "Destructible")
		{
            Explode();
		}
	}
	public void Explode()
    {
        Destroy(Rigidbody);
        GetComponent<Collider>().enabled = false;
        GetComponent<Renderer>().enabled = false;
        var instanceScale = gameObject.transform.localScale;

        if (DestructionClip != null)
        {
            AudioSource.PlayOneShot(DestructionClip);
        }

        GameObject brokenInstance = Instantiate(BrokenPrefab, transform.position, transform.rotation);
        brokenInstance.transform.localScale = instanceScale;

        Rigidbody[] rigidbodies = brokenInstance.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody body in rigidbodies)
        {
            if (Rigidbody != null)
            {
                // inherit velocities
                body.velocity = Rigidbody.velocity;
            }
            body.AddExplosionForce((UnityEngine.Random.Range(ExplosiveForceMin, ExplosiveForceMax)), transform.position, ExplosiveRadius);
        }

        //StartCoroutine(FadeOutRigidBodies(rigidbodies));
        
        foreach (Transform t in brokenInstance.GetComponentsInChildren<Transform>())
        {
            StartCoroutine(Shrink(t, 5));
        }
        
        Destroy(brokenInstance, 15);
    }

    private IEnumerator Shrink(Transform t, float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 newScale = t.localScale;

        while (newScale.x >= 0.05f)
        {
            newScale -= new Vector3(shardScaleFactor, shardScaleFactor, shardScaleFactor);

            t.localScale = newScale;
            yield return new WaitForSeconds(0.05f);
        }
    }

    //private IEnumerator FadeOutRigidBodies(Rigidbody[] Rigidbodies)
    //{
    //    WaitForSeconds Wait = new WaitForSeconds(PieceSleepCheckDelay);
    //    float activeRigidbodies = Rigidbodies.Length;

    //    while (activeRigidbodies > 0)
    //    {
    //        yield return Wait;

    //        foreach (Rigidbody rigidbody in Rigidbodies)
    //        {
    //            if (rigidbody.IsSleeping())
    //            {
    //                activeRigidbodies--;
    //            }
    //        }
    //    }


    //    yield return new WaitForSeconds(PieceDestroyDelay);

    //    float time = 0;
    //    Renderer[] renderers = Array.ConvertAll(Rigidbodies, GetRendererFromRigidbody);

    //    foreach (Rigidbody body in Rigidbodies)
    //    {
    //        Destroy(body.GetComponent<Collider>());
    //        Destroy(body);
    //    }

    //    while (time < 1)
    //    {
    //        float step = Time.deltaTime * PieceFadeSpeed;
    //        foreach (Renderer renderer in renderers)
    //        {
    //            renderer.transform.Translate(Vector3.down * (step / renderer.bounds.size.y), Space.World);
    //        }

    //        time += step;
    //        yield return null;
    //    }

    //    foreach (Renderer renderer in renderers)
    //    {
    //        Destroy(renderer.gameObject);
    //    }
    //    Destroy(gameObject);
    //}

    //private Renderer GetRendererFromRigidbody(Rigidbody Rigidbody)
    //{
    //    return Rigidbody.GetComponent<Renderer>();
    //}
}