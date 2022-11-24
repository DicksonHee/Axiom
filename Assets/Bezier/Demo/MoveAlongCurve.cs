using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bezier;

public class MoveAlongCurve : MonoBehaviour
{
    public float speed;
    public Curve curve;

    private float t;

    void Update()
    {
        t = F.Wrap(t + speed * Time.deltaTime);

        transform.position = curve.GetCurvePointNormalised(t);
        transform.forward = curve.GetDerivativeNormalised(t);
    }
}
