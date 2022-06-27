using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Axiom.Player.StateMachine
{
    public abstract class StateMachine : MonoBehaviour
    {
        protected State CurrentState;
        private StateName PreviousState;

        protected void InitializeState(State state)
        {
            CurrentState = state;
            CurrentState.EnterState(StateName.None);
        }
        
        public void ChangeState(State state)
        {
            Debug.Log(CurrentState.stateName);
            PreviousState = CurrentState.stateName;
            CurrentState.ExitState();
            CurrentState = state;
            CurrentState.EnterState(PreviousState);
            Debug.Log(CurrentState.stateName);
        }
    }
}
