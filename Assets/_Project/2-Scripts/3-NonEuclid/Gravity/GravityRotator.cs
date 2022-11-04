using System;
using UnityEngine;

namespace Axiom.NonEuclidean
{
    public class GravityRotator : MonoBehaviour
    {
        public static GravityRotator current;
        public float rotateSpeed;

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
            if (from == to) return;
        
            Vector3 cross = Vector3.Cross(from, to);
            float angle = Vector3.SignedAngle(from, to, cross);
            angle = Mathf.Min(angle, rotateSpeed * Time.deltaTime);

            transform.Rotate(cross, angle, Space.World);
        }

        private void OnDestroy()
        {
            Physics.gravity = new Vector3(0f, -9.81f, 0f);
        }
    }
}