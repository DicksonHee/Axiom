using System;
using UnityEngine;

namespace Axiom.NonEuclidean
{
    public class GravityRotator : MonoBehaviour
    {
        public float rotateSpeed;
        public static event Action<Vector3> OnGravityChanged;

        private static GravityRotator current;
        private bool shouldReportNewGravity;

        private void Awake()
        {
            if (current != null && current != this) Destroy(this);
            else current = this;
        }

        void Update()
        {
            UpdateGravity();
        }

        public static void SnapToGravity() => current.UpdateGravity(true);

        void UpdateGravity(bool snap = false)
        {
            Vector3 from = -transform.up;
            Vector3 to = Physics.gravity;
            if (from == to)
            {
                if (shouldReportNewGravity)
                {
                    shouldReportNewGravity = false;
                    OnGravityChanged?.Invoke(to);
                }
                return;
            }
        
            shouldReportNewGravity = true;
            Vector3 cross = Vector3.Cross(from, to);
            float angle = Vector3.SignedAngle(from, to, cross);

            if (!snap)
                angle = Mathf.Min(angle, rotateSpeed * Time.deltaTime);

            transform.Rotate(cross, angle, Space.World);
        }

        private void OnDestroy()
        {
            Physics.gravity = new Vector3(0, -1f, 0);
        }
    }
}