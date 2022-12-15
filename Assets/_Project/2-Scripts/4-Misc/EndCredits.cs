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
        StartCoroutine(FadeScreen());
        Invoke(nameof(ActuallyGoBackToTitle), 5f);
    }

    private void ActuallyGoBackToTitle()
    {
        SceneLoad_Manager.LoadSpecificScene("MainMenu");
    }

    private IEnumerator FadeScreen()
    {
        float counter = 0f;

        while(counter < 4f)
        {
            counter += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, counter / 4f);
            yield return null;
        }

        canvasGroup.alpha = 0f;
    }
}
