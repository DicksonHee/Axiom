using System;
using System.Collections.Generic;
using Axiom.Player.Movement.StateMachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Axiom.NonEuclidean
{
    public class Portal : MonoBehaviour
    {
        public Portal otherPortal;
        public MeshRenderer screen;

        private Camera playerCam, portalCam;
        private RenderTexture viewTexture;
        private List<TrackedTransform> tracked = new List<TrackedTransform>();
        private Vector3 screenStartLocalPosition;

        public bool changeTest = true;
        
        private void Awake()
        {
            playerCam = Camera.main;
            portalCam = GetComponentInChildren<Camera>();
            portalCam.enabled = false;
            screenStartLocalPosition = screen.transform.localPosition;
            ProtectScreenFromClipping();
        }

        private void CreateViewTexture()
        {
            if (viewTexture == null || viewTexture.width != Screen.width || viewTexture.height != Screen.height)
            {
                if (viewTexture != null)
                    viewTexture.Release();
                viewTexture = new RenderTexture(Screen.width, Screen.height, 0);

                portalCam.targetTexture = viewTexture;

                otherPortal.screen.material.SetTexture("_MainTex", viewTexture);
            }
        }

        private static bool VisibleFromCamera(Renderer renderer, Camera camera)
        {
            Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
            return GeometryUtility.TestPlanesAABB(frustumPlanes, renderer.bounds);
        }

        //called just before player camera is rendered
        public void Render(ScriptableRenderContext ctx)
        {
            if (!VisibleFromCamera(otherPortal.screen, playerCam)) return;

            screen.enabled = false;
            CreateViewTexture();

            Matrix4x4 m = transform.localToWorldMatrix * otherPortal.transform.worldToLocalMatrix * playerCam.transform.localToWorldMatrix;
            portalCam.transform.SetPositionAndRotation(m.GetPosition(), m.rotation);

            SetNearClipPlane();

            //portalCam.Render();
            UniversalRenderPipeline.RenderSingleCamera(ctx, portalCam);

            screen.enabled = true;
        }

        protected void AddTrackedTransform(TrackedTransform t) { tracked.Add(t); ProtectScreenFromClipping(); }

        private void Update()
        {
            //print($"({gameObject.name}) {tracked.Count}");
            CheckTrackedTransforms();
        }

        private void CheckTrackedTransforms()
        {
            for (int i = 0; i < tracked.Count; i++)
            {
                //print(Camera.main.transform.position - tracked[i].transform.position);
                int dot = (int)Mathf.Sign(Vector3.Dot(transform.forward, tracked[i].transform.position - transform.position));
                int last = tracked[i].lastDotSign;
                tracked[i].lastDotSign = dot;
                //print($"({gameObject.name}) dot: {dot}, last: {last}, position: {tracked[i].transform.position - transform.position}");

                if (last < 0 && dot > 0 || last > 0 && dot < 0)
                {
                    Teleport(tracked[i]);
                    tracked.RemoveAt(i);
                    ProtectScreenFromClipping();
                }
            }
        }

        private void Teleport(TrackedTransform t)
        {
            print($"Teleporting from {gameObject.name}");
            Matrix4x4 m = otherPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * t.transform.localToWorldMatrix;

            if (t.transform.TryGetComponent(out MovementSystem controller))
            {
                //m.SetColumn(3, Vector4.zero);
                //controller.TransformTargetVelocity(m);
                //controller.orientation.rotation = m.rotation;

                if (changeTest)
                {
                    m = otherPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * controller.orientation.localToWorldMatrix;
                    controller.cameraLook.TransformForward(m.rotation);
                    controller.TransformTargetVelocity();
                }
                
                t.transform.position = m.GetPosition();
            }
            else t.transform.SetPositionAndRotation(m.GetPosition(), m.rotation);

            otherPortal.AddTrackedTransform(t);
        }

        private void SetNearClipPlane()
        {
            int dot = (int)Mathf.Sign(Vector3.Dot(transform.forward, transform.position - portalCam.transform.position));

            Vector3 camSpacePos = portalCam.worldToCameraMatrix.MultiplyPoint(transform.position);
            Vector3 camSpaceNormal = portalCam.worldToCameraMatrix.MultiplyVector(transform.forward) * dot;
            float camSpaceDst = -Vector3.Dot(camSpacePos, camSpaceNormal);
            Vector4 clipPlaneCameraSpace = new Vector4(camSpaceNormal.x, camSpaceNormal.y, camSpaceNormal.z, camSpaceDst);

            portalCam.projectionMatrix = playerCam.CalculateObliqueMatrix(clipPlaneCameraSpace);
        }

        private void ProtectScreenFromClipping()
        {
            float halfHeight = playerCam.nearClipPlane * Mathf.Tan(playerCam.fieldOfView * 0.5f * Mathf.Deg2Rad);
            float halfWidth = halfHeight * playerCam.aspect;
            float dstToNearClipPlaneCorner = new Vector3(halfWidth, halfHeight, playerCam.nearClipPlane).magnitude;

            bool camFacingSameDirAsPortal = Vector3.Dot(
                transform.forward, transform.position - playerCam.transform.position) > 0;

            screen.transform.localScale = new Vector3(
                screen.transform.localScale.x,
                screen.transform.localScale.y,
                dstToNearClipPlaneCorner);

            print(gameObject.name + ", " + camFacingSameDirAsPortal);
            screen.transform.localPosition = screenStartLocalPosition + Vector3.forward * dstToNearClipPlaneCorner * (camFacingSameDirAsPortal ? 0.5f : -0.5f);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (tracked.FindIndex(x => x.transform == other.transform) < 0)
                tracked.Add(new TrackedTransform(other.transform, 0));
        }

        private void OnTriggerExit(Collider other)
        {
            int index = tracked.FindIndex(x => x.transform == other.transform);
            if (index >= 0)
                tracked.RemoveAt(index);
        }

        protected class TrackedTransform
        {
            public Transform transform;
            public int lastDotSign;

            public TrackedTransform(Transform t, int lastDotSign)
            {
                transform = t;
                this.lastDotSign = lastDotSign;
            }
        }
        
        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (otherPortal != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(screen.transform.position, otherPortal.screen.transform.position);
            }
        }
        #endif
    }
}