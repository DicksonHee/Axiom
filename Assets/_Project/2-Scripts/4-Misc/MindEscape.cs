using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bezier;
using UnityEngine.SceneManagement;

public class MindEscape : MonoBehaviour
{
    public Animator anim;

    public RespawnTrigger areaRespawnTrigger;
    public string sceneToLoad;

    public Curve jumpCurve;
    public float playerStartT, coreStartT;
    public float coreTDistance;

    public Transform core, landSpot;
    public Renderer coreRenderer;
    public Light coreLight;
    public float officeScale;

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
        areaRespawnTrigger.isEnabled = false;
        areaRespawnTrigger.gameObject.SetActive(false);
        StartCoroutine(ILoadScene());
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

    private Vector3 camForward1, camForward2;
    private float playerEndT;
    private float lightIntensity1;
    private float coreScale1, coreScale2;
    private void SetAnimVariables()
    {
        camForward1 = cam.transform.forward;
        camForward2 = jumpCurve.GetDerivativeNormalised(1f).normalized;
        playerEndT = 1f - coreTDistance;

        lightIntensity1 = coreLight.intensity;

        coreScale1 = core.transform.localScale.x;
        coreScale2 = coreScale1 * (1f / officeScale);
    }

    [Range(0, 1)] public float pathProgress, pathCloseness;
    [Range(0, 1)] public float cameraLook;
    [Range(0, 1)] public float officeFade, coreLightFade;
    [Range(0, 1)] public float coreZoom;
    private void UpdateAnimVariables()
    {
        float t = Mathf.Lerp(playerStartT, playerEndT, pathProgress);

        Vector3 curvePosition = jumpCurve.GetCurvePointNormalised(t) + curveOffset * (1f - pathCloseness);
        player.position = Vector3.Lerp(curvePosition, landSpot.position, coreZoom);
        cam.transform.forward = Vector3.Lerp(camForward1, camForward2, cameraLook);

        float coreT = Mathf.Max(t + coreTDistance, coreStartT); // coreTDistance * (1f - coreZoom)
        core.position = jumpCurve.GetCurvePointNormalised(coreT);
        core.localScale = Vector3.one * Mathf.Lerp(coreScale1, coreScale2, coreZoom);

        coreLight.intensity = Mathf.Lerp(lightIntensity1, 0f, coreLightFade);
        coreRenderer.material.SetColor("_BaseColor", new Color(1, 1, 1, 1f - officeFade));
    }

    public void LoadScene() => canLoad = true;

    private bool canLoad = false;
    private IEnumerator ILoadScene()
    {
        if (!SceneManager.GetSceneByName("Load_Scene").isLoaded) SceneManager.LoadSceneAsync("Load_Scene", LoadSceneMode.Additive);
        while (!SceneManager.GetSceneByName("Load_Scene").isLoaded)
        {
            yield return null;
        }

        yield return new WaitUntil(() => canLoad);

        LoadScreen.current.SetBlack();
        LoadScreen.current.SetOpaque();
        SceneLoad_Manager.LoadSpecificScene(sceneToLoad);
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawWireSphere(jumpCurve.GetCurvePointNormalised(Mathf.Lerp(playerStartT, playerEndT, pathProgress)), 0.3f);
    //}
}
