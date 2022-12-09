using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreSuck : MonoBehaviour
{
    public Transform origin, sphere;
    public float suckStrength;

    public Renderer coreRenderer;
    public Light coreLight;

    private Transform player;
    private float sphereStartDistance;
    private bool frozen;

    private void Awake()
    {
        sphereStartDistance = Vector3.Distance(origin.position, sphere.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        player = other.transform;
    }

    private void LateUpdate()
    {
        if (player == null || !frozen)
            SuckIntoVoid();
        else if (frozen)
            ApproachCore();
    }

    private void SuckIntoVoid()
    {
        Vector3 target = Vector3.Project(player.position - origin.position, origin.up);
        float distance = target.magnitude;
        target += origin.position;

        player.position = Vector3.Lerp(player.position, target, distance.Remap(0f, 50f));

        Vector3 spherePos = (distance + sphereStartDistance) * -origin.up + origin.position;
        sphere.position = spherePos;

        if (distance > 60f)
        {
            frozen = true;
            player.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    private float lightIntensity1;
    private Vector3 playerPos1, playerPos2;
    private void SetApproachCoreVariables()
    {
        lightIntensity1 = coreLight.intensity;

        playerPos1 = player.position;
        playerPos2 = coreRenderer.transform.position;
    }

    public float officeFade;
    public float coreZoom;
    private void ApproachCore()
    {
        coreLight.intensity = Mathf.Lerp(lightIntensity1, 0f, officeFade);
        coreRenderer.material.SetColor("_MainColor", new Color(1, 1, 1, 1f - officeFade));

        player.position = Vector3.Lerp(playerPos1, playerPos2, coreZoom);
    }
}
