using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructiblePieces : MonoBehaviour
{
    public void SetDefaultValues(Vector3 instanceScale, Vector3 velocity, float ExplosiveForceMin, float ExplosiveForceMax, float ExplosiveRadius, float shardScaleFactor)
    {
        transform.localScale = instanceScale;

        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody body in rigidbodies)
        {
            body.velocity = velocity;
            body.AddExplosionForce((Random.Range(ExplosiveForceMin, ExplosiveForceMax)), transform.position, ExplosiveRadius);
        }

        //StartCoroutine(FadeOutRigidBodies(rigidbodies));

        //foreach (Transform t in GetComponentsInChildren<Transform>())
        //{
        //    StartCoroutine(Shrink(t, 5, shardScaleFactor));
        //}
        
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.drag = 0.25f;
        }

        //Destroy(gameObject, 15);
    }

    //private IEnumerator Shrink(Transform t, float delay, float shardScaleFactor)
    //{
    //    yield return new WaitForSeconds(delay);

    //    Vector3 newScale = t.localScale;

    //    while (newScale.x >= 0.05f)
    //    {
    //        newScale -= new Vector3(shardScaleFactor, shardScaleFactor, shardScaleFactor);

    //        t.localScale = newScale;
    //        yield return new WaitForSeconds(0.05f);
    //    }
    //}
}
