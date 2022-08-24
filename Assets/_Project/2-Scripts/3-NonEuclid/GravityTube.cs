using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Axiom.Player.Movement.StateMachine;

namespace Axiom.Player.Movement.NonEuclid
{
    public class GravityTube : MonoBehaviour
    {
        private Transform target;

        private void Update()
        {
            if (!target) return;

            Vector3 local = transform.worldToLocalMatrix.MultiplyPoint(target.position);
            Debug.DrawLine(local + transform.position, transform.position);
            Vector3 gravity = new Vector3(local.x, 0, local.z).normalized;
            gravity = transform.localToWorldMatrix.MultiplyVector(gravity);
            print($"local {local}, gravity {gravity}");
            Physics.gravity = gravity;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out MovementSystem c))
                target = c.transform;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out MovementSystem c) && c.transform == target)
                target = null;
        }
    }
}