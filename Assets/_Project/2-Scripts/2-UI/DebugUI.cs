using TMPro;
using UnityEngine;
using Axiom.Player.Movement.StateMachine;
using UnityEngine.SceneManagement;

namespace Axiom.UI
{
    public class DebugUI : MonoBehaviour
    {
        private MovementSystem movementSystem;

        public TMP_Text _currentTimer;
        public TMP_Text _currentSpeed;
        public TMP_Text _currentState;
        public TMP_Text _previousState;

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
            GetPlayerMovementSystem();
        }

        private void Update()
        {
            if (movementSystem == null) return;
            _currentSpeed.text = "Current Speed: " + movementSystem.GetCurrentSpeed();
            _currentState.text = "Current State: " + movementSystem.GetCurrentStateName();
            _previousState.text = "Previous State: " + movementSystem.GetPreviousStatename();

            //if(SpeedrunTimer._isActive) _currentTimer.text = "" + Mathf.Round(SpeedrunTimer.GetElapsedTime() * 100f)/100f;
        }

        private void GetPlayerMovementSystem()
        {
            movementSystem = FindObjectOfType<MovementSystem>();
        }
    }
}
