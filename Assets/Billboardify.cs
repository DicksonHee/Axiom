using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboardify : MonoBehaviour
{
    public bool reverse;
    private Transform cam;

    private void Awake() => cam = Camera.main.transform;
    private void Update() => transform.forward = cam.transform.forward * (reverse ? -1 : 1);
}
