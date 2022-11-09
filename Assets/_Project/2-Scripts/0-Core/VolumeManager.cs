using Axiom.Core;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Axiom.Core
{
    public class VolumeManager : MonoBehaviour
    {
        public static VolumeManager current;
        public static event Action OnMultiplierChanged;

        public StudioEventEmitter bgmEmitter;

        public float sfxVolumeMultiplier { get; private set; }
        public float bgmVolumeMultiplier { get; private set; }
        public float dialogVolumeMultiplier { get; private set; }

        private void Awake()
        {
            if (current != null && current != this) Destroy(current);
            else current = this;

            SetDefaultMultipliers();
        }

        private void OnEnable()
        {
            SettingsData.OnSettingUpdated += SetNewBGMVolume;
        }

        private void OnDisable()
        {
            SettingsData.OnSettingUpdated -= SetNewBGMVolume;
        }

        public void SetNewBGMVolume()
        {
            bgmEmitter.EventInstance.setVolume(SettingsData.bgmVolume / 100f * bgmVolumeMultiplier);
        }

        public void SetDefaultMultipliers()
        {
            sfxVolumeMultiplier = 1f;
            bgmVolumeMultiplier = 1f;
            dialogVolumeMultiplier = 1f;

            bgmEmitter.EventInstance.setVolume(SettingsData.bgmVolume / 100f * bgmVolumeMultiplier);
            OnMultiplierChanged?.Invoke();
        }

        public void SetInDialogMultipliers()
        {
            sfxVolumeMultiplier = 0.5f;
            bgmVolumeMultiplier = 0.6f;
            dialogVolumeMultiplier = 1f;

            bgmEmitter.EventInstance.setVolume(SettingsData.bgmVolume / 100f * bgmVolumeMultiplier);
            OnMultiplierChanged?.Invoke();
        }
    }
}
