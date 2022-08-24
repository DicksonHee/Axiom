using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpeedrunTimer
{
    private static Dictionary<string, float> _sectionTimes = new();
    private static float _startTime;
    
    public static bool _isActive;

	public static void StartTimer()
    {
        _startTime = Time.time;
        _isActive = true;
    }

    public static void EndTimer(string sectionName)
    {
        if (_sectionTimes.TryGetValue(sectionName, out _)) _sectionTimes[sectionName] = Time.time - _startTime;
        else _sectionTimes.Add(sectionName, Time.time - _startTime);

        _isActive = false;
    }

    public static float GetElapsedTime() => Time.time - _startTime;
}
