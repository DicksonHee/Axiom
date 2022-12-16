using Axiom.Core;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShatteringSounds : MonoBehaviour
{
    public EventReference breakingPlatforms;

    private void OnCollisionEnter(Collision collision)
    {
        AudioManager.current.PlaySFX3D(breakingPlatforms, transform);
        Destroy(this);
    }
}
