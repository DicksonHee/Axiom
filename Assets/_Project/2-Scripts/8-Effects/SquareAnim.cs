using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SquareAnim : MonoBehaviour
{
    public List<RotatingSquares> rotatingSquares;
    public Transform startPosition;
    public Transform endPosition;

    private bool isPlayerDetected;
    private Transform playerTransform;

    private void Start()
    {
        for (int ii = 0; ii < rotatingSquares.Count; ii++)
        {
            rotatingSquares[ii].RandomRotate();
        }
    }

    private void Update()
    {
        if (!isPlayerDetected) return;

        for(int ii = 0; ii < rotatingSquares.Count; ii++)
        {
            if (GetLerpAmount() >  ((float) ii / rotatingSquares.Count))
            {
                rotatingSquares[ii].SetIsRotating(false, (float) ii / rotatingSquares.Count);
            }
            else
            {
                rotatingSquares[ii].SetIsRotating(true, (float) ii / rotatingSquares.Count);
            }

            if (GetLerpAmount() >= 1) rotatingSquares[ii].KillRotation();
        }
    }

    public float GetLerpAmount()
    {
        return InverseLerpV3(startPosition.position, endPosition.position, playerTransform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerDetected = true;
            playerTransform = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerDetected = false;
            playerTransform = null;
        }
    }

    private float InverseLerpV3(Vector3 a, Vector3 b, Vector3 value)
    {
        Vector3 AB = b - a;
        Vector3 AV = value - a;
        return Mathf.Clamp01(Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB));
    }
}
