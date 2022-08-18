using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;

public class Boid : MonoBehaviour
{
    public int swarmID;

    public MeshRenderer meshRenderer;
    public VisualEffect vfx;
    
    private Vector3 steering;

    private Boid leaderBoid;
    private float leaderAngle;

    private BoidSpawner spawner;
    private RaycastHit hitInfo;

    private float currentSpeed;
    private float currentTurningSpeed;
    
    public void SetParent(BoidSpawner parent)
    {
        spawner = parent;
    }

    public void FadeIn() => StartCoroutine(FadeIn_CO());
    public void FadeOut() => StartCoroutine(FadeOut_CO());
    
    private IEnumerator FadeIn_CO()
    {
        float duration = 0f;
        vfx.SetInt("BurstAmount", spawner.particleSpawnAmount);
        
        while (duration < spawner.fadeDuration)
        {
            duration += Time.deltaTime;
            meshRenderer.material.SetFloat("_DissolveAmount", Mathf.Lerp(1, 0, duration / spawner.fadeDuration));
            yield return null;
        }
        
        vfx.SetInt("BurstAmount", 0);
    }

    private IEnumerator FadeOut_CO()
    {
        float duration = 0f;
        vfx.SetInt("BurstAmount", spawner.particleSpawnAmount);

        while (duration < spawner.fadeDuration)
        {
            duration += Time.deltaTime;
            meshRenderer.material.SetFloat("_DissolveAmount", Mathf.Lerp(0, 1, duration / spawner.fadeDuration));
            yield return null;
        }

        vfx.SetInt("BurstAmount", 0);

        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
    
    public void SimulateMovement(List<Boid> boidList, float time)
    {
        steering = Vector3.zero;
        SeparationCheck(boidList);

        if (hitInfo.collider == null)
        {
            currentSpeed = spawner.speed;
            currentTurningSpeed = spawner.turningSpeed;
        }
        else
        {
            currentSpeed = spawner.speed * 0.8f;
            currentTurningSpeed = spawner.turningSpeed * 2f;
        }
    }

    private void FixedUpdate()
    {
        if (steering != Vector3.zero)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(steering), currentTurningSpeed * Time.deltaTime);
        }
        
        transform.position += transform.TransformDirection(new Vector3(0, 0, currentSpeed)) * Time.deltaTime;
    }

    private void SeparationCheck(List<Boid> boidList)
    {
        Vector3 separationDirection = Vector3.zero;
        Vector3 alignmentDirection = Vector3.zero;
        Vector3 cohesionDirection = Vector3.zero;
        Vector3 poiDirection = Vector3.zero;
        
        int separationCount = 0;
        int alignmentCount = 0;
        int cohesionCount = 0;
        
        // Boid to Boid interactions
        foreach (Boid boid in boidList)
        {
            if(boid == this) continue;
            if (boid.swarmID != swarmID) continue;
            
            float distanceToBoid = Vector3.Distance(boid.transform.position, transform.position);

            // Separation
            if (distanceToBoid < spawner.noClumpRadius)
            {
                separationDirection += boid.transform.position - transform.position;
                separationCount++;
            }

            // Alignment/ Cohesion/ Leader
            if (distanceToBoid < spawner.localAreaRadius)
            {
                alignmentDirection += boid.transform.forward;
                alignmentCount++;

                cohesionDirection += boid.transform.position - transform.position;
                cohesionCount++;
                
                var angle = Vector3.Angle(boid.transform.position - transform.position, transform.forward);
                if (angle < leaderAngle && angle < 90f)
                {
                    leaderBoid = boid;
                    leaderAngle = angle;
                }
            }
        }

        separationDirection = -separationDirection;
        cohesionDirection -= transform.position;
        
        if (separationCount > 0) separationDirection /= separationCount;
        if (alignmentCount > 0) alignmentDirection /= alignmentCount;
        if (cohesionCount > 0) cohesionDirection /= cohesionCount;
        
        // POI & Leader
        if(Vector3.Distance(poiDirection, transform.position) > spawner.localAreaRadius) poiDirection += (spawner.poiTarget - transform.position);
        if (leaderBoid != null) steering += (leaderBoid.transform.position - transform.position).normalized * 0.5f;
        
        // Adding all the weights together
        steering += separationDirection.normalized * spawner.separationWeight +
                    alignmentDirection.normalized * spawner.alignmentWeight +
                    cohesionDirection.normalized * spawner.cohesionWeight +
                    poiDirection.normalized * spawner.poiWeight;
        
        // Check for collision
        if (Physics.SphereCast(transform.position, 1f, transform.forward, out hitInfo, spawner.localAreaRadius,
                spawner.avoidanceLayers))

        {
            steering = -(hitInfo.point - transform.position).normalized;
        }
    }
}
