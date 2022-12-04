using UnityEngine;

namespace Axiom.Player.Movement.StateMachine
{
    public abstract class StateMachine : MonoBehaviour
    {
        protected State CurrentState;
        protected State PreviousState;

        protected void InitializeState(State state)
        {
            PreviousState = state;
            CurrentState = state;
            CurrentState.EnterState();
        }
        
        public virtual void ChangeState(State state)
        {
            Debug.Log(CurrentState.ToString());
            PreviousState = CurrentState;
            CurrentState.ExitState();
            CurrentState = state;
            CurrentState.EnterState();
            Debug.Log(CurrentState.ToString());
        }
    }
}
