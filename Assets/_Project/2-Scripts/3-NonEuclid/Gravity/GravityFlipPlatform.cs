using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Axiom.NonEuclidean
{
    public class GravityFlipPlatform : MonoBehaviour, Core.IPlayerLand
    {
        public float degrees;
        public float rotateSpeed;

        private Quaternion initRot;

        private bool rotating = false;
        private bool atBaseRotation = true;

        private bool playerOnPlatform = false;
        private bool playerHasLeft = true;

        private void Awake()
        {
            initRot = transform.rotation;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                playerOnPlatform = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerOnPlatform = false;
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
                print(currentRot);

                transform.rotation = currentRot;
                if(playerOnPlatform)
                    Physics.gravity = transform.up * -1;

                yield return null;
            }

            transform.rotation = initRot * Quaternion.AngleAxis(degTo, Vector3.right);
            Physics.gravity = transform.up * -1;
            atBaseRotation = !atBaseRotation;

            rotating = false;
        }
    }
}