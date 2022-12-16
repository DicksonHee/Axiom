using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpeedrunTimer
{
    private static float _startTime;
    private static float _endTime;
    
    public static bool _isActive;

	public static void StartTimer()
    {
        _startTime = Time.time;
        _isActive = true;
    }

    public static void EndTimer()
    {
        if (!_isActive) return;

        _endTime = Time.time - _startTime;
        _isActive = false;
    }

    public static float GetEndTime()
    {
        if (_isActive) EndTimer();
        return _endTime;
    }

    public static float GetElapsedTime()
    {
        return Time.time - _startTime;
    }
}