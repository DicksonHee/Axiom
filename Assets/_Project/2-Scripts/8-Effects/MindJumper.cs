using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Axiom.UI.MainMenu;
using UnityEngine.SceneManagement;

namespace Axiom
{
    public class MindJumper : MonoBehaviour
    {
        public string sceneToLoad;

        [Header("Anim")]
        public Animator jumpAnim;
        private bool jumping = false, activated = false;

        [Header("Zoom")]
        public Camera cam;
        public LayerMask interactMask;
        public Animator target;
        public Transform zoomTarget;
        public Vector3 teleport;
        public AnimationCurve zoomCurve;
        public float zoomTime;

        [Header("Silhouette")]
        public Transform voidBall;
        public Renderer[] targetRenderers;
        public Material silhouetteMaterial;

        [Header("Cubes")]
        public GameObject mindTransitionObject;
        private MindTransition mindTransition;

        private void Awake()
        {
            //ringRadii = new float[animObjects.Length];
            //for (int i = 0; i < animObjects.Length; i++)
            //{
            //    ringRadii[i] = animObjects[i].radius;
            //    animObjects[i].radius *= 10f;
            //}
        }

        private void Update()
        {
            if (jumping)
            {
                UpdateAnimationStuff();
                return;
            }

            if (!Physics.Raycast(cam.ViewportPointToRay(Vector3.one / 2), out RaycastHit hit, 100f, interactMask, QueryTriggerInteraction.Collide))
                return;

            if (!hit.transform.IsChildOf(target.transform))
                return;

            if (!activated && Input.GetKeyDown(KeyCode.E))
            {
                activated = true;
                Core.PostProcessingActions.current.RespawnAnimation(2f);
                Invoke(nameof(Jump), 2f);
            }
        }

        private Vector3 camForward1, camForward2;
        private float camFOV1, camRot1, camRot2;
        private void SetAnimationVariables()
        {
            camForward1 = cam.transform.forward;
            camForward2 = (zoomTarget.transform.position - cam.transform.position).normalized;
            camFOV1 = cam.fieldOfView;
            camRot1 = cam.transform.rotation.eulerAngles.z;
            camRot2 = camRot1 + 30f;
        }

        [Header("Animation Variables")]
        [Range(0, 1)] public float camForwardCorrection;
        [Range(0, 1)] public float ringRotationSlow;
        [Range(0, 1)] public float camZoom;
        private void UpdateAnimationStuff()
        {
            cam.transform.forward = Vector3.Lerp(camForward1, camForward2, camForwardCorrection);

            foreach (var ring in mindTransition.animObjects)
                ring.rotSpeedMult = Mathf.Lerp(10f, 1f, ringRotationSlow);

            cam.fieldOfView = Mathf.Lerp(camFOV1, 0f, camZoom);

            Vector3 camRot = cam.transform.rotation.eulerAngles;
            camRot.z = Mathf.Lerp(camRot1, camRot2, camZoom);
            cam.transform.rotation = Quaternion.Euler(camRot);
        }

        private void Jump()
        {
            mindTransition = Instantiate(mindTransitionObject).GetComponent<MindTransition>();

            voidBall.gameObject.SetActive(true);
            cam.transform.SetParent(null);
            target.transform.SetParent(null);

            cam.transform.position += teleport;
            target.transform.position += teleport;

            for (int r = 0; r < targetRenderers.Length; r++)
                targetRenderers[r].material = silhouetteMaterial;

            target.speed = 0f;

            SetAnimationVariables();
            jumpAnim.SetTrigger("Jump");
            
            jumping = true;
        }

        public void HeadReached()
        {
            voidBall.GetComponent<Renderer>().material = silhouetteMaterial;
            target.gameObject.SetActive(false);
            foreach (MainMenuAnim obj in mindTransition.animObjects) obj.ChangeColor();
            StartCoroutine(SwitchScenes());
        }

        public void PerspectiveSwitch(float duration) => mindTransition.perspectiveSwitcher.StartPerspectiveSwitch(duration);

        private IEnumerator SwitchScenes()
        {
            if (!SceneManager.GetSceneByName("Load_Scene").isLoaded) SceneManager.LoadSceneAsync("Load_Scene", LoadSceneMode.Additive);
            while (!SceneManager.GetSceneByName("Load_Scene").isLoaded)
            {
                yield return null;
            }

            LoadScreen.current.SetBlack();
            LoadScreen.current.SetOpaque();
            yield return new WaitForSeconds(2f);

            SceneLoad_Manager.LoadSpecificScene(sceneToLoad);

            print("StartingLoad");
            yield return new WaitUntil(() => !SceneLoad_Manager.Busy);
            print("FinishedLoad");

            NextScene();
            print("Next Scene");
        }

        private void NextScene()
        {
            mindTransition.overlayCam.transform.DOKill();
            mindTransition.overlayCam.transform.DORotate(new Vector3(0, 0, 90f), 2f, RotateMode.FastBeyond360).SetEase(Ease.InOutQuad);
            mindTransition.overlayCam.transform.DOMoveZ(10, 2f);
            mindTransition.overlayFadeAnim.SetTrigger("Fade");
        }
    }
}
