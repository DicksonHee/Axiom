using System;
using System.Collections;
using System.Collections.Generic;
using Axiom.Player.Movement.StateMachine;
using Axiom.Player.Movement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Axiom.NonEuclidean
{
    public class Portal : MonoBehaviour
    {
        public Portal otherPortal;
        public MeshRenderer screen;
        public bool isTeleportToBlue;
        
        [Header("Gravity")]
        public bool changeGravity;
        public Transform gravityDirection;
        [HideInInspector] public bool canTeleport = true;
        public bool teleportEnabled = true;
        public bool shouldRenderScreen = true;
        
        private Camera playerCam, portalCam;
        private RenderTexture viewTexture;
        private List<TrackedTransform> tracked = new List<TrackedTransform>();
        private Vector3 screenStartLocalPosition;
        
        private void Awake()
        {
            playerCam = Camera.main;
            portalCam = GetComponentInChildren<Camera>();
            portalCam.depthTextureMode = DepthTextureMode.Depth;
            portalCam.hideFlags = HideFlags.HideAndDontSave;
            portalCam.enabled = false;
            screenStartLocalPosition = screen.transform.localPosition;
            ProtectScreenFromClipping();
        }

        private void CreateViewTexture()
        {
            if (viewTexture != null && viewTexture.width == Screen.width && viewTexture.height == Screen.height) return;
            if (viewTexture != null) viewTexture.Release();
            viewTexture = new RenderTexture(Screen.width, Screen.height, 0);
            viewTexture.depth = 16;

            portalCam.targetTexture = viewTexture;

            otherPortal.screen.material.SetTexture("_MainTex", viewTexture);
        }

        public void ForceCreateViewTexture()
        {
            viewTexture = new RenderTexture(Screen.width, Screen.height, 0);
            viewTexture.depth = 16;

            portalCam.targetTexture = viewTexture;

            otherPortal.screen.material.SetTexture("_MainTex", viewTexture);
        }

        private static bool VisibleFromCamera(Renderer renderer, Camera camera)
        {
            Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
            return GeometryUtility.TestPlanesAABB(frustumPlanes, renderer.bounds);
        }

        //called just before player camera is rendered
        public void Render(ScriptableRenderContext ctx)
        {
            if (otherPortal == null || !VisibleFromCamera(otherPortal.screen, playerCam) || !shouldRenderScreen) return;

            screen.enabled = false;
            CreateViewTexture();

            Matrix4x4 m = transform.localToWorldMatrix * otherPortal.transform.worldToLocalMatrix * playerCam.transform.localToWorldMatrix;            
            portalCam.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);

            SetNearClipPlane();
            ProtectScreenFromClipping();

            //portalCam.Render();
            UniversalRenderPipeline.RenderSingleCamera(ctx, portalCam);

            screen.enabled = true;
        }

        protected void AddTrackedTransform(TrackedTransform t)
        {
            if (tracked.FindIndex(x => x.transform == t.transform) < 0)
            {
                tracked.Add(t);
                //print($"{transform.name} | Started Tracking");
            }
            ProtectScreenFromClipping(); 
        }

        private void RemoveTrackedTransform(TrackedTransform t)
        {
            if(tracked.Remove(t))
            //print($"{transform.name} | Stopped Tracking");
            ProtectScreenFromClipping();
        }

        private void LateUpdate()
        {
            //print($"({gameObject.name}) {tracked.Count}");
            //foreach (TrackedTransform t in tracked)
            //print($"{Time.time}, {t.transform.name} ({transform.name})");
            if (tracked.Count > 0) Debug.Log(this);
            CheckTrackedTransforms();
        }

        private void CheckTrackedTransforms()
        {
            for (int i = 0; i < tracked.Count; i++)
            {
                //print($"{transform.name} | {tracked[i].transform.name}");
                //print(Camera.main.transform.position - tracked[i].transform.position);
                int dot = (int)Mathf.Sign(Vector3.Dot(transform.forward, screen.transform.position - tracked[i].transform.position));
                int last = tracked[i].lastDotSign;
                tracked[i].lastDotSign = dot;
                //print($"({gameObject.name}) dot: {dot}, last: {last}, position: {tracked[i].transform.position - transform.position}");

                if (last < 0 && dot > 0 || last > 0 && dot < 0)
                    Teleport(tracked[i]);
            }
        }
        
        private void Teleport(TrackedTransform t)
        {
            if (!teleportEnabled || !t.transform.parent.parent.TryGetComponent(out MovementSystem controller))
                return;

            Matrix4x4 m = otherPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * controller.transform.localToWorldMatrix;
            Matrix4x4 portalM = otherPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix;

            controller.TeleportButForRealYo(m.GetPosition(), m.rotation, portalM.rotation);

            if (changeGravity)
                Physics.gravity = otherPortal.gravityDirection.forward;

            t.transform.gameObject.GetComponentInParent<MoveCamera>().ForceUpdate();

            RemoveTrackedTransform(t);
            t.lastDotSign = 0;
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

            //print(gameObject.name + ", " + camFacingSameDirAsPortal);
            screen.transform.localPosition = screenStartLocalPosition + Vector3.forward * dstToNearClipPlaneCorner * (camFacingSameDirAsPortal ? 0.5f : -0.5f);
        }

        private void OnTriggerEnter(Collider other)
        {
            Transform otherT = other.transform;
            if (other.TryGetComponent(out MovementSystem controller))
                otherT = playerCam.transform;

            if (tracked.FindIndex(x => x.transform == otherT) < 0)
            {
                tracked.Add(new TrackedTransform(otherT, 0));
                //print($"{transform.name} | Started Tracking");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Transform otherT = other.transform;
            if (other.TryGetComponent(out MovementSystem controller))
                otherT = playerCam.transform;

            //print($"{transform.name}, {other.transform.name}");
            int index = tracked.FindIndex(x => x.transform == otherT);
            if (index >= 0)
            {
                tracked.RemoveAt(index);
                //print($"{transform.name} | Stopped Tracking");
            }
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

                DrawArrow.ForGizmo(transform.position, transform.forward, Color.blue);
                DrawArrow.ForGizmo(transform.position, -transform.forward, Color.red);
            }

            if (changeGravity)
            {
                DrawArrow.ForGizmo(gravityDirection.position, gravityDirection.forward, Color.green);
            }

            foreach (TrackedTransform t in tracked)
            {
                if (!t.transform.parent.parent.TryGetComponent(out MovementSystem controller) || !canTeleport || !teleportEnabled)
                    return;

                Matrix4x4 m = otherPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * controller.transform.localToWorldMatrix;
                DrawArrow.ForGizmo((Vector3)m.GetColumn(3) + (controller.transform.position - t.transform.position), m.rotation * Vector3.down, Color.blue);
            }
        }
        #endif
    }
}