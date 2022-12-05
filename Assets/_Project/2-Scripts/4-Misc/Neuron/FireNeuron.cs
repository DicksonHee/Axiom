using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bezier;

public class FireNeuron : MonoBehaviour
{
    public NeuronPathArea[] pathAreas;
    public TrailRenderer neuron;
    public Animator neuronCharge;

    public int nearestPointSubdivisions;
    public float holdTimeToTrigger;
    public float speed;
    public float travelDistance;
    public float shrinkDistance;

    private bool fireTriggered = false;
    private float elapsedHoldTime = 0f;

    private Vector3[][] subdivCurves;
    private Curve activeCurve;
    private float tSpeed;
    private float tTarget;
    private float tShrink;
    private Vector3 playerOffset;
    private float offsetMult;
    private float t = 1;
    private bool neuronPathPendingClear = false;

    public void Awake()
    {
        subdivCurves = new Vector3[pathAreas.Length][];
        for (int c = 0; c < subdivCurves.Length; c++)
        {
            Curve curve = pathAreas[c].Curve;
            curve.Initialise();
            subdivCurves[c] = curve.SubdivideCurve(nearestPointSubdivisions, true);
        }
    }

    private void Update()
    {
        if (HoldTrigger())
            Fire();

        AnimateBuildUp();

        if (t < tTarget)
            UpdateNeuron();
    }

    private bool HoldTrigger()
    {
        if (!Input.GetKey(KeyCode.E))
        {
            fireTriggered = false;
            elapsedHoldTime = 0f;
            return false;
        }

        elapsedHoldTime += Time.deltaTime;
        if (!fireTriggered && elapsedHoldTime > holdTimeToTrigger)
        {
            fireTriggered = true;
            return true;
        }
        return false;
    }

    private void AnimateBuildUp()
    {
        float animT = Mathf.Clamp01(elapsedHoldTime / holdTimeToTrigger);
        if (fireTriggered) animT = 0;
        neuronCharge.SetFloat("Charge", animT);
    }

    private void UpdateNeuron()
    {
        Vector3 pos = activeCurve.GetCurvePointNormalised(t) + playerOffset * offsetMult;
        neuron.transform.position = pos;

        offsetMult *= 0.9f;
        t += tSpeed * Time.deltaTime;

        float sizeMult = t.Remap(tShrink, tTarget, 1f, 0f, true);
        neuron.transform.localScale = Vector3.one * sizeMult;
        neuron.widthMultiplier = sizeMult;

        if (neuronPathPendingClear)
        {
            neuronPathPendingClear = false;
            neuron.Clear();
        }
    }

    private void Fire()
    {
        int[] activeCurves = GetActiveCurves();
        if(activeCurves.Length <= 0) return;

        GetNearestPoint(activeCurves, out int curve, out int point);
        activeCurve = pathAreas[curve].Curve;
        tSpeed = speed / activeCurve.ArcLength;

        t = (float)point / (subdivCurves[curve].Length - 1);
        t = activeCurve.T2Dist(t) / activeCurve.ArcLength;

        float distTarget = activeCurve.T2Dist(t) + travelDistance;
        tTarget = distTarget / activeCurve.ArcLength;
        tShrink = (distTarget - shrinkDistance) / activeCurve.ArcLength;

        Debug.DrawRay(activeCurve.GetCurvePointNormalised(t), Vector3.up, Color.yellow, 10f);
        Debug.DrawRay(activeCurve.GetCurvePointNormalised(t), Vector3.right, Color.yellow, 10f);

        playerOffset = transform.position - activeCurve.GetCurvePointNormalised(t);
        offsetMult = 1f;

        neuronPathPendingClear = true;
        neuronCharge.SetTrigger("Fire");
    }

    private int[] GetActiveCurves()
    {
        List<int> active = new List<int>();
        for(int c = 0; c < pathAreas.Length; c++)
            if (pathAreas[c].gameObject.activeSelf && pathAreas[c].PointInArea(transform.position))
                active.Add(c);

        return active.ToArray();
    }

    private void GetNearestPoint(int[] curves, out int nearestCurve, out int nearestPoint)
    {
        nearestCurve = -1;
        nearestPoint = -1;
        float nearestDist = Mathf.Infinity;
        for (int c = 0; c < curves.Length; c++)
        {
            for (int p = 0; p < subdivCurves[curves[c]].Length; p++)
            {
                float dist = F.FastDistance(subdivCurves[curves[c]][p], transform.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearestCurve = curves[c];
                    nearestPoint = p;
                }
            }
        }
    }
}
