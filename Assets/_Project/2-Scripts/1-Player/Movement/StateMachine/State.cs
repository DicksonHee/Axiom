using UnityEngine;

namespace Axiom.Player.Movement.StateMachine
{
    public abstract class State
    {
        protected MovementSystem MovementSystem;

        protected float stateStartTime;

        private AnimationCurve movementCurve;
        private float previousSpeed;

        public StateName stateName { get; protected set; }
 
        public State(MovementSystem movementSystem)
        {
            MovementSystem = movementSystem;
        }

        public virtual void EnterState()
        {
            stateStartTime = Time.time;
            SelectMovementCurve();
        }

        public virtual void ExitState()
        {
        }

        public virtual void LogicUpdate()
        {
        }

        public virtual void PhysicsUpdate()
        {
        }

        protected void SelectMovementCurve()
        {
	        previousSpeed = MovementSystem.GetCurrentSpeed();
            movementCurve = MovementSystem.CurrentTargetSpeed > previousSpeed ? MovementSystem.accelerationCurve : MovementSystem.decelerationCurve;
		}
    }

    // For Debugging only
	public enum StateName
    {
        Idle,
        Walking,
        Running,
        BackRunning,
        Strafing,
        InAir,
        Climbing,
        Sliding,
        WallRunning,
        Crouching,
        LedgeGrabbing,
        LedgeClimbing,
        Vaulting,
        Landing,
        None
    }
}
