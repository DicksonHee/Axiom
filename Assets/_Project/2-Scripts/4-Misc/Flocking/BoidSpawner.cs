using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoidSpawner : MonoBehaviour
{
    [Header("Spawn")]
    public int numBoids;
    public GameObject boidGO;

    [Header("General")]
    public float speed;
    public float turningSpeed;
    public float noClumpRadius;
    public float localAreaRadius;
    public LayerMask avoidanceLayers;

    [Header("VFX")] 
    public int particleSpawnAmount;
    public float fadeDuration;
    
    [Header("Target")] 
    public Transform poi;
    [HideInInspector] public Vector3 poiTarget;
    
    [Header("BehaviourWeights")]
    [Range(0, 1)] public float separationWeight_Editor;
    [Range(0, 1)] public float alignmentWeight_Editor;
    [Range(0, 1)] public float cohesionWeight_Editor;
    [Range(0, 1)] public float poiWeight_Editor;

    [HideInInspector] public float separationWeight;
    [HideInInspector] public float alignmentWeight;
    [HideInInspector] public float cohesionWeight;
    [HideInInspector] public float poiWeight;
    
    private List<Boid> boidList = new();
    private bool isBoidsActive = false;
    
    // Start is called before the first frame update
    void Start()
    {
        for (int ii = 0; ii < numBoids; ii++)
        {
            Boid spawnedObj = Instantiate(boidGO, 
                transform.position + new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f),
                    Random.Range(-10f, 10f)), Quaternion.identity).GetComponent<Boid>();
            
            spawnedObj.SetParent(this);
            boidList.Add(spawnedObj);
        }
        
        float maxWeight = separationWeight_Editor + alignmentWeight_Editor + cohesionWeight_Editor + poiWeight_Editor;
        separationWeight = separationWeight_Editor / maxWeight;
        alignmentWeight = alignmentWeight_Editor / maxWeight;
        cohesionWeight = cohesionWeight_Editor / maxWeight;
        poiWeight = poiWeight_Editor / maxWeight;
        poiTarget = poi.position;
        
        FadeIn();
    }
    
    private void Simulate()
    {
        float maxWeight = separationWeight_Editor + alignmentWeight_Editor + cohesionWeight_Editor + poiWeight_Editor;
        separationWeight = separationWeight_Editor / maxWeight;
        alignmentWeight = alignmentWeight_Editor / maxWeight;
        cohesionWeight = cohesionWeight_Editor / maxWeight;
        poiWeight = poiWeight_Editor / maxWeight;
        poiTarget = poi.position;
        
        foreach (Boid boid in boidList)
        {
            boid.SimulateMovement(boidList, Time.deltaTime);
        }
    }

    private void FadeIn()
    {
        if (isBoidsActive) return;

        isBoidsActive = true;
        foreach (Boid boid in boidList)
        {
            boid.gameObject.SetActive(true);
            boid.FadeIn();
        }
        
        InvokeRepeating(nameof(Simulate), 0, 0.1f);
    }

    private void FadeOut()
    {
        if (!isBoidsActive) return;
        
        isBoidsActive = false;
        CancelInvoke(nameof(Simulate));
        
        foreach (Boid boid in boidList)
        {
            boid.FadeOut();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            FadeIn();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            FadeOut();
        }
    }
    
    
}
