using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Axiom.Player.StateMachine
{
    public abstract class StateMachine : MonoBehaviour
    {
        protected State CurrentState;

        public void InitializeState(State state)
        {
            CurrentState = state;
            CurrentState.EnterState();
        }
        
        public void ChangeState(State state)
        {
            CurrentState.ExitState();
            CurrentState = state;
            CurrentState.EnterState();
        }
    }
}
