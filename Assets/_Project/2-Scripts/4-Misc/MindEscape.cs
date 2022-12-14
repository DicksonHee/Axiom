using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bezier;

public class MindEscape : MonoBehaviour
{
    public Animator anim;

    public Curve jumpCurve;
    public float playerStartT, coreStartT;
    public float coreTDistance;

    public Transform core;
    public Renderer coreRenderer;
    public Light coreLight;

    private Transform player;
    private Camera cam;
    private Vector3 curveOffset;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        player = other.transform;
        cam = other.GetComponentInChildren<Camera>();
        curveOffset = player.position - jumpCurve.GetCurvePointNormalised(playerStartT);
        SetInputActive(false);
        SetAnimVariables();
        anim.SetTrigger("Jump");
    }

    private void Update()
    {
        if(player == null) return;
        UpdateAnimVariables();
    }

    private void SetInputActive(bool active)
    {
        if (active)
            Axiom.Core.PlayerMovementDetails.EnableAllMovementInput();
        else
            Axiom.Core.PlayerMovementDetails.DisableAllMovementInput();
        Axiom.Core.PlayerMovementDetails.cameraLookEnabled = active;
    }

    private Vector3 camForward1;
    private float playerEndT;
    private float lightIntensity1;
    private void SetAnimVariables()
    {
        camForward1 = cam.transform.forward;
        playerEndT = 1f - coreTDistance;

        lightIntensity1 = coreLight.intensity;
    }

    [Range(0, 1)] public float pathProgress, pathCloseness;
    [Range(0, 1)] public float cameraLook;
    [Range(0, 1)] public float officeFade, coreLightFade;
    [Range(0, 1)] public float coreZoom;
    private void UpdateAnimVariables()
    {
        float t = Mathf.Lerp(playerStartT, playerEndT, pathProgress);
        player.position = jumpCurve.GetCurvePointNormalised(t) + curveOffset * (1f - pathCloseness);
        Vector3 camForward2 = jumpCurve.GetDerivativeNormalised(t);
        cam.transform.forward = Vector3.Lerp(camForward1, camForward2, cameraLook);

        float coreT = Mathf.Max(t + coreTDistance * (1f - coreZoom), coreStartT);
        core.position = jumpCurve.GetCurvePointNormalised(coreT);

        coreLight.intensity = Mathf.Lerp(lightIntensity1, 0f, coreLightFade);
        coreRenderer.material.SetColor("_BaseColor", new Color(1, 1, 1, 1f - officeFade));
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawWireSphere(jumpCurve.GetCurvePointNormalised(Mathf.Lerp(playerStartT, playerEndT, pathProgress)), 0.3f);
    //}
}
