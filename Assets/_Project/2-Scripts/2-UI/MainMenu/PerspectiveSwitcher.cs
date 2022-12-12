using System;
using System.Collections;
using UnityEngine;

namespace Axiom.UI.MainMenu
{
    [RequireComponent(typeof(MatrixBlender))]
    public class PerspectiveSwitcher : MonoBehaviour
    {
        public float fov = 60f, near = .3f, far = 1000f, orthographicSize = 50f;
        public static event Action OnFinishLerpCam;

        private Matrix4x4 ortho, perspective;
        private float aspect;
        private MatrixBlender blender;
        private bool orthoOn;
    
        private Camera m_camera;

        void Start()
        {
            aspect = (float) Screen.width / Screen.height;
            ortho = Matrix4x4.Ortho(-orthographicSize * aspect, orthographicSize * aspect, -orthographicSize,
                orthographicSize, near, far);
            perspective = Matrix4x4.Perspective(fov, aspect, near, far);
            m_camera = GetComponent<Camera>();
            m_camera.projectionMatrix = ortho;
            orthoOn = true;
            blender = (MatrixBlender)GetComponent(typeof(MatrixBlender));

            StartCoroutine(LerpOrthoCam());
        }

        private IEnumerator LerpOrthoCam()
        {
            float counter = 0f;

            while(counter < 30f)
            {
                counter += Time.deltaTime;
                float orthoSize = Mathf.Lerp(0.5f, 7f, counter / 30f);
                ortho = Matrix4x4.Ortho(-orthoSize * aspect, orthoSize * aspect, -orthoSize, orthoSize, near, far);
                m_camera.projectionMatrix = ortho;
                
                yield return null;
            }

            m_camera.orthographicSize = 7f;
            OnFinishLerpCam?.Invoke();
        }

        public Coroutine StartPerspectiveSwitch(float duration = 2f)
        {
            orthoOn = !orthoOn;
            return orthoOn ? blender.BlendToMatrix(ortho, duration, 8, true) : blender.BlendToMatrix(perspective, 2f, 8, false);
        }
    }
}