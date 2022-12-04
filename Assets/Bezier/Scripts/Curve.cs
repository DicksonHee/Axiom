using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Bezier
{
    public class Curve : MonoBehaviour
    {
        [Header("Curve")]
        [Range(2, 20)] [SerializeField] private int normalisationAccuracy = 10;
        [SerializeField] private bool closeCurve = false;

        [Header("Viewport")]
        [Range(5, 50)] [SerializeField] private int viewportSubdivisions = 20;
        [SerializeField] private Color viewportColour = Color.white;
        [SerializeField] private float gizmoScale = 1f;

        public Point[] Points => points;
        public int SegmentCount => points.Length - 1;
        public float ArcLength => distLUT[^1];

        public static float GizmoScale;
        private float lastGizmoScale;

        private Point[] points;

        private Vector3[] subdividedPoints;
        private Vector3[] viewportPoints;
        private float[] distLUT;

        private bool curveActive;
        private bool initialised = false;

        private void Awake()
        {
            if (!initialised)
                Initialise();
        }

        public void Initialise()
        {
            InitialisePoints();
            subdividedPoints = SubdivideCurve(normalisationAccuracy, true);
            distLUT = GetDistanceLut(subdividedPoints);

            initialised = true;
        }

        private void InitialisePoints()
        {
            List<Point> pointList = new List<Point>(GetComponentsInChildren<Point>(true));
            if (closeCurve)
                pointList.Add(pointList[0]);
            points = pointList.ToArray();

            foreach (Point p in points)
                p.Initialise();
        }

        public Vector3 GetCurvePointNormalised(float t) => GetCurvePoint(NormaliseT(t));
        public Vector3 GetCurvePoint(float t)
        {
            float totalT = t * SegmentCount;
            t = totalT % 1;

            if (totalT <= 0) return points[0];
            else if (totalT >= SegmentCount) return points[^1];
            else if (t == 0) return points[Mathf.RoundToInt(totalT)];

            //print($"{totalT}: {Mathf.FloorToInt(totalT)} - {Mathf.CeilToInt(totalT)}");
            Vector3 P0 = points[Mathf.FloorToInt(totalT)];
            Vector3 P1 = points[Mathf.FloorToInt(totalT)].B;
            Vector3 P2 = points[Mathf.CeilToInt(totalT)].A;
            Vector3 P3 = points[Mathf.CeilToInt(totalT)];

            Vector3 A = Vector3.Lerp(P0, P1, t);
            Vector3 B = Vector3.Lerp(P1, P2, t);
            Vector3 C = Vector3.Lerp(P2, P3, t);

            Vector3 D = Vector3.Lerp(A, B, t);
            Vector3 E = Vector3.Lerp(B, C, t);

            return Vector3.Lerp(D, E, t);
        }

        public Vector3 GetDerivativeNormalised(float t) => GetDerivative(NormaliseT(t));
        public Vector3 GetDerivative(float t)
        {
            float totalT = t * SegmentCount;
            t = totalT % 1;

            Vector3 P0 = points[Mathf.FloorToInt(totalT)];
            Vector3 P1 = points[Mathf.FloorToInt(totalT)].B;
            Vector3 P2 = points[Mathf.CeilToInt(totalT)].A;
            Vector3 P3 = points[Mathf.CeilToInt(totalT)];

            float t2 = t * t;

            return P0 * (-3 * t2 + 6 * t - 3) +
                P1 * (9 * t2 - 12 * t + 3) +
                P2 * (-9 * t2 + 6 * t) +
                P3 * (3 * t2);
        }

        public float NormaliseT(float t) => Dist2T(t * ArcLength);

        public float T2Dist(float t)
        {
            if (t <= 0) return 0;
            else if (t >= 1) return ArcLength;

            int i = Mathf.FloorToInt(t * (distLUT.Length - 1));

            return t.Remap(
                (float)i / (distLUT.Length - 1),
                (i + 1f) / (distLUT.Length - 1),
                distLUT[i],
                distLUT[i + 1]);
        }

        public float Dist2T(float dist)
        {
            if (dist <= 0) return 0;
            else if (dist >= ArcLength) return 1;

            for (int i = 0; i < distLUT.Length - 1; i++)
            {
                if (dist.Between(distLUT[i], distLUT[i + 1]))
                {
                    return dist.Remap(
                        distLUT[i],
                        distLUT[i + 1],
                        (float)i / (distLUT.Length - 1),
                        (i + 1f) / (distLUT.Length - 1));
                }
            }

            return dist / ArcLength;
        }

        private float[] GetDistanceLut(Vector3[] samples)
        {
            float[] distancePerSample = new float[samples.Length];

            float cumulatveDistance = 0f;
            for (int i = 0; i < samples.Length - 1; i++)
            {
                float distance = Vector3.Distance(samples[i], samples[i + 1]);
                cumulatveDistance += distance;
                distancePerSample[i + 1] = cumulatveDistance;
            }

            return distancePerSample;
        }

        public Vector3[] SubdivideCurve(int subdivisions, bool perSegment = false)
        {
            if (perSegment)
                subdivisions *= SegmentCount;

            Vector3[] samples = new Vector3[subdivisions];
            for (int i = 0; i < subdivisions; i++)
            {
                float t = (float)i / (subdivisions - 1);
                samples[i] = GetCurvePoint(t);
            }

            return samples;
        }

        private void GetViewportPoints()
        {
            InitialisePoints();
            viewportPoints = SubdivideCurve(viewportSubdivisions, true);
        }

        private void OnValidate()
        {
            if (lastGizmoScale != gizmoScale)
                GizmoScale = gizmoScale;
            else
                gizmoScale = GizmoScale;

            lastGizmoScale = gizmoScale;
        }

        private void OnDrawGizmos()
        {
            Transform active = Selection.activeTransform;
            bool selected = active != null && (active == transform || active.IsChildOf(transform));
            if (selected != curveActive)
            {
                if (points == null)
                    InitialisePoints();

                foreach (Point p in points)
                    p.curveActive = selected;

                if (lastGizmoScale == gizmoScale && gizmoScale != GizmoScale)
                {
                    gizmoScale = GizmoScale;
                    lastGizmoScale = gizmoScale;
                }

                curveActive = selected;
            }

            if (viewportPoints == null || selected)
                GetViewportPoints();

            Gizmos.color = viewportColour;
            for (int i = 0; i < viewportPoints.Length - 1; i++)
                Gizmos.DrawLine(viewportPoints[i], viewportPoints[i + 1]);
        }
    }
}