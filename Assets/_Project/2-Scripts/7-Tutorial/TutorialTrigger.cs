using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Axiom.Tutorial
{
    public class TutorialTrigger : MonoBehaviour
    {
        public TutorialUI tutorialUI;
        public string tutorialName;

        public void ShowTutorial()
        {
            tutorialUI.ShowIndicator(tutorialName);
        }

        public void HideTutorial()
        {
            tutorialUI.HideIndicator(tutorialName);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                ShowTutorial();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                HideTutorial();
            }
        }
    }
}