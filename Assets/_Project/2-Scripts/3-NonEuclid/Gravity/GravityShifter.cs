using Axiom.Core;
using Axiom.Player.Movement.StateMachine;
using UnityEngine;

namespace Axiom.NonEuclidean
{
    public class GravityShifter : MonoBehaviour
    {
        public Vector3 localGravityFrom, localGravityTo;
        public Vector3 localStart, localEnd;

        private Vector3 start, end;
        private Vector3 from, to;
        private float sqrLength;
        private float t;

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

            t = Vector3.Dot(target.position - start, end - start) / sqrLength;
            t = Mathf.Clamp01(t);

            Physics.gravity = Vector3.Lerp(from, to, t).normalized * Physics.gravity.magnitude;

            //print($"{from}, {to}, {t} = {Physics.gravity}");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out MovementSystem c))
                target = c.transform;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out MovementSystem c) && c.transform == target)
            {
                if (t.Between(0f, 1f, true))
                    Physics.gravity = t > 0.5f ? to : from;
                target = null;
            }
        }

        private void OnDrawGizmos()
        {
            Debug.DrawLine(transform.localToWorldMatrix.MultiplyPoint(localStart), transform.localToWorldMatrix.MultiplyPoint(localEnd), Color.black);
            Debug.DrawRay(transform.localToWorldMatrix.MultiplyPoint(localStart), transform.rotation * localGravityFrom * 3, Color.red);
            Debug.DrawRay(transform.localToWorldMatrix.MultiplyPoint(localEnd), transform.rotation * localGravityTo * 3, Color.blue);
        }
    }
}