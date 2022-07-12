using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLock : MonoBehaviour
{
    public bool lockX, lockY, lockZ;

    private Vector3 initialLocalPos;

    private void Awake()
    {
        initialLocalPos = transform.localPosition;
    }

    private void Update()
    {
        Vector3 localPos = transform.localPosition;
        if (lockX) localPos.x = initialLocalPos.x;
        if (lockY) localPos.y = initialLocalPos.y;
        if (lockZ) localPos.z = initialLocalPos.z;
        transform.localPosition = localPos;
    }
}
