using Axiom.Core;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarSounds : MonoBehaviour
{
    public EventReference pillarCrashSound;
    public LayerMask groundLayer;

    private void OnTriggerEnter(Collider other)
    {
        AudioManager.current.PlaySFX3D(pillarCrashSound, transform);
        Destroy(this);
    }
}