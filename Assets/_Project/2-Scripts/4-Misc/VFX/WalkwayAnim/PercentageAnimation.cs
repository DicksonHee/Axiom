using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PercentageAnimation : MonoBehaviour
{
    public Animator animator;
    public Transform startPosition;
    public Transform endPosition;

    private bool isPlayerDetected;
    private Transform playerTransform;

    private void Update()
    {
        if (!isPlayerDetected) return;

        animator.Play("Rise", 0, InverseLerpV3(startPosition.position, endPosition.position, playerTransform.position));
    }

    private float InverseLerpV3(Vector3 a, Vector3 b, Vector3 value)
    {
        Vector3 AB = b - a;
        Vector3 AV = value - a;
        return Mathf.Clamp01(Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB));
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            isPlayerDetected = true;
            playerTransform = other.transform;
            animator.speed = 1f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerDetected = false;
            playerTransform = null;
            animator.speed = 0f;
        }
    }
}
