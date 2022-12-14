using Axiom.Player.Movement.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceCroucher : MonoBehaviour
{
    public MovementSystem movementSystem;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        movementSystem.ForceStartCrouch();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Invoke(nameof(EndForceCrouch), 0.2f);
    }
    void EndForceCrouch()
    {
        movementSystem.EndForceCrouch();
    }
}
