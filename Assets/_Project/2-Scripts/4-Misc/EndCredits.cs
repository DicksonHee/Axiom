using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class EndCredits : MonoBehaviour
{
    public VideoPlayer player;
    public CanvasGroup canvasGroup;

    private void OnEnable()
    {
        player.loopPointReached += GoBackToTitle;
    }

    private void OnDisable()
    {
        player.loopPointReached -= GoBackToTitle;
    }

    private void GoBackToTitle(VideoPlayer source)
    {
        SceneLoad_Manager.LoadSpecificScene("MainMenu");
    }
}
