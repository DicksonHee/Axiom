using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Axiom.Tutorial
{
    public class TutorialUI : MonoBehaviour
    {
        public List<TutorialCanvases> canvasGroups;

        private bool exitEarly;

        public void ShowIndicator(string canvasGroupName)
        {
            TriggerIndicator(GetCanvasGroupByName(canvasGroupName), 1, 0.5f);
        }

        public void HideIndicator(string canvasGroupName)
        {
            TriggerIndicator(GetCanvasGroupByName(canvasGroupName), 0, 0.5f);
        }

        private void TriggerIndicator(CanvasGroup canvasGroup, float targetAlpha, float duration)
        {
            StartCoroutine(Indicate(canvasGroup, targetAlpha, duration));
        }

        private CanvasGroup GetCanvasGroupByName(string canvasGroupName)
        {
            foreach (TutorialCanvases canvas in canvasGroups)
            {
                if (canvasGroupName == canvas.name) return canvas.canvasGroup;
            }

            return null;
        }


        private IEnumerator Indicate(CanvasGroup canvasGroup, float targetAlpha, float duration)
        {
            float startAlpha = canvasGroup.alpha;

            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);

                if(exitEarly)
                {
                    exitEarly = false;
                    yield break;
                }
                yield return null;
            }

            canvasGroup.alpha = targetAlpha;
        }
    }

    [Serializable]
    public class TutorialCanvases
    {
        public string name;
        public CanvasGroup canvasGroup;
    }
}