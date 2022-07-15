using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Axiom.NonEuclid.Gravity
{
    public class GravityShifter : MonoBehaviour
    {
        public Vector3 localGravityFrom, localGravityTo;
        public Vector3 localStart, localEnd;

        private Vector3 start, end;
        private Vector3 from, to;
        private float sqrLength;

        private Transform target;

        private void Awake()
        {
            start = transform.localToWorldMatrix.MultiplyPoint(localStart);
            end = transform.localToWorldMatrix.MultiplyPoint(localEnd);
            from = transform.rotation * localGravityFrom;
            to = transform.rotation * localGravityTo;
            sqrLength = (end - start).sqrMagnitude;
        }

        private void Update()
        {
            if (!target) return;

            float t = Vector3.Dot(target.position - start, end - start) / sqrLength;
            t = Mathf.Clamp01(t);

            Physics.gravity = Vector3.Lerp(from, to, t).normalized * Physics.gravity.magnitude;

            print($"{from}, {to}, {t} = {Physics.gravity}");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player.StateMachine.MovementSystem c))
                target = c.transform;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Player.StateMachine.MovementSystem c) && c.transform == target)
                target = null;
        }

        private void OnDrawGizmos()
        {
            Debug.DrawLine(transform.localToWorldMatrix.MultiplyPoint(localStart), transform.localToWorldMatrix.MultiplyPoint(localEnd), Color.black);
            Debug.DrawLine(transform.position, transform.localToWorldMatrix.MultiplyPoint(localGravityFrom * 5), Color.red);
            Debug.DrawLine(transform.position, transform.localToWorldMatrix.MultiplyPoint(localGravityTo * 5), Color.blue);
        }
    }
}