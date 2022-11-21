using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceManager : MonoBehaviour
{
    public List<Spawner> spawners;

    private void OnEnable()
    {
        RespawnManager.OnRespawn += ResetSequence;
    }

    private void OnDisable()
    {
        RespawnManager.OnRespawn -= ResetSequence;
    }

    public void ResetSequence()
    {
        foreach(Spawner spawner in spawners)
        {
            spawner.Reset();
        }
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha0)) ResetSequence();
    }
}