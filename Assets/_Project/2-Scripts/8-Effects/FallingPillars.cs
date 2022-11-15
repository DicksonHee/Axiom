using System;
using Axiom.Player.Movement.StateMachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Axiom.Core;
using Random = UnityEngine.Random;

public class FallingPillars : MonoBehaviour
{
    public MovementSystem movementSystem;
    public GameObject prefab;
    public float spawnDistance;
    public Vector2 spawnTime;

    public Transform startPos;
    public Transform endPos;

    private int currentIncrement = 0;
    private BoxCollider boxCollider;
    private bool isPlayerDetected;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }
    
    private void SpawnPillar(Vector3 targetPosition)
    {
        Vector3 spawnPos = targetPosition + new Vector3(0f, 100f, 0f);
        GameObject pillar = Instantiate(prefab, spawnPos, Quaternion.identity);
        pillar.transform.DOMove(targetPosition, 0.5f).SetEase(Ease.Flash);
    }

    private IEnumerator SpawnPillarDelay_CO(Vector3 targetPosition, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        SpawnPillar(targetPosition);
    }

    private void CheckFloor()
    {
        if (F.InverseLerpV3(startPos.position, endPos.position, movementSystem.transform.position) >
            (float)currentIncrement / 10)
        {
            currentIncrement++;
            spawnTime *= 0.9f;
        }

        Vector3 checkPos = movementSystem.transform.position + movementSystem.MoveDirection.normalized * spawnDistance;
        
        if (boxCollider.bounds.Contains(checkPos))
        {
            SpawnPillar(checkPos);
            if (currentIncrement > 5) StartCoroutine(SpawnPillarDelay_CO(checkPos, 0.2f));
        }

        Invoke(nameof(CheckFloor), Random.Range(spawnTime.x, spawnTime.y));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerDetected = true;
            Invoke(nameof(CheckFloor), Random.Range(spawnTime.x, spawnTime.y));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerDetected = false;
            CancelInvoke(nameof(CheckFloor));
            StopAllCoroutines();
        }
    }
}
