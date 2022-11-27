using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bezier;

public class FireNeuron : MonoBehaviour
{
    public Curve[] curves;
    public float speed;
    public Transform neuron;

    private Curve nearestCurve;
    private float tSpeed;
    private Vector3 playerOffset;
    private float offsetMult;
    private float t = 1;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            Fire();

        if (t >= 1)
            return;

        Vector3 pos = nearestCurve.GetCurvePointNormalised(t) + playerOffset * offsetMult;
        neuron.transform.position = pos;

        //offsetMult = Mathf.Max(offsetMult - 2f * Time.deltaTime, 0);
        //offsetMult *= 0.9f;
        playerOffset *= 0.9f;
        t += tSpeed * Time.deltaTime;
    }

    public void Fire()
    {
        FindNearestPoints(out nearestCurve, out int point1, out int point2);
        print(point1 + " | " + point2);

        tSpeed = speed / nearestCurve.ArcLength;
        t = EstimateTValue(nearestCurve, point1, point2);
        t = nearestCurve.T2Dist(t) / nearestCurve.ArcLength;
        print((float)t * nearestCurve.SegmentCount);
        Debug.DrawRay(nearestCurve.GetCurvePointNormalised(t), Vector3.up, Color.yellow, 10f);
        Debug.DrawRay(nearestCurve.GetCurvePointNormalised(t), Vector3.right, Color.yellow, 10f);
        playerOffset = transform.position - nearestCurve.GetCurvePointNormalised(t);
        offsetMult = 1f;
    }

    private float EstimateTValue(Curve curve, int point1, int point2)
    {
        Vector3 v1 = curve.Points[point1];
        Vector3 v2 = curve.Points[point2];
        float sqrLength = (v2 - v1).sqrMagnitude;

        float t = Vector3.Dot(transform.position - v1, v2 - v1) / sqrLength;
        t = Mathf.Clamp01(t);

        return (point1 + t) / (curve.SegmentCount);
    }

    private void FindNearestPoints(out Curve curve, out int point1, out int point2)
    {
        int nearestCurve = -1;
        int nearestPoint = -1;
        float nearestDist = Mathf.Infinity;
        for (int c = 0; c < curves.Length; c++)
        {
            if (!curves[c].enabled) continue;

            for (int p = 0; p < curves[c].Points.Length; p++)
            {
                float dist = Axiom.Core.F.FastDistance(transform.position, curves[c].Points[p]);
                if (dist < nearestDist)
                {
                    print($"[{p}]: {dist}");
                    nearestCurve = c;
                    nearestPoint = p;
                    nearestDist = dist;
                }
            }
        }

        if (nearestCurve < 0)
            throw new System.Exception("Couldn't find closest point: No active curves found");

        curve = curves[nearestCurve];
        if (nearestPoint >= curve.SegmentCount)
        {
            point1 = nearestPoint - 1;
            point2 = nearestPoint;
        }
        else if (nearestPoint <= 0)
        {
            point1 = nearestPoint;
            point2 = nearestPoint + 1;
        }
        else
        {
            float previous = Axiom.Core.F.FastDistance(curve.Points[nearestPoint - 1], transform.position);
            float next = Axiom.Core.F.FastDistance(curve.Points[nearestPoint + 1], transform.position);

            if (previous < next)
            {
                point1 = nearestPoint - 1;
                point2 = nearestPoint;
            }
            else
            {
                point1 = nearestPoint;
                point2 = nearestPoint + 1;
            }
        }
    }
}
