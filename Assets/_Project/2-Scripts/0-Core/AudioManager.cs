using Axiom.Core;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.SceneManagement;
using Axiom.Dialogue;

namespace Axiom.Core
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager current;
        public StudioEventEmitter bgmEmitter;
        public List<SceneBGM> sceneBgms;

        private void Awake()
        {
            if (current != null && current != this) Destroy(current);
            else current = this;
            
            SetNewBGMVolume();
            SetNewSFXVolume();
        }

        private void OnEnable()
        {
            SettingsData.OnSettingUpdated += SetNewBGMVolume;
            SettingsData.OnSettingUpdated += SetNewSFXVolume;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SettingsData.OnSettingUpdated -= SetNewBGMVolume;
            SettingsData.OnSettingUpdated -= SetNewSFXVolume;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            foreach (SceneBGM bgm in sceneBgms)
            {
                if (scene.name == bgm.sceneName)
                {
                    if (bgm.bgmEventReference.ToString() == bgmEmitter.EventReference.ToString()) return;
                    SetNewBGM(bgm.bgmEventReference, 1f);
                }
            }
        }
        
        private void SetNewBGMVolume()
        {
            RuntimeManager.StudioSystem.setParameterByName("MasterMusicFader", SettingsData.bgmVolume / 100f);
        }

        private void SetNewSFXVolume()
        {
            RuntimeManager.StudioSystem.setParameterByName("MasterSFXFader", SettingsData.sfxVolume / 100f);
        }

        public void PauseDialogue() => ProgrammerSounds.current.SetPauseState(true);
        public void UnpauseDialogue() => ProgrammerSounds.current.SetPauseState(false);

        public void SetNewBGM(EventReference newEventReference, float duration)
        {
            StartCoroutine(SetNewBGM_CO(newEventReference, duration));
        }

        public void PlaySFX2D(EventReference eventReference)
        {
            RuntimeManager.PlayOneShot(eventReference);
        }

        public void PlaySFX3D(EventReference eventReference, Transform targetTransform)
        {
            RuntimeManager.PlayOneShot(eventReference, targetTransform.position);
        }
        
        public void LerpBGMParameter(string parameterName, float initialValue, float finalValue, float duration)
        {
            StartCoroutine(LerpBGMParameter_CO(parameterName, initialValue, finalValue, duration));
        }

        private IEnumerator LerpBGMParameter_CO(string parameterName, float initialValue, float finalValue, float duration)
        {
            float counter = 0f;

            while(counter < duration)
            {
                Debug.Log(parameterName);
                counter += Time.deltaTime;
                RuntimeManager.StudioSystem.setParameterByName(parameterName, Mathf.Lerp(initialValue, finalValue, counter / duration));
                yield return null;
            }
        }

        private IEnumerator SetNewBGM_CO(EventReference newEventReference, float duration)
        {
            if (newEventReference.IsNull)
            {
                RuntimeManager.StudioSystem.setParameterByName("MasterMusicFader", 0);
                yield break;
            }

            float counter = 0f;
            float startVal = SettingsData.bgmVolume / 100f;
            
            while (counter < duration)
            {
                counter += Time.deltaTime;
                RuntimeManager.StudioSystem.setParameterByName("MasterMusicFader", Mathf.Lerp(startVal, 0, counter/duration));
                yield return null;
            }

            counter = 0f;
            bgmEmitter.ChangeEvent(newEventReference.Path);
            yield return new WaitForSeconds(1f);
            bgmEmitter.Play();
            
            while (counter < duration)
            {
                counter += Time.deltaTime;
                RuntimeManager.StudioSystem.setParameterByName("MasterMusicFader", Mathf.Lerp(0, startVal, counter/duration));
                yield return null;
            }
        }
    }

    [Serializable]
    public class SceneBGM
    {
        public string sceneName;
        public EventReference bgmEventReference;
    }
}
