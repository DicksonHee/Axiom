using JetBrains.Annotations;
using LootLocker.Requests;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Axiom.Core;

public class Leaderboard : MonoBehaviour
{
    public static Leaderboard current;

    public TMP_InputField memberID;
    public int leaderboardID;

    public CanvasGroup leaderboardCanvasGroup;
    public CanvasGroup enterScoreCanvasGroup;
    public TMP_Text finalTimeDisplay;

    public TMP_Text[] leaderboardDisplay;

    private void Awake()
    {
        if (current != null && current != this) Destroy(this);
        else current = this;

        HideCanvasGroup(enterScoreCanvasGroup);
        HideCanvasGroup(leaderboardCanvasGroup);
    }

    private void Start()
    {
        StartCoroutine(LoginRoutine());
    }

    private IEnumerator LoginRoutine()
    {
        bool done = false;

        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if(response.success)
            {
                Debug.Log("Player was logged in");
            }
            else
            {
                Debug.Log("Could not start session");
                done = true;
            }
        });

        yield return new WaitWhile(() => done == false);
    }

    public void ShowEnterScoreCanvas()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        finalTimeDisplay.text = ConvertToTime(ConvertToScore(SpeedrunTimer.GetEndTime()));
        ShowCanvasGroup(enterScoreCanvasGroup);
    }

    public void HideEnterScoreCanvas()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        HideCanvasGroup(enterScoreCanvasGroup);
    }

    public void ShowLeaderboardCanvas()
    {
        leaderboardCanvasGroup.alpha = 1f;

        LootLockerSDKManager.GetScoreList(leaderboardID, 10, (response) =>
        {
            if (response.success)
            {
                LootLockerLeaderboardMember[] leaderboardMembers = response.items;

                for (int ii = 0; ii < 10; ii++)
                {
                    if (ii >= leaderboardMembers.Length) leaderboardDisplay[ii].text = "";
                    else leaderboardDisplay[ii].text = "" + (ii + 1) + ". " + leaderboardMembers[ii].member_id + "  |  " + ConvertToTime(leaderboardMembers[ii].score);
                }
            }
            else
            {
                Debug.Log("Failed to get scores.");
            }
        });

        ShowCanvasGroup(leaderboardCanvasGroup);
    }

    public void HideLeaderboardCanvas()
    {
        HideCanvasGroup(leaderboardCanvasGroup);
    }

    private void ShowCanvasGroup(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }

    private void HideCanvasGroup(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    public void SubmitScore()
    {
        if (string.IsNullOrEmpty(memberID.text)) return;

        float endTime = SpeedrunTimer.GetEndTime();
        LootLockerSDKManager.SubmitScore(memberID.text, ConvertToScore(endTime), leaderboardID, (response) =>
        {
            if (response.success) Debug.Log("Successfully submitted score");
            else Debug.Log("Unable to submit score: " + response.Error);
        });

        HideEnterScoreCanvas();
        HideLeaderboardCanvas();
        Invoke(nameof(LoadMainMenu), 1f);
    }

    private void LoadMainMenu()
    {
        SceneLoad_Manager.LoadSpecificScene("MainMenu");
    }

    [System.Obsolete]
    public string[] GetScores()
    {
        string[] leaderboard = new string[10];

        LootLockerSDKManager.GetScoreList(leaderboardID, 10, (response) =>
        {
            if (response.success)
            {
                LootLockerLeaderboardMember[] leaderboardMembers = response.items;

                for (int ii = 0; ii < leaderboardMembers.Length; ii++)
                {
                    leaderboard[ii] = ConvertToTime(leaderboardMembers[ii].score);
                }
            }
            else
            {
                Debug.Log("Failed to get scores.");
            }
        });

        return leaderboard;
    }

    public int ConvertToScore(float time)
    {
        float floatTime = time * 100;
        int intTime = (int) floatTime;

        return intTime;
    }

    public string ConvertToTime(int score)
    {
        int miliSeconds = score % 100;
        int seconds = (score / 100) % 60;
        int minutes = (score / 100) / 60;

        return "" + minutes + "m " + seconds + "s " + miliSeconds + "ms";
    }
}
