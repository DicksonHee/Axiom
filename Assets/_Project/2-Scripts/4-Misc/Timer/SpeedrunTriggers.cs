using Axiom.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedrunTriggers : MonoBehaviour
{
    public void StartTimer()
    {
        if(SettingsData.isSpeedrunMode)
        {
            SpeedrunTimer.StartTimer();
        }
    }
}
