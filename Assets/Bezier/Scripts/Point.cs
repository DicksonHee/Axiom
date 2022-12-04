using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Bezier
{
    public class Point : MonoBehaviour
    {
        private const float pointRadius = 0.2f;
        private const float handleRadius = 0.15f;

        public Vector3 A => tA.position;
        public Vector3 B => tB.position;

        private Transform tA, tB;

        [HideInInspector] public bool curveActive;

        public void Initialise()
        {
            tA = transform.GetChild(0);
            tB = transform.GetChild(1);
        }

        private void OnDrawGizmos()
        {
            if (!curveActive)
                return;

            if (tA == null || tB == null)
                Initialise();

            bool selected = Selection.activeTransform == transform ||
                Selection.activeTransform == tA ||
                Selection.activeTransform == tB;

            Gizmos.color = selected ? Color.blue : Color.grey;
            Gizmos.DrawWireSphere(A, handleRadius * Curve.GizmoScale);
            Gizmos.DrawLine(transform.position, A);

            Gizmos.color = selected ? Color.red : Color.grey;
            Gizmos.DrawWireSphere(B, handleRadius * Curve.GizmoScale);
            Gizmos.DrawLine(transform.position, B);

            Gizmos.color = selected ? Color.white : Color.gray;
            Gizmos.DrawWireSphere(transform.position, pointRadius * Curve.GizmoScale);
        }

        public static implicit operator Vector3(Point p) => p.transform.position;
    }
}