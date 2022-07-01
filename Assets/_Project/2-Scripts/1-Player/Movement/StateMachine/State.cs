using System;
using System.Collections;
using System.Collections.Generic;
using Axiom.Player.Movement;
using UnityEngine;

namespace Axiom.Player.StateMachine
{
    public abstract class State
    {
        protected MovementSystem MovementSystem;

        protected float stateStartTime;
        protected StateName previousState;
        
        protected AnimationCurve movementCurve;
        protected float previousSpeed;

        public StateName stateName { get; protected set; }

        public State(MovementSystem movementSystem)
        {
            MovementSystem = movementSystem;
        }

        public virtual void EnterState(StateName prevState)
        {
            stateStartTime = Time.time;
            previousState = prevState;
            
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
			switch (previousState)
			{
				case StateName.Idle:
					previousSpeed = MovementSystem.idleSpeed;
					break;
				case StateName.Walking:
					previousSpeed = MovementSystem.walkSpeed;
					break;
				case StateName.Running:
					previousSpeed = MovementSystem.forwardSpeed;
					break;
				case StateName.Strafing:
					previousSpeed = MovementSystem.strafeSpeed;
					break;
				case StateName.InAir:
					previousSpeed = MovementSystem.inAirSpeed;
					break;
				case StateName.Climbing:
					break;
				case StateName.Sliding:
					previousSpeed = 0f;
					break;
				case StateName.WallRunning:
					previousSpeed = MovementSystem.walkSpeed;
					break;
				case StateName.BackRunning:
					previousSpeed = MovementSystem.backwardSpeed;
					break;
				case StateName.Crouching:
					previousSpeed = MovementSystem.crouchSpeed;
					break;
				case StateName.LedgeGrabbing:
					break;
				case StateName.LedgeClimbing:
					break;
				case StateName.Vaulting:
					break;
			}

            movementCurve = MovementSystem.currentTargetSpeed > previousSpeed ? MovementSystem.accelerationCurve : MovementSystem.decelerationCurve;
		}

		protected void CalculateMovementSpeed(AnimationCurve curve = null, float startSpeed = 0f, bool resetTime = false)
        {
            if (movementCurve == null) return;
            if (startSpeed > 0f) previousSpeed = startSpeed;
            if (resetTime) stateStartTime = Time.time;
            MovementSystem.CalculateMovementSpeed(curve ?? movementCurve, previousSpeed, Time.time - stateStartTime);
        }
	}

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
        None
    }
}
