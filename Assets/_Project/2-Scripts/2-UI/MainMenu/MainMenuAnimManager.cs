using System;
using System.Collections;
using System.Collections.Generic;
using Axiom.Core;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

namespace Axiom.UI.MainMenu
{
    public class MainMenuAnimManager : MonoBehaviour
    {
        public List<MainMenuAnim> animObjects;

        public UnityEvent OnStartEvent, OnFinishEvent;
        public GameObject persistentObject;
        
        private PerspectiveSwitcher perspectiveSwitcher;
        private AxiomAnimator axiomAnimator;
        private AsyncOperation sceneAsync;

        private void Awake()
        {
            SettingsData.isSpeedrunMode = false;
            perspectiveSwitcher = GetComponent<PerspectiveSwitcher>();
            axiomAnimator = GetComponent<AxiomAnimator>();

            if (!GameObject.FindWithTag("PersistentObj")) Instantiate(persistentObject);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void StartGame(string sceneToLoad)
        {
            StartCoroutine(GameStart_CO(sceneToLoad));
        }

        public void StartSpeedrunGame(string sceneToLoad)
        {
            SettingsData.isSpeedrunMode = true;
            StartCoroutine(GameStart_CO(sceneToLoad));
        }

        public void ShowLeaderboard()
        {
            
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
            
            if (!SceneManager.GetSceneByName("Load_Scene").isLoaded) SceneManager.LoadSceneAsync("Load_Scene", LoadSceneMode.Additive);
            while(!SceneManager.GetSceneByName("Load_Scene").isLoaded)
            {
                yield return null;
            }
            
            LoadScreen.current.SetWhite();
            LoadScreen.current.SetOpaque();
            yield return new WaitForSeconds(2f);
            
            SceneLoad_Manager.LoadSpecificScene(sceneToLoad);

            print("StartingLoad");
            yield return new WaitUntil(() => !SceneLoad_Manager.Busy);
            print("FinishedLoad");

            yield return new WaitForSeconds(1.5f);
            NextScene();

            OnFinishEvent?.Invoke();
            //SceneManager.LoadScene(sceneToLoad);
        }

        private void NextScene()
        {
            transform.DOKill();
            transform.DORotate(new Vector3(0, 0, 90f), 2f, RotateMode.FastBeyond360).SetEase(Ease.InOutQuad);
            transform.DOMoveZ(10, 2f);
        }

        public void Destroy()
        {
            Destroy(transform.parent.gameObject);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
