using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Axiom.UI.MainMenu
{
    public class MainMenuAnimManager : MonoBehaviour
    {
        public List<MainMenuAnim> animObjects;

        public UnityEvent OnStartEvent;
        
        private PerspectiveSwitcher perspectiveSwitcher;
        private AxiomAnimator axiomAnimator;
        private AsyncOperation sceneAsync;

        private void Awake()
        {
            perspectiveSwitcher = GetComponent<PerspectiveSwitcher>();
            axiomAnimator = GetComponent<AxiomAnimator>();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void StartGame(string sceneToLoad)
        {
            StartCoroutine(GameStart_CO(sceneToLoad));
        }

        private IEnumerator GameStart_CO(string sceneToLoad)
        {
            OnStartEvent?.Invoke();
            axiomAnimator.StopAnim();
            transform.DOMove(new Vector3(0, 0, -16), 1.5f);
            yield return new WaitForSeconds(2f);

            perspectiveSwitcher.StartPerspectiveSwitch();
            yield return new WaitForSeconds(2f);
            axiomAnimator.ZoomLogo();
            yield return new WaitForSeconds(1.4f);

            foreach(MainMenuAnim obj in animObjects) obj.ChangeColor();
            transform.DOMove(new Vector3(0f, 0f, -5.5f), 2f).SetEase(Ease.InBounce).SetEase(Ease.OutCubic);
            yield return new WaitForSeconds(2f);
        
            transform.DORotate(new Vector3(0, 0, 360f), 60f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Incremental);

            yield return new WaitForSeconds(1f);
            
            NextScene();

            yield return new WaitForSeconds(1f);

            SceneManager.LoadScene(sceneToLoad);
        }

        private void NextScene()
        {
            transform.DOKill();
            transform.DORotate(new Vector3(0, 0, 90f), 2f, RotateMode.FastBeyond360).SetEase(Ease.InOutQuad);
            transform.DOMoveZ(10, 2f);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
