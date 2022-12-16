using Axiom.Core;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpeedrunTimerUI : MonoBehaviour
{
    public static SpeedrunTimerUI current;

    public TMP_Text timerDisplay;

    private void Awake()
    {
        if (current != null && current != this) Destroy(this);
        else current = this;

        if(SettingsData.isSpeedrunMode && SpeedrunTimer._isActive)
        {
            timerDisplay.text = "" + Leaderboard.current.ConvertToTime(Leaderboard.current.ConvertToScore(SpeedrunTimer.GetElapsedTime()));
        }
        else
        {
            timerDisplay.text = "";
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Credits") timerDisplay.text = "";
    }

    private void Update()
    {
        if (!SettingsData.isSpeedrunMode || !SpeedrunTimer._isActive) return;
        timerDisplay.text = "" + Leaderboard.current.ConvertToTime(Leaderboard.current.ConvertToScore(SpeedrunTimer.GetElapsedTime()));
    }
}
