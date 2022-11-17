using System;
using UnityEngine;

namespace Axiom.NonEuclidean
{
    public class GravityRotator : MonoBehaviour
    {
        public static GravityRotator current;
        public float rotateSpeed;

        private bool shouldReportNewGravity;
        public static event Action<Vector3> OnGravityChanged;

        private void Awake()
        {
            if (current != null && current != this) Destroy(this);
            else current = this;
        }

        void Update()
        {
            UpdateGravity();
        }

        public void UpdateGravity()
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
            angle = Mathf.Min(angle, rotateSpeed * Time.deltaTime);

            transform.Rotate(cross, angle, Space.World);
        }
    }
}