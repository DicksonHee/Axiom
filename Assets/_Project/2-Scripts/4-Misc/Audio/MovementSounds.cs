using System;
using System.Collections;
using System.Collections.Generic;
using Axiom.Core;
using Axiom.Player.Movement.StateMachine;
using FMODUnity;
using UnityEngine;
using Random = UnityEngine.Random;

public class MovementSounds : MonoBehaviour
{
    public EventReference jumpEventReference;
    public EventReference landEventReference;
    public EventReference sprintEventReference;
    public EventReference respawnEventReference;

    private bool canPlaySprintSound = true;
    private bool canPlayLandSound = true;

    private void OnEnable()
    {
        //MovementSystem.OnJump += PlayJumpSound;
        //MovementSystem.OnLand += PlayLandSound;
        //MovementSystem.OnSprint += PlaySprintSound;
        RespawnManager.OnPlayRespawnSound += PlayRespawnSound;
    }
    
    private void OnDisable()
    {
        //MovementSystem.OnJump -= PlayJumpSound;
        //MovementSystem.OnLand -= PlayLandSound;
        //MovementSystem.OnSprint -= PlaySprintSound;
        RespawnManager.OnPlayRespawnSound -= PlayRespawnSound;
    }

    public void PlayRespawnSound()
    {
        AudioManager.current.PlaySFX2D(respawnEventReference);
    }
    
    public void PlayJumpSound()
    {
        AudioManager.current.PlaySFX3D(jumpEventReference, transform);
    }

    public void PlayLandSound()
    {
        if (!canPlayLandSound) return;
        
        SetCannotPlayLandSound();
        Invoke(nameof(SetCanPlayLandSound), 1f);
        
        AudioManager.current.PlaySFX3D(landEventReference, transform);
    }

    public void PlaySprintSound()
    {
        if (!canPlaySprintSound) return;
        
        SetCannotPlaySprintSound();
        Invoke(nameof(SetCanPlaySprintSound), Random.Range(5f,10f));
        
        AudioManager.current.PlaySFX3D(sprintEventReference, transform);
    }

    private void SetCannotPlaySprintSound() => canPlaySprintSound = false;
    private void SetCanPlaySprintSound() => canPlaySprintSound = true;
    
    private void SetCannotPlayLandSound() => canPlayLandSound = false;
    private void SetCanPlayLandSound() => canPlayLandSound = true;
}
