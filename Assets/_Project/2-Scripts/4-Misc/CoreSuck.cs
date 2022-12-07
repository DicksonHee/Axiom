using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreSuck : MonoBehaviour
{
    public Transform origin, sphere;
    public float suckStrength;

    private Transform player;
    private float sphereStartDistance;

    private void Awake()
    {
        sphereStartDistance = Vector3.Distance(origin.position, sphere.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        player = other.transform;
    }

    private void FixedUpdate()
    {
        if (player == null) return;

        Vector3 target = Vector3.Project(player.position - origin.position, origin.up);
        float distance = target.magnitude;
        target += origin.position;

        float scaledStrength = suckStrength * Mathf.Clamp01(distance / 10f);

        player.position = Vector3.Lerp(player.position, target, scaledStrength * Time.fixedDeltaTime);

        Vector3 spherePos = (distance + sphereStartDistance) * -origin.up + origin.position;
        sphere.position = spherePos;
    }
}
