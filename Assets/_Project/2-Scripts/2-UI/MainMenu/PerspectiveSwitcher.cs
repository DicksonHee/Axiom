using UnityEngine;

namespace Axiom.UI.MainMenu
{
    [RequireComponent(typeof(MatrixBlender))]
    public class PerspectiveSwitcher : MonoBehaviour
    {
        public float fov = 60f, near = .3f, far = 1000f, orthographicSize = 50f;

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
        }

        public Coroutine StartPerspectiveSwitch()
        {
            orthoOn = !orthoOn;
            return orthoOn ? blender.BlendToMatrix(ortho, 2f, 8, true) : blender.BlendToMatrix(perspective, 2f, 8, false);
        }
    }
}