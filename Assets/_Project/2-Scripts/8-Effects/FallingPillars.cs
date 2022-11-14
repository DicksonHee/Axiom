using Axiom.Player.Movement.StateMachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPillars : MonoBehaviour
{
    public LayerMask groundLayer;
    public MovementSystem movementSystem;
    public GameObject prefab;
    public float spawnDistance;
    public Vector2 spawnBounds;

    private BoxCollider boxCollider;
    private bool isPlayerDetected;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    public void SpawnPillar(Vector3 targetPosition)
    {
        Vector3 spawnPos = targetPosition + new Vector3(0f, 100f, 0f);
        GameObject pillar = Instantiate(prefab, spawnPos, Quaternion.identity);
        pillar.transform.DOMove(targetPosition, 0.5f).SetEase(Ease.Flash);
    }

    private void CheckFloor()
    {
        if (boxCollider.bounds.Contains(movementSystem.transform.position + movementSystem.ForwardDirection * spawnDistance))
        {
            SpawnPillar(movementSystem.transform.position + movementSystem.ForwardDirection * spawnDistance);
        }

        Invoke(nameof(CheckFloor), Random.Range(spawnBounds.x, spawnBounds.y));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerDetected = true;
            Invoke(nameof(CheckFloor), Random.Range(spawnBounds.x, spawnBounds.y));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerDetected = false;
            CancelInvoke(nameof(CheckFloor));
        }
    }
}
