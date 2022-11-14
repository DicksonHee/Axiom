using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class FallingBridge : MonoBehaviour
{
    public List<BridgeRows> bridgeRows;

    public Transform startPosition;
    public Transform endPosition;

    private int currentRow = 0;
    private bool isPlayerDetected;
    private Transform playerTransform;

    private void Update()
    {
        if (!isPlayerDetected) return;

        if (currentRow < bridgeRows.Count && InverseLerpV3(startPosition.position, endPosition.position, playerTransform.position) > (float) currentRow / bridgeRows.Count)
        {
            DropRow();
        }
    }

    private void DropRow()
    {
        foreach(GameObject go in bridgeRows[currentRow].rows)
        {
            go.transform.DOLocalMoveY(-85f, Random.Range(0.75f, 1.5f)).SetEase(Ease.InOutQuint);
        }

        currentRow++;
    }

    private float InverseLerpV3(Vector3 a, Vector3 b, Vector3 value)
    {
        Vector3 AB = b - a;
        Vector3 AV = value - a;
        return Mathf.Clamp01(Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerDetected = true;
            playerTransform = other.transform;
        }
    }
}

[Serializable]
public class BridgeRows
{
    public List<GameObject> rows;
}
