using Axiom.Core;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Axiom.Core
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager current;

        private void Awake()
        {
            if (current != null && current != this) Destroy(current);
            else current = this;
        }

        private void OnEnable()
        {
            SettingsData.OnSettingUpdated += SetNewBGMVolume;
            SettingsData.OnSettingUpdated += SetNewSFXVolume;
        }

        private void OnDisable()
        {
            SettingsData.OnSettingUpdated -= SetNewBGMVolume;
            SettingsData.OnSettingUpdated -= SetNewSFXVolume;
        }

        private void SetNewBGMVolume()
        {
            RuntimeManager.StudioSystem.setParameterByName("MasterMusicFader", SettingsData.bgmVolume / 100f);
        }

        private void SetNewSFXVolume()
        {
            RuntimeManager.StudioSystem.setParameterByName("MasterSFXFader", SettingsData.sfxVolume / 100f);
        }
    }
}
