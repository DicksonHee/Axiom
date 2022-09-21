using TMPro;
using UnityEngine;
using Axiom.Player.Movement.StateMachine;

namespace Axiom.UI
{
    public class DebugUI : MonoBehaviour
    {
        public MovementSystem _movementSystem;

        public TMP_Text _currentTimer;
        public TMP_Text _currentSpeed;
        public TMP_Text _currentState;
        public TMP_Text _previousState;

        private void Update()
        {
            _currentSpeed.text = "Current Speed: " + _movementSystem.GetCurrentSpeed();
            _currentState.text = "Current State: " + _movementSystem.GetCurrentStateName();
            _previousState.text = "Previous State: " + _movementSystem.GetPreviousStatename();

            //if(SpeedrunTimer._isActive) _currentTimer.text = "" + Mathf.Round(SpeedrunTimer.GetElapsedTime() * 100f)/100f;
        }
    }
}
