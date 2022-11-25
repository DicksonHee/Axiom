using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleShards : MonoBehaviour
{
    private void OnEnable()
    {
        RespawnManager.OnRespawn += DestroyShard;
    }

    private void OnDisable()
    {
        RespawnManager.OnRespawn -= DestroyShard;
    }

    private void DestroyShard()
    {
        Destroy(gameObject);
    }
}
