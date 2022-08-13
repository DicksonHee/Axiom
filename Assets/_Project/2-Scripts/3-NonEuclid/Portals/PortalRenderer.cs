using UnityEngine;
using UnityEngine.Rendering;

namespace Axiom.NonEuclidean
{
    public class PortalRenderer : MonoBehaviour
    {
        Portal[] portals;

        private void Awake()
        {
            portals = FindObjectsOfType<Portal>();
        }

        private void RenderPortals(ScriptableRenderContext ctx, Camera cam)
        {
            if (cam != Camera.main) return;

            foreach (Portal portal in portals)
                if (portal) portal.Render(ctx);
        }

        private void OnEnable()
        {
            RenderPipelineManager.beginCameraRendering += RenderPortals;
        }

        private void OnDisable()
        {
            RenderPipelineManager.beginCameraRendering -= RenderPortals;
        }
    }
}