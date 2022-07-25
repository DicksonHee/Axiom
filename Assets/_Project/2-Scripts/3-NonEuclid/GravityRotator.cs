using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityRotator : MonoBehaviour
{
    void Update()
    {
        Vector3 from = -transform.up;
        Vector3 to = Physics.gravity;
        if (from == to) return;
        
        Vector3 cross = Vector3.Cross(from, to);
        float angle = Vector3.SignedAngle(from, to, cross);

        transform.Rotate(cross, angle, Space.World);
    }
}
