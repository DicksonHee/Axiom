using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Axiom.Core
{
    public static class SettingsData
    {
        public static event Action OnSettingUpdated;

        public static int mouseSensitivity;
        public static int bgmVolume;
        public static int sfxVolume;
        public static int dialogVolume;

        public static int textSize;
        public static int textBackgroundOpacity;

        public static bool isSpeedrunMode = true;

        public static void InitialiseValues()
        {
            mouseSensitivity = 50;
            bgmVolume = 75;
            sfxVolume = 75;
            dialogVolume = 50;
            textSize = 1;
            textBackgroundOpacity = 100;
        }
        
        public static void SettingsUpdated()
        {
            OnSettingUpdated?.Invoke();
        }
    }
}