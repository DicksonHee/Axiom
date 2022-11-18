using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Axiom.Player.Movement.StateMachine;
using Axiom.Player.Movement;

namespace Axiom.NonEuclidean
{
    public class GravityFlipPlatform : MonoBehaviour, Core.IPlayerLand
    {
        public float degrees;
        public float rotateSpeed;
        public FlippyTrigger[] landingTriggers;

        private FlippyTrigger activeTrigger;

        private Quaternion initRot;

        private bool rotating = false;
        private bool atBaseRotation = true;

        private bool playerOnPlatform => activeTrigger != null;
        private bool playerHasLeft = true;
        private MovementSystem player;

        private void Awake()
        {
            initRot = transform.rotation;
        }

        private void ResetRotation()
        {
            StopAllCoroutines();
            transform.rotation = initRot;
            atBaseRotation = true;
            activeTrigger = null;
            playerHasLeft = true;
            rotating = false;
        }

        private void OnEnable()
        {
            foreach (FlippyTrigger trigger in landingTriggers)
            {
                trigger.OnEnter += OnFlippyTriggerEnter;
                trigger.OnExit += OnFlippyTriggerExit;
            }
            RespawnManager.OnRespawn += ResetRotation;
        }

        private void OnDisable()
        {
            foreach (FlippyTrigger trigger in landingTriggers)
            {
                trigger.OnEnter -= OnFlippyTriggerEnter;
                trigger.OnExit -= OnFlippyTriggerExit;
            }
            RespawnManager.OnRespawn -= ResetRotation;
        }

        private void OnFlippyTriggerEnter(Collider other, FlippyTrigger trigger)
        {
            if (other.CompareTag("Player"))
            {
                if (player == null)
                    player = other.GetComponent<MovementSystem>();

                activeTrigger = trigger;
                //playerOnPlatform = true;
            }
        }

        private void OnFlippyTriggerExit(Collider other, FlippyTrigger trigger)
        {
            if (other.CompareTag("Player"))
            {
                //playerOnPlatform = false;
                activeTrigger = null;
                playerHasLeft = true;
            }
        }

        public void OnPlayerLand()
        {
            if (playerHasLeft && !rotating)
            {
                playerHasLeft = false;
                StartCoroutine(Rotate());
            }
        }

        private void OnDrawGizmos()
        {
            Quaternion target = transform.rotation * Quaternion.AngleAxis(degrees, Vector3.right);
            Debug.DrawRay(transform.position, transform.up * -5f, Color.blue);
            Debug.DrawRay(transform.position, target * Vector3.up * -5f, Color.red);
        }

        IEnumerator Rotate()
        {
            rotating = true;

            float degFrom = atBaseRotation ? 0 : degrees;
            float degTo = atBaseRotation ? degrees : 0;

            float time = Mathf.Abs(degrees) / rotateSpeed;
            for (float elapsed = 0; elapsed < time; elapsed += Time.deltaTime)
            {
                float t = elapsed / time;

                float currentDeg = Mathf.Lerp(degFrom, degTo, t);
                Quaternion currentRot = initRot * Quaternion.AngleAxis(currentDeg, Vector3.right);

                transform.rotation = currentRot;
                if (playerOnPlatform)
                {
                    Quaternion rotDelta = Quaternion.AngleAxis(Time.deltaTime / time * (degTo - degFrom), transform.right);
                    Vector3 relativePlayerPos = player.transform.position - transform.position;
                    Vector3 newPlayerPos = rotDelta * relativePlayerPos + transform.position;

                    player.TeleportPlayer(newPlayerPos);

                    Physics.gravity = activeTrigger.transform.up * -1;
                    GravityRotator.SnapToGravity();
                }

                yield return null;
            }

            transform.rotation = initRot * Quaternion.AngleAxis(degTo, Vector3.right);

            if(playerOnPlatform)
                Physics.gravity = activeTrigger.transform.up * -1;

            atBaseRotation = !atBaseRotation;

            rotating = false;
        }
    }
}