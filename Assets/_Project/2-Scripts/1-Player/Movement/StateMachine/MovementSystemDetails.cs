using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Axiom.Player.StateMachine
{
    public class MovementSystemDetails : MonoBehaviour
    {
        public MovementSystem _movementSystem;

        public TMP_Text _currentSpeed;
        public TMP_Text _currentState;
        public TMP_Text _previousState;

        private void Update()
        {
            _currentSpeed.text = "Current Speed: " + _movementSystem.GetCurrentSpeed();
            _currentState.text = "Current State: " + _movementSystem.GetCurrentStateName();
            _previousState.text = "Previous State: " + _movementSystem.GetPreviousStatename();
        }
    }
}
