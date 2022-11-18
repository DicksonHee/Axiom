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

        public static void SettingsUpdated()
        {
            OnSettingUpdated?.Invoke();
        }
    }
}